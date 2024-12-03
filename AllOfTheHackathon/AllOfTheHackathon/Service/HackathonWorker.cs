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
    HackathonEvent hackathonEvent,
    HackathonContext hackathonContext,
    AverageSatisfactionCalculator averageSatisfactionCalculator) : IHostedService
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
                    DoHackathon();
                    break;
                }
                case "2":
                {
                    DoPrintHackathon();
                    break;
                }
                case "3":
                {
                    DoPrintAverageSatisfaction();
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

    private void DoHackathon()
    {
        var id = hackathonEvent.Handle();
        hackathonContext.SaveChanges();
        Console.WriteLine($"Хакатон проведен. Идентификатор: {id}");
        Console.WriteLine();
    }

    private void DoPrintHackathon()
    {
        Console.WriteLine("Введите идентификатор:");
        Console.Write("> ");
        var idStr = Console.ReadLine();
        if (idStr == null)
        {
            Console.WriteLine($"Невалидные данные: {idStr}");
            return;
        }

        Guid id;
        try
        {
            id = Guid.Parse(idStr);
        }
        catch (FormatException e)
        {
            Console.WriteLine($"Input is not recognized: {e.Message}");
            return;
        }

        HackathonEntity hackathon;
        try
        {
            var tempHackathons = hackathonContext.Hackathons
                .Where(h => h.Id == id);
            tempHackathons =  tempHackathons.Include(h => h.Juniors)
                .Include(h => h.TeamLeads)
                .Include(h => h.JuniorWishlists)
                .Include(h => h.TeamLeadWishlists)
                .Include(h => h.Teams)
                .ThenInclude(teamEntity => teamEntity.Junior)
                .Include(h => h.Teams)
                .ThenInclude(teamEntity => teamEntity.TeamLead)
                .AsSplitQuery();
            hackathon = tempHackathons.First();
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"Хакатон не найден: {e.Message}");
            return;
        }
        
        Console.WriteLine($"Хакатон {id}");
        Console.WriteLine();
        Console.WriteLine("Джуны:");
        foreach (var junior in hackathon.Juniors)
        {
            Console.WriteLine($"  - №{junior.Id}\t{junior.Name}");
        }
        Console.WriteLine();
        Console.WriteLine("Тимлиды:");
        foreach (var teamLead in hackathon.TeamLeads)
        {
            Console.WriteLine($"  - №{teamLead.Id}\t{teamLead.Name}");
        }
        Console.WriteLine();
        Console.WriteLine("Сформированные команды (джун - тимлид)");
        foreach (var team in hackathon.Teams)
        {
            Console.WriteLine($"  - {team.Junior.Name}\t-{team.TeamLead.Name}");
        }
        Console.WriteLine();
        Console.WriteLine($"Уровень гармоничности {hackathon.AverageSatisfaction}");
        Console.WriteLine();
        Console.WriteLine();
    }

    private void DoPrintAverageSatisfaction()
    {
        var (average, total) = averageSatisfactionCalculator.Calculate();
        Console.WriteLine($"Средний уровень удовлетворенности по {total} хакатонам равен {average}");
        Console.WriteLine();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}