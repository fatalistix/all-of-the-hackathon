using HrManagerRabbitService.Services;
using MassTransit;
using RabbitCommon.Messages;

namespace HrManagerRabbitService.Consumers;

public class HackathonStartConsumer(CheckingService service) : IConsumer<HackathonStartMessage>
{
    public Task Consume(ConsumeContext<HackathonStartMessage> context)
    {
        service.Start(context.Message.HackathonId); 
        return Task.CompletedTask;
    }
}