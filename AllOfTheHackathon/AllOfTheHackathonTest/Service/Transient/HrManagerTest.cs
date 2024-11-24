using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class HrManagerTest
{
    [TestMethod]
    public void GivenMockTeamBuildingStrategy_WhenBuildTeams_ThenMockCalledOnce()
    {
        var teamLeads = new List<Employee>([
            new Employee(0, "John"),
            new Employee(1, "Jack")
        ]);

        var juniors = new List<Employee>([
            new Employee(0, "Mike"),
            new Employee(1, "Sam")
        ]);

        var teamleadsWishlists = new List<Wishlist>([
            new Wishlist(0, [0, 1]),
            new Wishlist(0, [0, 1])
        ]);

        var juniorsWishlists = new List<Wishlist>([
            new Wishlist(0, [0, 1]),
            new Wishlist(0, [0, 1])
        ]);

        var teams = new List<Team>([
            new Team(teamLeads[0], juniors[0]),
            new Team(teamLeads[1], juniors[1])
        ]);
        
        var mock = new Mock<ITeamBuildingStrategy>();
        mock.Setup(a => a.BuildTeams(teamLeads, juniors, teamleadsWishlists, juniorsWishlists))
            .Returns(teams);

        var hrManager = new HrManager(mock.Object);
        var result = hrManager.BuildTeams(teamLeads, juniors, teamleadsWishlists, juniorsWishlists);
        Assert.AreEqual(teams.Count, result.Count, "Hr manager returned wrong team");
        for (var i = 0; i < teams.Count; ++i)
        {
            Assert.AreSame(teams[i], result[i], "Manager returns not expected result");
        }
        
        mock.Verify(
            a => a.BuildTeams(
                teamLeads, 
                juniors, 
                teamleadsWishlists, 
                juniorsWishlists), 
            Times.Once);
    }
}