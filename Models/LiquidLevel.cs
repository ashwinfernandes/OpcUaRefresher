using OpcUaRefresher.Models;

internal class LiquidLevel : ISensor
{
    public string Id { get; } = "liquid-level";
    public string Name { get; } = "Liquid Level";
    public string Description { get; } = "Liquid level sensor";
    public string Unit { get; } = "liters";
    public double Value { get; set; }

    public Quality Quality { get; private set; }

    public DateTime LastUpdated { get; private set; }

    public void Update(double value, Quality quality, DateTime lastUpdated)
    {
        this.Value = value;
        this.Quality = quality;
        this.LastUpdated = lastUpdated;
    }
}