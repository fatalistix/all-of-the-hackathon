using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Repository;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AllOfTheHackathon.Service;

public class HackathonWorker(EmployeeCsvRepository repository, 
    Hackathon hackathon, 
    HrManager hrManager, 
    HrDirector hrDirector, 
    IHostApplicationLifetime applicationLifetime, 
    IConfiguration configuration,
    IMapper mapper,
    HackathonContext hackathonContext) : IHostedService
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

        var teamLeadsEntities = teamLeads.Select(mapper.Map<TeamLeadEntity>).ToList();
        hackathonContext.TeamLeads.AddRange(teamLeadsEntities);
        hackathonContext.SaveChanges();
        
        repository.Load(_juniorsResources);
        var juniors = repository.Get();

        var juniorsEntities = juniors.Select(mapper.Map<JuniorEntity>).ToList();
        hackathonContext.Juniors.AddRange(juniorsEntities);
        hackathonContext.SaveChanges();

        var summarySatisfaction = 0.0;
        for (var i = 0; i < _hackathonTimes; ++i)
        {
            var (teamLeadsWishlists, juniorsWishlists) = hackathon.Hold(teamLeads, juniors);

            var teamLeadWishlistsEntities = teamLeadsWishlists.Select(mapper.Map<TeamLeadWishlistEntity>).ToList();
            hackathonContext.TeamLeadWishlists.AddRange(teamLeadWishlistsEntities);
            var juniorsWishlistsEntities = juniorsWishlists.Select(mapper.Map<JuniorWishlistEntity>).ToList();
            hackathonContext.JuniorWishlists.AddRange(juniorsWishlistsEntities);
            
            var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

            var teamsEntities = teams.Select(mapper.Map<TeamEntity>).ToList();
            foreach (var team in teamsEntities)
            {
                foreach (var juniorEntity in juniorsEntities.Where(juniorEntity => team.Junior.Id == juniorEntity.Id))
                {
                    team.Junior = juniorEntity;
                }

                foreach (var teamLeadEntity in teamLeadsEntities.Where(teamLeadEntity => team.TeamLead.Id == teamLeadEntity.Id))
                {
                    team.TeamLead = teamLeadEntity;
                }
            }
            hackathonContext.Teams.AddRange(teamsEntities); 
            
            var globalSatisfaction = hrDirector.Calculate(teams, teamLeadsWishlists, juniorsWishlists);
            summarySatisfaction += globalSatisfaction;
            
            var id = Guid.NewGuid();
            var hackathonEntity = new HackathonEntity
            {
                Id = id,
                TeamLeads = teamLeadsEntities,
                Juniors = juniorsEntities,
                TeamLeadWishlists = teamLeadWishlistsEntities,
                JuniorWishlists = juniorsWishlistsEntities,
                Teams = teamsEntities,
                AverageSatisfaction = globalSatisfaction
            };
            
            hackathonContext.Hackathons.Add(hackathonEntity);
        }
        
        Console.WriteLine($"Средний уровень удовлетворенности по {_hackathonTimes} хакатонам:");
        Console.WriteLine(summarySatisfaction / _hackathonTimes);
        hackathonContext.SaveChanges();
        
        applicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}