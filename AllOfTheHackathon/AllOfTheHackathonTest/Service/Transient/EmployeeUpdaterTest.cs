using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
using AllOfTheHackathon.Repository;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
        
        const string connectionString = "DataSource=myshareddb;mode=memory;cache=shared";
        var keepAliveConnection = new SqliteConnection(connectionString);
        keepAliveConnection.Open();
        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseSqlite(connectionString).Options;

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
        
        employeeCsvRepositoryMock.Verify(x => x.Load("AllOfTheHackathonTest.Resources.Teamleads2.csv"), Times.Once());
        employeeCsvRepositoryMock.Verify(x => x.Load("AllOfTheHackathonTest.Resources.Juniors2.csv"), Times.Once());
        employeeCsvRepositoryMock.Verify(x => x.Get(), Times.Exactly(2));
    }
}