using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Database.Entity;
using AllOfTheHackathon.Service.Transient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Testcontainers.PostgreSql;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class AverageSatisfactionCalculatorTest
{

    [TestMethod]
    public void GivenTwoHackathons_WhenCalculate_ThenOk()
    {
        var postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16.2")
            .WithPortBinding(5435, 5432)
            .WithEnvironment(new Dictionary<string, string>
            {
                    { "POSTGRES_USER", "all-of-the-hackathon-owner-test" },
                    { "POSTGRES_PASSWORD", "all-of-the-hackathon-password-test" },
                    { "POSTGRES_DB", "all-of-the-hackathon-test" }
                }
            ).Build();
        postgres.StartAsync().Wait();

        const string connectionString = "Host=localhost;" + 
                                        "Port=5435;" + 
                                        "Database=all-of-the-hackathon-test;" + 
                                        "Username=all-of-the-hackathon-owner-test;" + 
                                        "Password=all-of-the-hackathon-password-test";
        
        using var keepAliveConnection = new NpgsqlConnection(connectionString);
        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseNpgsql(connectionString).Options;

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
        // postgres.StopAsync().Wait();
    }
}