namespace OpcUaRefresher.Models
{
    public interface ISensor
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        string Unit { get; }
        double Value { get; set; }
    }
}