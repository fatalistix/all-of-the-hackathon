using HrManagerHttpService.Models;
using HrManagerHttpService.Services;
using HttpCommon.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HrManagerHttpService.Controllers;

[ApiController]
[Route("/")]
public class HrManagerController(CollectingService service) : Controller
{
    [HttpPost]
    public void SendPreferences([FromBody] EmployeeRequest employeeRequest)
    {
        var employeeType = employeeRequest.Type switch
        {
            "teamlead" => EmployeeType.Teamlead,
            "junior" => EmployeeType.Junior,
            _ => throw new ArgumentException("Unknown employee type")
        };
        
        service.AddInfo(employeeRequest.Id, employeeRequest.Name, employeeRequest.DesiredEmployees, employeeType);
    }
}