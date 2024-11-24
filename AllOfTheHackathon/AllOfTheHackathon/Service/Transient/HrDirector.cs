using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Service.Transient;

public class HrDirector(ICalculator calculator)
{
    public double Calculate(in IList<Team> teams, IList<Wishlist> teamLeadsWishlists,
        IList<Wishlist> juniorsWishLists)
    {
        var teamLeadsDesiredJuniors = teamLeadsWishlists.ToDictionary(t => t.EmployeeId, t => t.DesiredEmployees);
        var juniorsDesiredTeamLeads = juniorsWishLists.ToDictionary(j => j.EmployeeId, j => j.DesiredEmployees);

        var values = (from t in teams
            let juniorSatisfaction = CountSatisfaction(t.TeamLead.Id, juniorsDesiredTeamLeads[t.Junior.Id])
            let teamLeadSatisfaction = CountSatisfaction(t.Junior.Id, teamLeadsDesiredJuniors[t.TeamLead.Id])
            select new List<int>([juniorSatisfaction, teamLeadSatisfaction]))
            .SelectMany(x => x)
            .ToList();

        return calculator.Calculate(values);
    }

    private static int CountSatisfaction(int targetId, int[] desiredEmployees)
    {
        return desiredEmployees.Length - Array.FindIndex(desiredEmployees, de => de == targetId);
    }
}