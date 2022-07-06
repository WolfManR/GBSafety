using ConsoleClient.ServiceProviders;

namespace ConsoleClient.LoadBalancers;

public class RandomLoadBalancer : ILoadBalancer
{
    private readonly IServiceDiscoveryProvider _sdProvider;
    private readonly Random _random = new Random();

    public RandomLoadBalancer(IServiceDiscoveryProvider sdProvider)
    {
        _sdProvider = sdProvider;
    }

    public async Task<string> GetServiceAsync()
    {
        var services = await _sdProvider.GetServicesAsync();
        return services[_random.Next(services.Count)];
    }
}