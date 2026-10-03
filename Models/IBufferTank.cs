namespace OpcUaRefresher.Models
{
    public interface IBufferTank
    {
        ISensor LiquidLevelSensor { get; }
    }
}