using System.Collections.Concurrent;
using HrManagerHttpService.Models;

namespace HrManagerHttpService.Services;

public class CollectingService(HrManagerService service)
{
    private readonly ConcurrentDictionary<int, EmployeeWithDesiredEmployees> _juniorParticipants = new();
    private readonly ConcurrentDictionary<int, EmployeeWithDesiredEmployees> _teamLeadParticipants = new();
    private readonly Mutex _mutex = new();

    public bool AddInfo(int id, string name, IList<int> desiredEmployees, EmployeeType employeeType)
    {
        if (id is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "id is less then 1 or greater then 5");
        }

        var desiredEmployeesSet = desiredEmployees.ToHashSet();
        if (desiredEmployeesSet.Count != 5)
        {
            throw new ArgumentException("number of desired employees is not 5");
        }

        for (var i = 1; i < 6; ++i)
        {
            desiredEmployeesSet.Remove(i);
        }

        if (desiredEmployeesSet.Count != 0)
        {
            throw new ArgumentException("desired employees contains invalid ids (must be 1 to 5)");
        }
        
        switch (employeeType)
        {
            case EmployeeType.Junior:
                _juniorParticipants[id] = new EmployeeWithDesiredEmployees(id, name, desiredEmployees);
                break;
            case EmployeeType.Teamlead:
                _teamLeadParticipants[id] = new EmployeeWithDesiredEmployees(id, name, desiredEmployees);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(employeeType), employeeType, "unknown employee type");
        }

        var result = false;

        _mutex.WaitOne();
        Console.WriteLine($"{_juniorParticipants.Count} <==> {_teamLeadParticipants.Count}");
        if (_juniorParticipants.Count == 5 && _teamLeadParticipants.Count == 5)
        {
            service.DoWork(_juniorParticipants, _teamLeadParticipants);
            result = true;
        }
        _mutex.ReleaseMutex();

        return result;
    }
}
