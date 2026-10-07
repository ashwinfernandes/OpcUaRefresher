namespace OpcUaRefresher.Models
{
    public interface ISensor
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        string Unit { get; }
        double Value { get; }
        Quality Quality { get; }
        DateTime LastUpdated { get; }
        public void Update(double value, Quality quality, DateTime lastUpdated);
    }
}