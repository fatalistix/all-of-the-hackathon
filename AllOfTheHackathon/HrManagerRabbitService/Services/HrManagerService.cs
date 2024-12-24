using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using HrManagerRabbitService.Clients;
using HrManagerRabbitService.Models;
using MassTransit;
using Microsoft.Extensions.Hosting;
using RabbitCommon.Messages;
using RabbitCommon.Requests;

namespace HrManagerRabbitService.Services;

public class HrManagerService(HrManager hrManager, 
    ITeamSender teamSender,
    IPublishEndpoint publishEndpoint)
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

        var request = new TeamRequest(teams);
        var message = new WishlistsMessage(teamLeadsWishlists, juniorsWishlists);

        teamSender.SendTeams(request).Wait();
        publishEndpoint.Publish(message).Wait();
        
        Console.WriteLine("SENT MESSAGE");
    }
}