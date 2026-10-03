using OpcUaRefresher.Models;

namespace OpcUaRefresher.services
{
    public class BufferTankService(IOpcUaSubscriberService subscriberService, IBufferTank bufferTank) : IBufferTankService
    {
        public ISensor LiquidLevelSensor => bufferTank.LiquidLevelSensor;

        public ISensor GetLiquidLevelSensor() => LiquidLevelSensor;

        public void RegisterSensors()
        {
            subscriberService.RegisterSensor(LiquidLevelSensor);
        }
    }
}