using HrDirectorHttpService.Services;
using HttpCommon.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HrDirectorHttpService.Controllers;

[ApiController]
[Route("/")]
public class HrDirectorController(HrDirectorService hrDirectorService)
{
    [HttpPost]
    public void SendTeamsAndWishlists([FromBody] TeamsAndWishlistsRequest teamsAndWishlistsRequest)
    {
        hrDirectorService.DoWork(teamsAndWishlistsRequest.Teams, teamsAndWishlistsRequest.TeamLeadsWishlists,
            teamsAndWishlistsRequest.JuniorsWishlists);
    }
}