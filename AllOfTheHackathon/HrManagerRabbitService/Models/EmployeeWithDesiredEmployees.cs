namespace HrManagerRabbitService.Models;

public readonly record struct EmployeeWithDesiredEmployees(int Id, string Name, IList<int> DesiredEmployees);
