using OpcUaRefresher.Models;

namespace OpcUaRefresher.services
{
    public interface IBufferTankService
    {
        ISensor GetLiquidLevelSensor();
        void RegisterSensors();
    }
}