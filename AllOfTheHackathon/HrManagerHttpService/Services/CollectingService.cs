using System.Text.Json;
using HrManagerHttpService.Databases.Contexts;
using HrManagerHttpService.Databases.Entities;
using HrManagerHttpService.Models;

namespace HrManagerHttpService.Services;

public class CollectingService(IConfiguration configuration, HrManagerContext context)
{
    public void AddInfo(int id, string name, IList<int> desiredEmployees, EmployeeType employeeType)
    {
        var participantsCount = configuration.GetValue<int>("ParticipantsCount");
        if (id < 1 || id > participantsCount)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, $"id is less then 1 or greater then {participantsCount}");
        }

        var desiredEmployeesSet = desiredEmployees.ToHashSet();
        if (desiredEmployeesSet.Count != participantsCount)
        {
            throw new ArgumentException($"number of desired employees is not {participantsCount}");
        }

        for (var i = 1; i <= participantsCount; ++i)
        {
            desiredEmployeesSet.Remove(i);
        }

        if (desiredEmployeesSet.Count != 0)
        {
            throw new ArgumentException($"desired employees contains invalid ids (must be 1 to {participantsCount})");
        }

        switch (employeeType)
        {
            case EmployeeType.Junior:
                AddJunior(id, name, desiredEmployees);
                break;
            case EmployeeType.Teamlead:
                AddTeamLead(id, name, desiredEmployees);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(employeeType), employeeType, "unknown employee type");
        }
        
        Console.WriteLine($"Wrote info about {id} and {name} with type {employeeType}");
        context.SaveChanges();
    }

    private void AddJunior(int id, string name, IList<int> desiredEmployees)
    {
        var junior = new JuniorWithDesiredTeamLeadsIds(id, name, JsonSerializer.Serialize(desiredEmployees));
        context.Juniors.Add(junior);
    }

    private void AddTeamLead(int id, string name, IList<int> desiredEmployees)
    {
        var teamLead = new TeamLeadWithDesiredJuniorsIds(id, name, JsonSerializer.Serialize(desiredEmployees));
        context.TeamLeads.Add(teamLead);
    }
}