using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Subscriptions;
using OpcUaRefresher.Models;
using OpcUaRefresher.services;

internal sealed class OpcUaSubscriberService(ILogger<OpcUaSubscriberService> logger, Func<CancellationToken, Task<ManagedSession>> connect) : BackgroundService, IOpcUaSubscriberService
{
    private ManagedSession? session;

    private List<ISensor> registeredSensors = new();

    private readonly Dictionary<string, NodeId> sensorNodeIds = new(StringComparer.OrdinalIgnoreCase)
    {
        ["liquid-level"] = new NodeId(1003, 3)
    };

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OPC UA Subscriber Service is starting.");
        var delay = TimeSpan.FromSeconds(1);
        var maxDelay = TimeSpan.FromSeconds(30);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                this.session = await connect(cancellationToken);
                logger.LogInformation("Connected to OPC UA server: {Endpoint}", this.session.Endpoint.EndpointUrl);

                await Task.WhenAll(registeredSensors.Select(sensor => MonitorAsync(sensor, cancellationToken)));
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("OPC UA Subscriber Service is stopping.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OPC UA session failed; retrying in {Delay} seconds", delay.TotalSeconds);
            }

            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("OPC UA Subscriber Service is stopping.");
                break;
            }
            delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
        }
    }

    private async Task MonitorAsync(ManagedSession session, ISensor sensor, NodeId nodeId, CancellationToken ct)
    {
        await foreach (DataValueChange change in session.DefaultStreaming
            .SubscribeDataChangesAsync(nodeId, ct: ct)
            .WithCancellation(ct))
        {
            if (StatusCode.IsBad(change.Value.StatusCode))
            {
                logger.LogWarning("{Name}: bad status {Status}", sensor.Name, change.Value.StatusCode);
                continue;
            }

            if (change.Value.WrappedValue.TryGetValue(out double value))
            {
                sensor.Value = value;
            }

            logger.LogInformation("{Name}: {Value} ({Status})",
                sensor.Name, value, change.Value.StatusCode);
        }
    }

    private Task MonitorAsync(ISensor sensor, CancellationToken ct)
    {
        if (session == null)
        {
            throw new InvalidOperationException("Session is not established. Cannot monitor sensor.");
        }

        if (!sensorNodeIds.TryGetValue(sensor.Id, out var nodeId))
        {
            throw new ArgumentException($"Sensor '{sensor.Name}' is not registered for monitoring.");
        }

        return MonitorAsync(session, sensor, nodeId, ct);
    }

    public void RegisterSensor(ISensor sensor)
    {
        registeredSensors.Add(sensor);
    }
}