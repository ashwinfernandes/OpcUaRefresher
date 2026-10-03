using OpcUaRefresher.Models;

internal class LiquidLevel : ISensor
{
    public string Id { get; } = "liquid-level";
    public string Name { get; } = "Liquid Level";
    public string Description { get; } = "Liquid level sensor";
    public string Unit { get; } = "liters";
    public double Value { get; set; }
}