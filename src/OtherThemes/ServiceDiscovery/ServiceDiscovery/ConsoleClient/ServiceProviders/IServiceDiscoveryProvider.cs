namespace ConsoleClient.ServiceProviders;

public interface IServiceDiscoveryProvider
{
    Task<List<string>> GetServicesAsync();
}