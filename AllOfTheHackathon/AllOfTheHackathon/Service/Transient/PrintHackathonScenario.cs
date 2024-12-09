using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace AllOfTheHackathon.Service.Transient;

public class PrintHackathonScenario(HackathonContext hackathonContext)
{
    public void Perform()
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
}