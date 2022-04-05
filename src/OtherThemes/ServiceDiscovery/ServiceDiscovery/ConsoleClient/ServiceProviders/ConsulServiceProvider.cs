using Consul;

namespace ConsoleClient.ServiceProviders;

public class ConsulServiceProvider : IServiceDiscoveryProvider
{
    public async Task<List<string>> GetServicesAsync()
    {
        var consulClient = new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri("http://localhost:8500");
        });
        var queryResult = await consulClient.Health.Service("Service", string.Empty, true);
        List<string> result = new();
        foreach (var serviceEntry in queryResult.Response)
        {
            result.Add(serviceEntry.Service.Address + ":" + serviceEntry.Service.Port);
        }
        return result;
    }
}