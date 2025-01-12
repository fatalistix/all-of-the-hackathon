using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;

namespace HrDirectorRabbitService.Services;

public class HrDirectorService(HrDirector hrDirector,
    HackathonContext hackathonContext,
    IMapper mapper)
{
    private readonly List<Action> _endOfWorkListeners = [];
    
    public void DoWork(IList<Team> teams, IList<Wishlist> teamLeadsWishlists, IList<Wishlist> juniorsWishlists, Guid hackathonId)
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

        var hackathonEntity = new HackathonEntity
        {
            Id = hackathonId,
            TeamLeads = teamLeadsEntities,
            Juniors = juniorsEntities,
            TeamLeadWishlists = teamLeadWishlistsEntities,
            JuniorWishlists = juniorWishlistsEntities,
            Teams = teamsEntities,
            AverageSatisfaction = result
        };

        hackathonContext.Hackathons.Add(hackathonEntity);
        Console.WriteLine($"Уровень гармоничности: {result}");
        Console.WriteLine($"Идентификатор хакатона: {hackathonId}");
        hackathonContext.SaveChanges();
        
        
        NotifyEndOfWorkListeners();
    }
    
    private IList<TeamLeadEntity> PersistTeamLeads(IList<TeamLeadEntity> teamLeadEntities)
    {
        for (var i = 0; i < teamLeadEntities.Count; ++i)
        {
            var teamLeadEntity = teamLeadEntities[i];
            var existing = hackathonContext.TeamLeads.Find(teamLeadEntity.Id);
            if (existing == null)
            {
                hackathonContext.Add(teamLeadEntity);
            }
            else
            {
                teamLeadEntities[i] = existing;
                // hackathonContext.TeamLeads.Update(teamLeadEntity);
            }
        }

        hackathonContext.SaveChanges();

        return teamLeadEntities;
    }
    
    private IList<JuniorEntity> PersistJuniors(IList<JuniorEntity> juniorEntities)
    {
         for (var i = 0; i < juniorEntities.Count; ++i)
         {
             var juniorEntity = juniorEntities[i];
             var existing = hackathonContext.Juniors.Find(juniorEntity.Id);
             if (existing == null)
             {
                 hackathonContext.Add(juniorEntity);
             }
             else
             {
                 juniorEntities[i] = existing;
                 // hackathonContext.Juniors.Update(juniorEntity);
             }
         }

         hackathonContext.SaveChanges();
 
         return juniorEntities;       
    }

    private void NotifyEndOfWorkListeners()
    {
        foreach (var endOfWorkListener in _endOfWorkListeners)
        {
            endOfWorkListener.Invoke();
        }
    }

    public void AddEndOfWorkListener(Action a)
    {
        _endOfWorkListeners.Add(a);
    }
}