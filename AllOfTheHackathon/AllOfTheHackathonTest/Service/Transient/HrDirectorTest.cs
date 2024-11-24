using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class HrDirectorTest
{
    [TestMethod]
    public void GivenTeamsWithMaxDesiredChoices_WhenCalculate_ThenSameNumber()
    {
        var teamLeads = new List<Employee>([
            new Employee(0, "a"),
            new Employee(1, "b"),
            new Employee(2, "c")
        ]);

        var juniors = new List<Employee>([
            new Employee(0, "a"),
            new Employee(1, "b"),
            new Employee(2, "c")
        ]);

        var teamLeadsWishlists = new List<Wishlist>([
            new Wishlist(0, [0, 1, 2]),
            new Wishlist(1, [1, 2, 0]),
            new Wishlist(2, [2, 1, 0])
        ]);

        var juniorsWishlists = new List<Wishlist>([
            new Wishlist(0, [0, 1, 2]),
            new Wishlist(1, [1, 2, 0]),
            new Wishlist(2, [2, 1, 0])
        ]);

        var teams = new List<Team>([
            new Team(teamLeads[0], juniors[0]),
            new Team(teamLeads[1], juniors[1]),
            new Team(teamLeads[2], juniors[2])
        ]);

        var expected = new List<int>([
            3, 3, 3, 3, 3, 3
        ]);

        var mock = new Mock<ICalculator>();
        mock.Setup(a => a.Calculate(expected)).Returns(1);

        var hrDirector = new HrDirector(mock.Object);
        hrDirector.Calculate(teams, teamLeadsWishlists, juniorsWishlists);
        
        mock.Verify(
            a => a.Calculate(expected),
            Times.Once());
    }
}