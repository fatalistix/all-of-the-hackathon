using EmployeeRabbitService.Services;
using MassTransit;
using RabbitCommon.Messages;

namespace EmployeeRabbitService.Consumers;

public class HackathonStartConsumer(EmployeeService employeeService) : IConsumer<HackathonStartMessage>
{
    public Task Consume(ConsumeContext<HackathonStartMessage> context)
    {
        employeeService.DoWork(context.Message.HackathonId);
        return Task.CompletedTask;
    }
}