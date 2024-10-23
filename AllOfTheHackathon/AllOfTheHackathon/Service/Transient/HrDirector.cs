using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Service.Transient;

public class HrDirector
{
    public double Calculate(in IList<Team> teams, IList<Wishlist> teamLeadsWishlists,
        IList<Wishlist> juniorsWishLists)
    {
        var teamLeadsDesiredJuniors = teamLeadsWishlists.ToDictionary(t => t.EmployeeId, t => t.DesiredEmployees);
        var juniorsDesiredTeamLeads = juniorsWishLists.ToDictionary(j => j.EmployeeId, j => j.DesiredEmployees);
        
        var n = teams.Count * 2;
        var sum = (from t in teams let juniorSatisfaction = CountSatisfaction(t.TeamLead.Id, juniorsDesiredTeamLeads[t.Junior.Id]) let teamLeadSatisfaction = CountSatisfaction(t.Junior.Id, teamLeadsDesiredJuniors[t.TeamLead.Id]) select 1.0 / juniorSatisfaction + 1.0 / teamLeadSatisfaction).Sum();

        return n / sum;
    }

    private static int CountSatisfaction(int targetId, int[] desiredEmployees)
    {
        return desiredEmployees.Length - Array.FindIndex(desiredEmployees, de => de == targetId);
    }
}