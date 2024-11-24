using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class HackathonTest
{
    private readonly Hackathon _hackathon = new();
    
    [TestMethod]
    public void Given5TeamLeadsAnd5Juniors_WhenHold_ThenTwoWishlistsLength5()
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

        var (teamLeadsWishlists, juniorsWishlists) = _hackathon.Hold(teamLeads, juniors);
        Assert.AreEqual(teamLeads.Count, teamLeadsWishlists.Count, "Number of wishlists must be equal with number of team leads");
        Assert.AreEqual(juniors.Count, juniorsWishlists.Count, "Size of wishlist must be equal with number of juniors");

        foreach (var wishlist in juniorsWishlists)
        {
            Assert.AreEqual(teamLeads.Count, wishlist.DesiredEmployees.Length, "Number of desired team leads should be equal to number of teamleads");
            foreach (var teamLead in teamLeads)
            {
                var found = false;
                foreach (var desiredEmployee in wishlist.DesiredEmployees)
                {
                    if (desiredEmployee == teamLead.Id)
                    {
                        found = true;
                    }
                }
                
                Assert.IsTrue(found, "Team lead not found in wishlist");
            }
        }

        foreach (var wishlist in teamLeadsWishlists)
        {
            Assert.AreEqual(juniors.Count, wishlist.DesiredEmployees.Length, "Number of desired juniors should be equal to number of juniors");
            foreach (var junior in juniors)
            {
                var found = false;
                foreach (var desiredEmployee in wishlist.DesiredEmployees)
                {
                    if (desiredEmployee == junior.Id)
                    {
                        found = true;
                    } 
                }
                
                Assert.IsTrue(found, "Junior not found in wishlist");
            }
        }
    }

    [TestMethod]
    public void Given3TeamLeadsAnd2Junior_WhenHold_ThenArgumentException()
    {
        var teamLeads = new List<Employee>([
            new Employee(0, "Andrey"),
            new Employee(1, "Gabriel"),  
            new Employee(2, "William")
        ]);

        var juniors = new List<Employee>([
            new Employee(0, "Dmitry"),
            new Employee(1, "Artem")
        ]);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            _hackathon.Hold(teamLeads, juniors);
        });
    }
}