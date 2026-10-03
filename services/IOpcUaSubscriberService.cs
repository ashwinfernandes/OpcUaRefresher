using OpcUaRefresher.Models;

namespace OpcUaRefresher.services
{
    public interface IOpcUaSubscriberService
    {
        void RegisterSensor(ISensor sensor);
    }
}