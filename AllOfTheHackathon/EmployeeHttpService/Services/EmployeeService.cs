using AllOfTheHackathon.Internal.Extension;
using AllOfTheHackathon.Repository;
using EmployeeHttpService.Configs;
using HttpCommon.Requests;

namespace EmployeeHttpService.Services;

public class EmployeeService(
    IHostApplicationLifetime hostApplicationLifetime,
    IEmployeeCsvRepository employeeCsvRepository,
    EmployeeConfig employeeConfig,
    IConfiguration configuration,
    IPreferencesSender client,
    ILogger<EmployeeService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Employee Service is starting.");
        Task.Run(DoWork, cancellationToken);
        return Task.CompletedTask;
    }

    private void DoWork()
    {
        var juniorsCsvFile = configuration["JuniorsCsvFile"];
        var teamLeadCsvFile = configuration["TeamLeadsCsvFile"];
        if (string.IsNullOrEmpty(juniorsCsvFile) || string.IsNullOrEmpty(teamLeadCsvFile))
        {
            throw new ApplicationException("JuniorsCsvFile and TeamLeadCsvFile must be specified.");
        }
        
        var currentCsvFile = employeeConfig.Type == "junior" ? juniorsCsvFile : teamLeadCsvFile;
        var desiredCsvFile = employeeConfig.Type == "junior" ? teamLeadCsvFile : juniorsCsvFile;
        
        var currents = employeeCsvRepository.LoadFromExecutingDirectory(currentCsvFile);
        currents = currents.Where(j => j.Id == employeeConfig.Id).ToList();
        switch (currents.Count)
        {
            case > 1:
                throw new InvalidOperationException($"There are multiple employees with id {employeeConfig.Id} and type {employeeConfig.Type}.");
            case 0:
                throw new InvalidOperationException($"There are no employees with id {employeeConfig.Id} and type {employeeConfig.Type}.");
        }
        
        var current = currents[0];
        
        var desired = employeeCsvRepository.LoadFromExecutingDirectory(desiredCsvFile);
        var desiredIds = desired.Select(d => d.Id).ToList();
        desiredIds.Shuffle();

        logger.LogInformation("Employee with id {} and name {} is sending preferences.", current.Id, current.Name);
        try
        {
            client.SendPreferences(new EmployeeRequest(current.Id, current.Name, desiredIds, employeeConfig.Type))
                .Wait();
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send preferences: {}", ex);
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}