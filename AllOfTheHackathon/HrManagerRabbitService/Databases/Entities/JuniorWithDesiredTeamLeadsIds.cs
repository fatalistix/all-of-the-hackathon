namespace HrManagerRabbitService.Databases.Entities;

public record JuniorWithDesiredTeamLeadsIds(int Id, string Name, string DesiredTeamLeadsIds, Guid HackathonId);
