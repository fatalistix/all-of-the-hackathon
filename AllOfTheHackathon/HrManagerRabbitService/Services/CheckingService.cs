using System.Collections.Concurrent;
using AutoMapper;
using HrManagerRabbitService.Databases.Contexts;
using HrManagerRabbitService.Models;
using Microsoft.EntityFrameworkCore;

namespace HrManagerRabbitService.Services;

public class CheckingService(
    IConfiguration configuration,
    HrManagerContext context,
    HrManagerService service,
    IMapper mapper) : BackgroundService
{
    public readonly ConcurrentDictionary<Guid, bool> HackathonIds = new();
    public int IsStarted = 0;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = configuration.GetValue<int>("CheckingDelay");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var enumerator = HackathonIds.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var id = enumerator.Current.Key;
                    DoWork(id);
                }
            }
            await Task.Delay(delay * 1000, stoppingToken);
        }
    }
    
    private void DoWork(Guid hackathonId)
    {
        var count = configuration.GetValue<int>("ParticipantsCount");
        context.Juniors.Load();
        context.TeamLeads.Load();

        var juniorsEntities = context.Juniors.Where(j => j.HackathonId.Equals(hackathonId)).ToList();
        var teamLeadsEntities = context.Juniors.Where(j => j.HackathonId.Equals(hackathonId)).ToList();

        if (juniorsEntities.Count != count || teamLeadsEntities.Count != count)
        {
            return;
        }

        var teamLeads = teamLeadsEntities.Select(mapper.Map<EmployeeWithDesiredEmployees>).ToList();
        var juniors = juniorsEntities.Select(mapper.Map<EmployeeWithDesiredEmployees>).ToList();
        service.DoWork(teamLeads, juniors, hackathonId);
        HackathonIds.Remove(hackathonId, out _);
    }
}