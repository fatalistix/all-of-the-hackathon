using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Service.Transient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class AverageSatisfactionCalculatorTest
{

    [TestMethod]
    public void GivenTwoHackathons_WhenCalculate_ThenOk()
    {
        const string connectionString = "DataSource=myshareddb;mode=memory;cache=shared";
        var keepAliveConnection = new SqliteConnection(connectionString);
        keepAliveConnection.Open();
        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseSqlite(connectionString).Options;

        var hackathonContext = new HackathonContext(options);

        hackathonContext.Hackathons.AddRange(new HackathonEntity
        {
            Id = Guid.NewGuid(),
            AverageSatisfaction = 1.0
        }, new HackathonEntity
        {
            Id = new Guid(),
            AverageSatisfaction = 1.0
        });
        hackathonContext.SaveChanges();

        var calculator = new AverageSatisfactionCalculator(hackathonContext);
        Assert.AreEqual((1.0, 2), calculator.Calculate());
    }
}