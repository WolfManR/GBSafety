namespace ConsoleClient.LoadBalancers;

public interface ILoadBalancer
{
    Task<string> GetServiceAsync();
}