namespace OpcUaRefresher.Models
{
    public sealed class BufferTank(ISensor liquidLevelSensor) : IBufferTank
    {
        public ISensor LiquidLevelSensor { get; private set; } = liquidLevelSensor;
    }
}