using EmployeeRabbitService.Services;
using MassTransit;
using RabbitCommon.Messages;

namespace EmployeeRabbitService.Consumers;

public class HackathonStartConsumer(EmployeeService employeeService) : IConsumer<HackathonStartMessage>
{
    public Task Consume(ConsumeContext<HackathonStartMessage> context)
    {
        employeeService.Handle();
        return Task.CompletedTask;
    }
}