using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using AutoMapper;
using HrManagerHttpService.Clients;
using HrManagerHttpService.Models;
using HttpCommon.Requests;

namespace HrManagerHttpService.Services;

public class HrManagerService(
    HrManager hrManager,
    ITeamsAndWishlistsSender teamsAndWishlistsSender,
    IHostApplicationLifetime applicationLifetime,
    IMapper mapper)
{
    public void DoWork(IList<EmployeeWithDesiredEmployees> teamLeadsWithDesiredJuniorsList,
        IList<EmployeeWithDesiredEmployees> juniorsWithDesiredTeamLeadsList)
    {
        var teamLeads = teamLeadsWithDesiredJuniorsList.Select(e => mapper.Map<Employee>(e));
        var juniors = juniorsWithDesiredTeamLeadsList.Select(e => mapper.Map<Employee>(e));

        var teamLeadsWishlists = teamLeadsWithDesiredJuniorsList
            .Select(e => new Wishlist(e.Id, e.DesiredEmployees.ToArray())).ToList();
        var juniorsWishlists = juniorsWithDesiredTeamLeadsList
            .Select(e => new Wishlist(e.Id, e.DesiredEmployees.ToArray())).ToList();

        var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

        var request = new TeamsAndWishlistsRequest(
            teams, teamLeadsWishlists, juniorsWishlists);

        teamsAndWishlistsSender.SendTeamsAndWishlists(request).Wait();
        Console.WriteLine("SENT MESSAGE");

        applicationLifetime.StopApplication();
    }
}