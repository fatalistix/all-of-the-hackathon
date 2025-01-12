namespace RabbitCommon.Messages;

public record EmployeeMessage(int Id, string Name, string Type, IList<int> DesiredEmployees, Guid HackathonId);