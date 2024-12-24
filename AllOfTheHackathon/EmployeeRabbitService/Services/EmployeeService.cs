using AllOfTheHackathon.Internal.Extension;
using MassTransit;
using RabbitCommon.Messages;

namespace EmployeeRabbitService.Services;

public class EmployeeService(StatusService statusService,
    IPublishEndpoint publishEndpoint)
{
    public void Handle()
    {
        var desiredEmployees = new List<int>([1, 2, 3, 4, 5]);
        desiredEmployees.Shuffle();
        Console.WriteLine($"Shuffled: [{string.Join(", ", desiredEmployees.Select(v => v.ToString()))}]");

        var message = new EmployeeMessage(
            statusService.EmployeeId,
            statusService.EmployeeName,
            statusService.EmployeeType,
            desiredEmployees);

        publishEndpoint.Publish(message).Wait();
    }
}