using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Service.Transient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AllOfTheHackathon.Service;

public class HackathonWorker(
    IHostApplicationLifetime applicationLifetime, 
    IConfiguration configuration,
    EmployeeUpdater employeeUpdater,
    HackathonScenario hackathonScenario,
    PrintHackathonScenario printHackathonScenario,
    PrintAverageSatisfactionScenario printAverageSatisfactionScenario) : IHostedService
{
    private readonly int _hackathonTimes = configuration.GetValue<int>("Hackathon:Times");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(Run, cancellationToken);
        return Task.CompletedTask;
    }

    private void Run()
    {
        employeeUpdater.UpdateEmployees(); 
        
        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Провести хакатон");
            Console.WriteLine("2 - Распечатать списки участников, сформированную гармоничность и рассчитанную гармоничность");
            Console.WriteLine("3 - Рассчитать среднюю гармоничность по всем хакатонам");
            Console.WriteLine("q - Выйти");
            Console.Write("> ");

            var value = Console.ReadLine();

            switch (value)
            {
                case "1":
                {
                    hackathonScenario.Perform();
                    break;
                }
                case "2":
                {
                    printHackathonScenario.Perform();
                    break;
                }
                case "3":
                {
                    printAverageSatisfactionScenario.Perform();
                    break;
                }
                case "q":
                {
                    applicationLifetime.StopApplication();
                    return;
                }
                default:
                {
                    Console.WriteLine("Неожиданная команда, попробуйте еще раз");
                    Console.Write("> ");
                    break;
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}