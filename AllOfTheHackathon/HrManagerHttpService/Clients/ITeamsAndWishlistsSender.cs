using HttpCommon.Requests;
using Refit;

namespace HrManagerHttpService.Clients;

public interface ITeamsAndWishlistsSender
{
    [Post("/")]
    Task SendTeamsAndWishlists(TeamsAndWishlistsRequest request);
}
