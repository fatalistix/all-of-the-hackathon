using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Internal.Comparer;
using AllOfTheHackathon.TeamBuildingStrategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllOfTheHackathonTest.TeamBuildingStrategy;

[TestClass]
public class GaleShapleyTeamBuildingStrategyTest
{
    private readonly GaleShapleyTeamBuildingStrategy _strategy = new();

    [TestMethod]
    public void Given5TeamLeadsAnd5JuniorsWithWishlists_WhenBuildTeams_ThenTeamWithLength5()
    {
        var teamLeads = new List<Employee>([
            new Employee(0, "Andrey"),
            new Employee(1, "Gabriel"),  
            new Employee(2, "William"),
            new Employee(3, "Victor"),
            new Employee(4, "Hans")
        ]);
        
        var juniors = new List<Employee>([
            new Employee(0, "Dmitry"),
            new Employee(1, "Artem"),
            new Employee(2, "Alexander"),
            new Employee(3, "Ilya"),
            new Employee(4, "Pavel")
        ]);

        var teamLeadsWishlists = new List<Wishlist>([
            new Wishlist(0, [3, 2, 4, 1, 0]),
            new Wishlist(1, [2, 3, 4, 0, 1]),
            new Wishlist(2, [4, 0, 3, 1, 2]),
            new Wishlist(3, [3, 2, 1, 4, 0]),
            new Wishlist(4, [0, 2, 3, 1, 4])
        ]);

        var juniorsWishlists = new List<Wishlist>([
            new Wishlist(0, [0, 1, 2, 3, 4]),
            new Wishlist(1, [3, 4, 0, 1, 2]),
            new Wishlist(2, [3, 1, 2, 0, 4]),
            new Wishlist(3, [4, 2, 0, 1, 3]),
            new Wishlist(4, [2, 3, 0, 4, 1])
        ]);

        var teams = _strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
        Assert.AreEqual(5, teams.Count, "Number of teams must match with number of participants");
        
        var nonBusyTeamLeads = new HashSet<int>(teamLeads.Select(t => t.Id));
        var nonBusyJuniors = new HashSet<int>(juniors.Select(j => j.Id));
        var expectedTeams = new HashSet<Team>([
            new Team(teamLeads[0], juniors[1]),
            new Team(teamLeads[3], juniors[2]),
            new Team(teamLeads[4], juniors[3]),
            new Team(teamLeads[2], juniors[4]),
            new Team(teamLeads[1], juniors[0])
        ], new TeamLeadJuniorEqualityComparer());

        foreach (var team in teams)
        {
            Assert.IsTrue(nonBusyTeamLeads.Remove(team.TeamLead.Id), "Team lead appears more than in one team");
            Assert.IsTrue(nonBusyJuniors.Remove(team.Junior.Id), "Junior appears more than in one team");
            Assert.IsTrue(expectedTeams.Remove(team), "Received not expected team");
        }
        
        Assert.AreEqual(0, nonBusyTeamLeads.Count, "Not all teamleads are in teams");
        Assert.AreEqual(0, nonBusyJuniors.Count, "Not all juniors are in teams");
        Assert.AreEqual(0, expectedTeams.Count, "Expected teams are not empty");
    }
}
