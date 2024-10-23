using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Internal.Model;

namespace AllOfTheHackathon.TeamBuildingStrategy;

public class GaleShapleyTeamBuildingStrategy : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads,
        IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsList = teamLeads.ToList();
        var juniorsList = juniors.ToList();
        
        var teamLeadsInternal = teamLeadsList.Select(t => new InternalEmployee(t.Id, t.Name)).ToList();
        var juniorsInternal = juniorsList.Select(j => new InternalEmployee(j.Id, j.Name)).ToList();
        var teamLeadsWishlistsInternal = teamLeadsWishlists
            .Select(w => new InternalWishlist(w.EmployeeId, w.DesiredEmployees)).ToList();
        var juniorsWishlistsInternal =
            juniorsWishlists.Select(w => new InternalWishlist(w.EmployeeId, w.DesiredEmployees)).ToList();
        var teamLeadIdToJuniorIdPair = BuildTeamsInternal(teamLeadsInternal, juniorsInternal, teamLeadsWishlistsInternal, juniorsWishlistsInternal);

        return teamLeadIdToJuniorIdPair.Select(e => MakeTeam(e.Value, e.Key, juniorsList, teamLeadsList));
    }

    private static Dictionary<int, int> BuildTeamsInternal(in IList<InternalEmployee> teamLeads,
        in IList<InternalEmployee> juniors,
        in IList<InternalWishlist> teamLeadsWishlists,
        in IList<InternalWishlist> juniorsWishlists)
    {
        Validate(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists); 
        
        var freeJuniorsQueue = new Queue<int>(juniors.Select(j => j.Id));
        
        var teamLeadsDesiredJuniors = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
        var juniorsDesiredTeamLeads = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);

        var teamLeadIdToJuniorIdPair = new Dictionary<int, int>();
        
        while (freeJuniorsQueue.Count > 0)
        {
            var currentJuniorId = freeJuniorsQueue.Dequeue();
            var desiredTeamLeads = juniorsDesiredTeamLeads[currentJuniorId];

            foreach (var dt in desiredTeamLeads)
            {
                if (teamLeadIdToJuniorIdPair.TryGetValue(dt, out var selectedJuniorId))
                {
                    var desiredJuniors = teamLeadsDesiredJuniors[dt];
                    if (!PrefersCurrenMoreThanSelectedOne(currentJuniorId, selectedJuniorId, desiredJuniors))
                    {
                        continue;
                    }
                    teamLeadIdToJuniorIdPair[dt] = currentJuniorId;
                    freeJuniorsQueue.Enqueue(selectedJuniorId);
                    break;
                }

                teamLeadIdToJuniorIdPair[dt] = currentJuniorId;
                break;
            }
        }

        return teamLeadIdToJuniorIdPair;
    }

    private static Team MakeTeam(int juniorId, int teamLeadId, IEnumerable<Employee> juniors,
        IEnumerable<Employee> teamleads)
    {
        var junior = juniors.First(j => j.Id == juniorId);
        var teamLead = teamleads.First(t => t.Id == teamLeadId);

        return new Team(teamLead, junior);
    }

    private static bool PrefersCurrenMoreThanSelectedOne(in int currentJuniorId, in int selectedJuniorId,
        in int[] desiredJuniors)
    {
        foreach (var dj in desiredJuniors)
        {
            if (dj == currentJuniorId)
            {
                return true;
            }

            if (dj == selectedJuniorId)
            {
                return false;
            }
        }

        return false;
    }

    private static void Validate(in IList<InternalEmployee> teamLeads, 
        in IList<InternalEmployee> juniors, 
        in IList<InternalWishlist> teamLeadsWishlists,
        in IList<InternalWishlist> juniorsWishlists)
    {
        var sizes = new HashSet<int> { teamLeads.Count, juniors.Count, teamLeadsWishlists.Count, juniorsWishlists.Count }; 
        if (sizes.Count > 1)
        {
            throw new ArgumentException("Counts of input data must be equal");
        }
    }
}