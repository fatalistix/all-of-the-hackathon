namespace HrManagerRabbitService.Databases.Entities;

public record TeamLeadWithDesiredJuniorsIds(int Id, string Name, string DesiredJuniorsIds, Guid HackathonId);