using Consul;

using Microsoft.AspNetCore.Hosting.Server.Features;

using System.Net;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace ServiceA;

public class ConsulServiceRegistration : BackgroundService
{
    private readonly IConsulClient _client;
    private readonly IServer _server;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private CancellationTokenSource? _cts;
    private string? _serviceId;

    public ConsulServiceRegistration(IConsulClient client, IServer server, IHostApplicationLifetime hostApplicationLifetime)
    {
        _client = client;
        _server = server;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForApplicationStarted();

        await Register(stoppingToken);

        while (!stoppingToken.IsCancellationRequested) { }

        await _client.Agent.ServiceDeregister(_serviceId);
    }

    private Task WaitForApplicationStarted()
    {
        var completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _hostApplicationLifetime.ApplicationStarted.Register(() => completionSource.TrySetResult());
        return completionSource.Task;
    }

    public async Task Register(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var features = _server.Features;
        var address = features.Get<IServerAddressesFeature>()?.Addresses.First();
        if (address is null) throw new InvalidOperationException("Some unhandled problems with services");
        var uri = new Uri(address);
        _serviceId = "Service-v1-" + Dns.GetHostName() + "-" + uri.Authority;
        var registration = new AgentServiceRegistration()
        {
            ID = _serviceId,
            Name = "Service",
            Address = uri.Host,
            Port = uri.Port,
            Tags = new[] { "api" },
            Check = new AgentServiceCheck()
            {
                // Сердцебиение и адрес
                HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/healthz",
                // сверхурочное время
                Timeout = TimeSpan.FromSeconds(2),
                // проверяем интервал
                Interval = TimeSpan.FromSeconds(10)
            }
        };
        // Сначала удалите сервис, чтобы избежать повторной регистрации
        await _client.Agent.ServiceDeregister(registration.ID, _cts.Token);
        await _client.Agent.ServiceRegister(registration, _cts.Token);
    }
}