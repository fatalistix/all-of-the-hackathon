using HrManagerRabbitService.Services;
using MassTransit;
using RabbitCommon.Messages;

namespace HrManagerRabbitService.Consumers;

public class HackathonStartConsumer(CheckingService service) : IConsumer<HackathonStartMessage>
{
    public Task Consume(ConsumeContext<HackathonStartMessage> context)
    {
        service.HackathonIds[context.Message.HackathonId] = true;
        if (Interlocked.CompareExchange(ref service.IsStarted, 1, 0) == 0)
        {
            service.StartAsync(CancellationToken.None);
        }
        return Task.CompletedTask;
    }
}