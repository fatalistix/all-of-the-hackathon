using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AutoMapper;

namespace AllOfTheHackathon.Service.Transient;

public class HackathonEvent(
    Hackathon hackathon,
    HackathonContext hackathonContext,
    IMapper mapper,
    HrManager hrManager,
    HrDirector hrDirector)
{
    public Guid Handle()
    {
        var teamLeadsEntities = hackathonContext.TeamLeads.ToList();
        var juniorsEntities = hackathonContext.Juniors.ToList();
        var teamLeads = teamLeadsEntities.Select(mapper.Map<Employee>).ToList();
        var juniors = juniorsEntities.Select(mapper.Map<Employee>).ToList();
        
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
        return id;
    }
}