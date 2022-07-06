using ConsoleClient.ServiceProviders;

namespace ConsoleClient.LoadBalancers;

public class RoundRobinLoadBalancer : ILoadBalancer
{
    private readonly IServiceDiscoveryProvider _sdProvider;
    private readonly object _lock = new();
    private int _index;

    public RoundRobinLoadBalancer(IServiceDiscoveryProvider sdProvider)
    {
        _sdProvider = sdProvider;
    }

    public async Task<string> GetServiceAsync()
    {
        var services = await _sdProvider.GetServicesAsync();
        lock (_lock)
        {
            if (_index >= services.Count)
            {
                _index = 0;
            }
            return services[_index++];
        }
    }
}