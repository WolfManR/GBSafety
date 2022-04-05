using ConsoleClient.LoadBalancers;
using ConsoleClient.ServiceProviders;

var client = new HttpClient();
ILoadBalancer balancer = new RoundRobinLoadBalancer(new PollingConsulServiceProvider());
// Вызов с использованием алгоритма опроса
for (int i = 0; i < 10; i++)
{
    var service = await balancer.GetServiceAsync();
    Console.WriteLine(DateTime.Now + "-RoundRobin:" +
                      await client.GetStringAsync("http://" + service + "/api/values") + " --> Request from " + service);
}
// Используем случайный алгоритм вызова
balancer = new RandomLoadBalancer(new PollingConsulServiceProvider());
for (int i = 0; i < 10; i++)
{
    var service = await balancer.GetServiceAsync();
    Console.WriteLine(DateTime.Now + "-Random:" +
                      await client.GetStringAsync("http://" + service + "/api/values") + " --> Request from " + service);
}