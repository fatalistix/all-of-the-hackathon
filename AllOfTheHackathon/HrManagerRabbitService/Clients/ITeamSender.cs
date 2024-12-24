using RabbitCommon.Requests;
using Refit;

namespace HrManagerRabbitService.Clients;

public interface ITeamSender
{
    [Post("/")]
    Task SendTeams(TeamRequest teamRequest);
}