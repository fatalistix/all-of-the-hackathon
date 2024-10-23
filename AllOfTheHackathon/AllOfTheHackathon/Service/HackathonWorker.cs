using AllOfTheHackathon.Repository;
using AllOfTheHackathon.Service.Transient;
using Microsoft.Extensions.Hosting;

namespace AllOfTheHackathon.Service;

public class HackathonWorker(EmployeeCsvRepository repository, 
    Hackathon hackathon, 
    HrManager hrManager, 
    HrDirector hrDirector, 
    IHostApplicationLifetime applicationLifetime) : IHostedService
{
    private const int HackathonTimes = 1_000;
    private const string TeamLeadsResources = "AllOfTheHackathon.Resources.Teamleads20.csv";
    private const string JuniorsResources = "AllOfTheHackathon.Resources.Juniors20.csv";
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(Run, cancellationToken);
        return Task.CompletedTask;
    }

    private void Run()
    {
        repository.Load(TeamLeadsResources);
        var teamLeads = repository.Get();
        
        repository.Load(JuniorsResources);
        var juniors = repository.Get();

        var summarySatisfaction = 0.0;
        for (var i = 0; i < HackathonTimes; ++i)
        {
            var (teamLeadsWishlists, juniorsWishlists) = hackathon.Hold(teamLeads, juniors);
            
            var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
            var globalSatisfaction = hrDirector.Calculate(teams, teamLeadsWishlists, juniorsWishlists);
            summarySatisfaction += globalSatisfaction;
        }
        
        Console.WriteLine($"Средний уровень удовлетворенности по {HackathonTimes} хакатонам:");
        Console.WriteLine(summarySatisfaction / HackathonTimes);
        
        applicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}