namespace HrManagerHttpService.Models;

public readonly record struct EmployeeWithDesiredEmployees(int Id, string Name, IList<int> DesiredEmployees);
