using HrManagerRabbitService.Models;
using HrManagerRabbitService.Services;
using MassTransit;
using RabbitCommon.Messages;

namespace HrManagerRabbitService.Consumers;

public class EmployeeConsumer(CollectingService collectingService) : IConsumer<EmployeeMessage>
{
    public Task Consume(ConsumeContext<EmployeeMessage> context)
    {
        var message = context.Message;
        var employeeType = message.Type switch
        {
            "teamlead" => EmployeeType.Teamlead,
            "junior" => EmployeeType.Junior,
            _ => throw new ArgumentException("Unknown employee type")
        };

        collectingService.AddInfo(message.Id, message.Name, message.DesiredEmployees, employeeType,
            message.HackathonId);
        return Task.CompletedTask;
    }
}