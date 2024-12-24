using MassTransit;
using Microsoft.Extensions.Hosting;
using RabbitCommon.Messages;

namespace HrDirectorRabbitService.Services;

public class HrDirectorHostedService(StatusService statusService, 
    HrDirectorService hrDirectorService,
    IPublishEndpoint publishEndpoint,
    IHostApplicationLifetime hostApplicationLifetime) : IHostedService
{
    private int _hackathonTimes;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hackathonTimes = statusService.HackathonTimes;
        Handle();
        return Task.CompletedTask;
    }

    private void Handle()
    {
        hrDirectorService.AddEndOfWorkListener(HandleEndOfWork);
        publishEndpoint.Publish(new HackathonStartMessage()).Wait();
        Console.WriteLine("DIRECTOR STARTED");
    }

    private void HandleEndOfWork()
    {
        Console.WriteLine("END OF WORK");
        --_hackathonTimes;
        if (_hackathonTimes == 0)
        {
            hostApplicationLifetime.StopApplication();
            return;
        }

        publishEndpoint.Publish(new HackathonStartMessage()).Wait();
        Console.WriteLine("DIRECTOR CONTINUED");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}