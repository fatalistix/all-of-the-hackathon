using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
using AllOfTheHackathon.Service.Transient;
using AllOfTheHackathon.TeamBuildingStrategy;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        
        const string connectionString = "DataSource=myshareddb;mode=memory;cache=shared";
        var keepAliveConnection = new SqliteConnection(connectionString);
        keepAliveConnection.Open();
        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseSqlite(connectionString).Options;
        
        var hackathonContext = new HackathonContext(options);

        // var hackathonEvent = new HackathonEvent();
    }
}