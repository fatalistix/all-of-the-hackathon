using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;
using HrManagerRabbitService.Clients;
using HrManagerRabbitService.Models;
using MassTransit;
using Microsoft.Extensions.Hosting;
using RabbitCommon.Messages;
using RabbitCommon.Requests;

namespace HrManagerRabbitService.Services;

public class HrManagerService(HrManager hrManager, 
    ITeamSender teamSender,
    IPublishEndpoint publishEndpoint,
    IMapper mapper)
{
    public void DoWork(IList<EmployeeWithDesiredEmployees> teamLeadsWithDesiredJuniorsList,
        IList<EmployeeWithDesiredEmployees> juniorsWithDesiredTeamLeadsList, Guid hackathonId)
    {
        var teamLeads = teamLeadsWithDesiredJuniorsList.Select(e => mapper.Map<Employee>(e));
        var juniors = juniorsWithDesiredTeamLeadsList.Select(e => mapper.Map<Employee>(e));
        
        var teamLeadsWishlists = teamLeadsWithDesiredJuniorsList
            .Select(e => new Wishlist(e.Id, e.DesiredEmployees.ToArray())).ToList();
        var juniorsWishlists = juniorsWithDesiredTeamLeadsList
            .Select(e => new Wishlist(e.Id, e.DesiredEmployees.ToArray())).ToList();
        
        var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

        var request = new TeamRequest(teams, hackathonId);
        var message = new WishlistsMessage(teamLeadsWishlists, juniorsWishlists, hackathonId);

        teamSender.SendTeams(request).Wait();
        publishEndpoint.Publish(message).Wait();
        
        Console.WriteLine("SENT MESSAGE");
    }
}