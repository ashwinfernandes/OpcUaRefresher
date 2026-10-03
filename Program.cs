// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;


Console.WriteLine("Minimal OPC UA client example");
HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
    options.SetMinimumLevel(LogLevel.Information);
});
builder.Services.AddOpcUa().AddClient(options =>
{
    options.AutoAcceptUntrustedCertificates = true; // Set to false in production for security
    options.RejectSHA1SignedCertificates = true;
    options.MinimumCertificateKeySize = 2048;
    options.ApplicationName = "MinimalOpcUaClient";
    options.ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:MinimalOpcUaClient";
    options.ProductUri = "urn:MinimalOpcUaClient";
    options.PkiRoot = Path.Combine(builder.Environment.ContentRootPath, "pki");
    options.Session = new ManagedSessionOptions
    {
        SessionName = "MinimalOpcUaClientSession",
        SessionTimeout = TimeSpan.FromSeconds(60),
        ReconnectPolicy = new ReconnectPolicyOptions
        {
            MaxRetries = 2
        }
    };
}).AddDiscoveryAndConnect(options =>
{
    options.DiscoveryUrl = "opc.tcp://localhost:53530/OPCUA/SimulationServer"; // Replace with your OPC UA server endpoint
    options.SecurityMode = MessageSecurityMode.None; // Set to true for secure connections
    options.SecurityPolicyUri = SecurityPolicies.None; // Set to true for secure connections
}).AddSubscriptions(options =>
{
    options.PublishingInterval = TimeSpan.FromSeconds(1); // 1 second
    options.LifetimeCount = 100;
    options.KeepAliveCount = 10;
    options.MaxNotificationsPerPublish = 1000;
    options.Priority = 0;
}).AddAlarms();
builder.Services.AddHostedService<OpcUaSubscriberService>();
var host = builder.Build();
await host.RunAsync();
