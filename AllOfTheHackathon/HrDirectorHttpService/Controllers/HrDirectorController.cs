using HrDirectorHttpService.Requests;
using HrDirectorHttpService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrDirectorHttpService.Controllers;

[ApiController]
[Route("/")]
public class HrDirectorController(HrDirectorService hrDirectorService)
{
    [HttpPost]
    public void SendTeamsAndWishlists([FromBody] TeamRequest teamRequest)
    {
        hrDirectorService.DoWork(teamRequest.Teams, teamRequest.TeamLeadsWishlists, teamRequest.JuniorsWishlists); 
    }
}