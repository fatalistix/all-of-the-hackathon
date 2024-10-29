using AllOfTheHackathon.Repository;
using AllOfTheHackathon.Service.Transient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AllOfTheHackathon.Service;

public class HackathonWorker(EmployeeCsvRepository repository, 
    Hackathon hackathon, 
    HrManager hrManager, 
    HrDirector hrDirector, 
    IHostApplicationLifetime applicationLifetime, 
    IConfiguration configuration) : IHostedService
{
    private readonly int _hackathonTimes = configuration.GetValue<int>("Hackathon:Times");
    private readonly string _teamLeadsResources = configuration.GetValue<string>("Repository:Csv:TeamLeadsFileLocation") 
                                                  ?? throw new InvalidOperationException("Setting \"Repository:Csv:TeamLeadsFileLocation\" must be set");
    private readonly string _juniorsResources = configuration.GetValue<string>("Repository:Csv:JuniorsFileLocation") 
                                                ?? throw new InvalidOperationException("Setting \"Repository:Csv:JuniorsFileLocation\" must be set");
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(Run, cancellationToken);
        return Task.CompletedTask;
    }

    private void Run()
    {
        repository.Load(_teamLeadsResources);
        var teamLeads = repository.Get();
        
        repository.Load(_juniorsResources);
        var juniors = repository.Get();

        var summarySatisfaction = 0.0;
        for (var i = 0; i < _hackathonTimes; ++i)
        {
            var (teamLeadsWishlists, juniorsWishlists) = hackathon.Hold(teamLeads, juniors);
            
            var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
            var globalSatisfaction = hrDirector.Calculate(teams, teamLeadsWishlists, juniorsWishlists);
            summarySatisfaction += globalSatisfaction;
        }
        
        Console.WriteLine($"Средний уровень удовлетворенности по {_hackathonTimes} хакатонам:");
        Console.WriteLine(summarySatisfaction / _hackathonTimes);
        
        applicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}