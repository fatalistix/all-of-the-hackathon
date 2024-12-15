using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using HrManagerHttpService.Clients;
using HrManagerHttpService.Models;
using HrManagerHttpService.Requests;
using Microsoft.Extensions.Hosting;

namespace HrManagerHttpService.Services;

public class HrManagerService(HrManager hrManager, 
    ITeamSender teamSender,
    IHostApplicationLifetime applicationLifetime)
{
    public void DoWork(IDictionary<int, EmployeeWithDesiredEmployees> juniorToDesiredTeamLeads, 
        IDictionary<int, EmployeeWithDesiredEmployees> teamLeadToDesiredJuniors)
    {
        var juniors = juniorToDesiredTeamLeads.Select(e => new Employee(e.Value.Id, e.Value.Name));
        var teamLeads = teamLeadToDesiredJuniors.Select(e => new Employee(e.Value.Id, e.Value.Name));
        
        var juniorsWishlists = juniorToDesiredTeamLeads
            .Select(e => new Wishlist(e.Key, e.Value.DesiredEmployees.ToArray()))
            .ToList();
        var teamLeadsWishlists = teamLeadToDesiredJuniors
            .Select(e => new Wishlist(e.Key, e.Value.DesiredEmployees.ToArray()))
            .ToList();
        
        var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

        var request = new TeamRequest(
            teams, teamLeadsWishlists, juniorsWishlists);

        teamSender.SendTeamsAndWishlists(request).Wait();
        Console.WriteLine("SENT MESSAGE");
        
        applicationLifetime.StopApplication();
    }
}