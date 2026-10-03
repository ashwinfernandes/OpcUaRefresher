using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Subscriptions;

internal sealed class OpcUaSubscriberService(ILogger<OpcUaSubscriberService> logger, Func<CancellationToken, Task<ManagedSession>> connect) : BackgroundService
{
    private readonly IReadOnlyCollection<NodeId> nodeIdsToSubscribe = new List<NodeId>
    {
        new(1001, 3),
        new(1002, 3),
        new(1003, 3)
    };

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OPC UA Subscriber Service is starting.");

        try
        {
            var session = await connect(cancellationToken);

            await using (session)
            {
                logger.LogInformation("Connected to OPC UA server: {Endpoint}", session.Endpoint.EndpointUrl);
                await Task.WhenAll(nodeIdsToSubscribe.Select(nodeId => MonitorAsync(session, $"Node {nodeId}", nodeId, cancellationToken)));
            }

            // Implement your OPC UA subscription logic here
            while (!cancellationToken.IsCancellationRequested)
            {
                // Simulate some work
                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the OPC UA Subscriber Service.");
        }
    }

    private async Task MonitorAsync(ManagedSession session, string name, NodeId nodeId, CancellationToken ct)
    {
        await foreach (DataValueChange change in session.DefaultStreaming
            .SubscribeDataChangesAsync(nodeId, ct: ct)
            .WithCancellation(ct))
        {
            if (StatusCode.IsBad(change.Value.StatusCode))
            {
                logger.LogWarning("{Name}: bad status {Status}", name, change.Value.StatusCode);
                continue;
            }
            logger.LogInformation("{Name}: {Value} ({Status})",
                name, change.Value.WrappedValue, change.Value.StatusCode);
        }
    }
}