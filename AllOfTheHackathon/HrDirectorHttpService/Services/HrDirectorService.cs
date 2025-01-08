using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;

namespace HrDirectorHttpService.Services;

public class HrDirectorService(HrDirector hrDirector,
    HackathonContext hackathonContext,
    IMapper mapper,
    IHostApplicationLifetime hostApplicationLifetime)
{
    public void DoWork(IList<Team> teams, IList<Wishlist> teamLeadsWishlists, IList<Wishlist> juniorsWishlists)
    {
        var result = hrDirector.Calculate(teams, teamLeadsWishlists, juniorsWishlists);

        IList<TeamLeadEntity> teamLeadsEntities = teams.Select(t => mapper.Map<TeamLeadEntity>(t.TeamLead)).ToList();
        IList<JuniorEntity> juniorsEntities = teams.Select(t => mapper.Map<JuniorEntity>(t.Junior)).ToList();
        teamLeadsEntities = PersistTeamLeads(teamLeadsEntities);
        juniorsEntities = PersistJuniors(juniorsEntities);
        
        var teamLeadWishlistsEntities = teamLeadsWishlists.Select(mapper.Map<TeamLeadWishlistEntity>).ToList();
        var juniorWishlistsEntities = juniorsWishlists.Select(mapper.Map<JuniorWishlistEntity>).ToList();
        hackathonContext.TeamLeadWishlists.AddRange(teamLeadWishlistsEntities);
        hackathonContext.JuniorWishlists.AddRange(juniorWishlistsEntities);

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

        var id = Guid.NewGuid();
        var hackathonEntity = new HackathonEntity
        {
            Id = id,
            TeamLeads = teamLeadsEntities,
            Juniors = juniorsEntities,
            TeamLeadWishlists = teamLeadWishlistsEntities,
            JuniorWishlists = juniorWishlistsEntities,
            Teams = teamsEntities,
            AverageSatisfaction = result
        };

        hackathonContext.Hackathons.Add(hackathonEntity);
        Console.WriteLine($"Уровень гармоничности: {result}");
        Console.WriteLine($"Идентификатор хакатона: {id}");
        hackathonContext.SaveChanges();
        hostApplicationLifetime.StopApplication();
    }

    private IList<TeamLeadEntity> PersistTeamLeads(IList<TeamLeadEntity> teamLeadEntities)
    {
        foreach (var teamLeadEntity in teamLeadEntities)
        {
            var existing = hackathonContext.TeamLeads.Find(teamLeadEntity.Id);
            if (existing == null)
            {
                hackathonContext.Add(teamLeadEntity);
            }
            else
            {
                hackathonContext.TeamLeads.Update(teamLeadEntity);
            }
        }

        return teamLeadEntities;
    }
    
    private IList<JuniorEntity> PersistJuniors(IList<JuniorEntity> juniorEntities)
    {
         foreach (var juniorEntity in juniorEntities)
         {
             var existing = hackathonContext.Juniors.Find(juniorEntity.Id);
             if (existing == null)
             {
                 hackathonContext.Add(juniorEntity);
             }
             else
             {
                 hackathonContext.Juniors.Update(juniorEntity);
             }
         }
 
         return juniorEntities;       
    }
}