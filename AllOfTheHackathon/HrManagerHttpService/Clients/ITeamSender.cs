using HrManagerHttpService.Requests;
using Refit;

namespace HrManagerHttpService.Clients;

public interface ITeamSender
{
    [Post("/")]
    Task SendTeamsAndWishlists(TeamRequest request);
}
