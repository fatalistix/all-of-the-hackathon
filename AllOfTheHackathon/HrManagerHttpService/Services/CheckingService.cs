using AutoMapper;
using HrManagerHttpService.Databases.Contexts;
using HrManagerHttpService.Models;
using Microsoft.EntityFrameworkCore;

namespace HrManagerHttpService.Services;

public class CheckingService(
    IConfiguration configuration,
    HrManagerContext context,
    HrManagerService service,
    IMapper mapper)
    : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var delay = configuration.GetValue<int>("CheckingDelay");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(delay));
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = configuration.GetValue<int>("ParticipantsCount");
        context.Juniors.Load();
        context.TeamLeads.Load();

        if (context.Juniors.Count() != count || context.TeamLeads.Count() != count)
        {
            return;
        }

        _timer?.Change(Timeout.Infinite, 0);
        var teamLeads = context.TeamLeads.Select(mapper.Map<EmployeeWithDesiredEmployees>).ToList();
        var juniors = context.Juniors.Select(mapper.Map<EmployeeWithDesiredEmployees>).ToList();
        service.DoWork(teamLeads, juniors);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}