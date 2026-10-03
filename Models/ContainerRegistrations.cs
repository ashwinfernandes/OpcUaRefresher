using OpcUaRefresher.services;

namespace OpcUaRefresher.Models
{
    public static class ContainerRegistrations
    {
        public static IServiceCollection RegisterComponents(this IServiceCollection services)
        {
            services.AddTransient<ISensor, LiquidLevel>();
            services.AddSingleton<IBufferTank, BufferTank>();
            services.AddSingleton<IBufferTankService, BufferTankService>();
            services.AddSingleton(provider =>
            {
                var liquidLevelSensor = provider.GetRequiredService<ISensor>();
                return new BufferTank(liquidLevelSensor);
            });
            return services;
        }
    }
}