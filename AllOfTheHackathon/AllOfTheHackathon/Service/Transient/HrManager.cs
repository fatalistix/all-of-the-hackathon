using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Service.Transient;

public class HrManager(ITeamBuildingStrategy teamBuildingStrategy)
{
    public IList<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        return teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
    }
}