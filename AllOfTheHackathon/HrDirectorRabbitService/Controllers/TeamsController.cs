using HrDirectorRabbitService.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitCommon.Requests;

namespace HrDirectorRabbitService.Controllers;

[ApiController]
[Route("/")]
public class TeamsController(CollectingService collectingService)
{
    [HttpPost]
    public void SendTeams([FromBody] TeamRequest teamRequest)
    {
        collectingService.AddTeams(teamRequest.Teams);
    }
}