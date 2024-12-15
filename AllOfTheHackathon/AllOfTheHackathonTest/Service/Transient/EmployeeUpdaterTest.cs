using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
using AllOfTheHackathon.Repository;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;

namespace AllOfTheHackathonTest.Service.Transient;

[TestClass]
public class EmployeeUpdaterTest
{
    
    [TestMethod]
    public void GivenNothing_WhenUpdateEmployees_ThenOk()
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
        
        var configurationValues = new Dictionary<string, string>
        {
            { "Repository:Csv:TeamLeadsFileLocation", "AllOfTheHackathonTest.Resources.Teamleads2.csv" },
            { "Repository:Csv:JuniorsFileLocation", "AllOfTheHackathonTest.Resources.Juniors2.csv" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues!)
            .Build();

        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JuniorProfile());
            mc.AddProfile(new TeamLeadProfile());
        });
        var mapper = mappingConfig.CreateMapper();

        var employeeCsvRepositoryMock = new Mock<IEmployeeCsvRepository>();
        employeeCsvRepositoryMock.Setup(m => m.Get())
            .Returns(teamLeads)
            .Callback(
                () => employeeCsvRepositoryMock.Setup(m => m.Get())
                    .Returns(juniors));
        
        var postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16.2")
            .WithPortBinding(5436, 5432)
            .WithEnvironment(new Dictionary<string, string>
                {
                    { "POSTGRES_USER", "all-of-the-hackathon-owner-test" },
                    { "POSTGRES_PASSWORD", "all-of-the-hackathon-password-test" },
                    { "POSTGRES_DB", "all-of-the-hackathon-test" }
                }
            ).Build();
        postgres.StartAsync().Wait();

        const string connectionString = "Host=localhost;" + 
                                        "Port=5436;" + 
                                        "Database=all-of-the-hackathon-test;" + 
                                        "Username=all-of-the-hackathon-owner-test;" + 
                                        "Password=all-of-the-hackathon-password-test";
        
        using var keepAliveConnection = new NpgsqlConnection(connectionString);
        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseNpgsql(connectionString).Options;

        var hackathonContext = new HackathonContext(options);

        var employeeUpdater = new EmployeeUpdater(
            configuration,
            employeeCsvRepositoryMock.Object,
            mapper,
            hackathonContext);
        
        employeeUpdater.UpdateEmployees();
        
        foreach (var juniorEntity in hackathonContext.Juniors)
        {
            var foundJunior = juniors.Find(j => j.Id == juniorEntity.Id && j.Name == juniorEntity.Name);
            Assert.IsNotNull(foundJunior);
        }

        foreach (var teamLeadEntity in hackathonContext.TeamLeads)
        {
            var foundTeamLead = teamLeads.Find(t => t.Id == teamLeadEntity.Id && t.Name == teamLeadEntity.Name);
            Assert.IsNotNull(foundTeamLead);
        }
        
        employeeCsvRepositoryMock.Verify(x => x.LoadFromAssembly("AllOfTheHackathonTest.Resources.Teamleads2.csv"), Times.Once());
        employeeCsvRepositoryMock.Verify(x => x.LoadFromAssembly("AllOfTheHackathonTest.Resources.Juniors2.csv"), Times.Once());
        employeeCsvRepositoryMock.Verify(x => x.Get(), Times.Exactly(2));
    }
}