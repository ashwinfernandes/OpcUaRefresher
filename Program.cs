// See https://aka.ms/new-console-template for more information
using Opc.Ua;
using Opc.Ua.Client;
using OpcUaRefresher.Models;
using OpcUaRefresher.services;


Console.WriteLine("Minimal OPC UA client example");
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");
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
            Strategy = BackoffStrategy.Exponential
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
builder.Services.AddSingleton<OpcUaSubscriberService>();
builder.Services.AddSingleton<IOpcUaSubscriberService>(provider => provider.GetRequiredService<OpcUaSubscriberService>());
builder.Services.AddControllers();
builder.Services.AddHostedService(provider => provider.GetRequiredService<OpcUaSubscriberService>());
builder.Services.RegisterComponents();
var app = builder.Build();

var bufferTankService = app.Services.GetRequiredService<IBufferTankService>();
bufferTankService.RegisterSensors();

app.MapControllers();
app.MapGet("/", () => "Hello World!");

await app.RunAsync();
