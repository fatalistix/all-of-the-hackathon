using AutoMapper;
using HrManagerRabbitService.Databases.Contexts;
using HrManagerRabbitService.Models;
using Microsoft.EntityFrameworkCore;

namespace HrManagerRabbitService.Services;

public class CheckingService(
    IConfiguration configuration,
    HrManagerContext context,
    HrManagerService service,
    IMapper mapper)
{
    private Timer? _timer;

    public void Start(Guid hackathonId)
    {
        var delay = configuration.GetValue<int>("CheckingDelay");
        _timer = new Timer(DoWork, hackathonId, TimeSpan.Zero, TimeSpan.FromSeconds(delay));
    }

    private void DoWork(object? state)
    {
        if (state is not Guid hackathonId)
        {
            throw new InvalidOperationException($"Expected hackathon's id as state, but received {state?.GetType()}");
        }

        var count = configuration.GetValue<int>("ParticipantsCount");
        context.Juniors.Load();
        context.TeamLeads.Load();

        var juniorsEntities = context.Juniors.Where(j => j.HackathonId.Equals(hackathonId)).ToList();
        var teamLeadsEntities = context.Juniors.Where(j => j.HackathonId.Equals(hackathonId)).ToList();

        if (juniorsEntities.Count != count || teamLeadsEntities.Count != count)
        {
            return;
        }

        Stop();
        var teamLeads = teamLeadsEntities.Select(mapper.Map<EmployeeWithDesiredEmployees>).ToList();
        var juniors = juniorsEntities.Select(mapper.Map<EmployeeWithDesiredEmployees>).ToList();
        service.DoWork(teamLeads, juniors, hackathonId);
    }

    private void Stop()
    {
        _timer?.Change(Timeout.Infinite, 0);
    }
}