using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AllOfTheHackathon.Service.Transient;

public class EmployeeUpdater(
    IConfiguration configuration,
    IEmployeeCsvRepository repository,
    IMapper mapper,
    HackathonContext hackathonContext)
{
    private readonly string _teamLeadsResources = configuration.GetValue<string>("Repository:Csv:TeamLeadsFileLocation") 
                                                  ?? throw new InvalidOperationException("Setting \"Repository:Csv:TeamLeadsFileLocation\" must be set");
    private readonly string _juniorsResources = configuration.GetValue<string>("Repository:Csv:JuniorsFileLocation") 
                                                ?? throw new InvalidOperationException("Setting \"Repository:Csv:JuniorsFileLocation\" must be set");
    
    public void UpdateEmployees()
    {
        repository.Load(_teamLeadsResources);
        var teamLeads = repository.Get();
        
        var teamLeadsEntities = teamLeads.Select(mapper.Map<TeamLeadEntity>).ToList();
        hackathonContext.TeamLeads.Load();
        var areTeamLeadsSame = hackathonContext.TeamLeads.ToList()
            .Select(
                t => teamLeadsEntities
                    .Find(e => e.Id == t.Id && e.Name == t.Name) != null)
            .Aggregate((value, result) => value & result);
        if (!areTeamLeadsSame)
        {
            hackathonContext.TeamLeads.RemoveRange(hackathonContext.TeamLeads);
            hackathonContext.TeamLeads.AddRange(teamLeadsEntities);
            hackathonContext.SaveChanges();
        }
        
        repository.Load(_juniorsResources);
        var juniors = repository.Get();
        
        var juniorsEntities = juniors.Select(mapper.Map<JuniorEntity>).ToList();
        hackathonContext.Juniors.Load();
        var areJuniorsSame = hackathonContext.Juniors.ToList()
            .Select(
                j => juniorsEntities
                    .Find(e => e.Id == j.Id && e.Name == j.Name) != null)
            .Aggregate((value, result) => value & result);
        if (areJuniorsSame)
        {
            return;
        }
        
        hackathonContext.Juniors.RemoveRange(hackathonContext.Juniors);
        hackathonContext.Juniors.AddRange(juniorsEntities);
        hackathonContext.SaveChanges();
    }
}