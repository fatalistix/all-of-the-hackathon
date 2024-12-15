using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Testcontainers.PostgreSql;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class HackathonEventTest
{

    [TestMethod]
    public void GivenNothing_WhenHandle_ThenOk()
    {
        var teamLeads = new List<Employee>
        {
            new(1, "Филиппова Ульяна"),
            new(2, "Николаев Григорий")
        };
        
        var juniors = new List<Employee>
        {
            new(1, "Юдин Адам"),
            new(2, "Яшина Яна")
        };
        
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JuniorProfile());
            mc.AddProfile(new TeamLeadProfile());
        });
        var mapper = mappingConfig.CreateMapper();
        

        var postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16.2")
            .WithPortBinding(5437, 5432)
            .WithEnvironment(new Dictionary<string, string>
                {
                    { "POSTGRES_USER", "all-of-the-hackathon-owner-test" },
                    { "POSTGRES_PASSWORD", "all-of-the-hackathon-password-test" },
                    { "POSTGRES_DB", "all-of-the-hackathon-test" }
                }
            ).Build();
        postgres.StartAsync().Wait();

        const string connectionString = "Host=localhost;" + 
                                        "Port=5437;" +
                                        "Database=all-of-the-hackathon-test;" + 
                                        "Username=all-of-the-hackathon-owner-test;" + 
                                        "Password=all-of-the-hackathon-password-test";
        
        using var keepAliveConnection = new NpgsqlConnection(connectionString);
        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseNpgsql(connectionString).Options;

        
        var hackathonContext = new HackathonContext(options);

        // var hackathonEvent = new HackathonEvent();
    }
}