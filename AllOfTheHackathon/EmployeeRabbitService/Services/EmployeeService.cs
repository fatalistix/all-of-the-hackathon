using AllOfTheHackathon.Internal.Extension;
using AllOfTheHackathon.Repository;
using EmployeeRabbitService.Configs;
using MassTransit;
using RabbitCommon.Messages;

namespace EmployeeRabbitService.Services;

public class EmployeeService(
    EmployeeConfig employeeConfig,
    IPublishEndpoint publishEndpoint,
    IConfiguration configuration,
    IEmployeeCsvRepository employeeCsvRepository,
    ILogger<EmployeeService> logger)
{
    public void DoWork(Guid hackathonId)
    {
        var juniorsCsvFile = configuration["JuniorsCsvFile"];
        var teamLeadCsvFile = configuration["TeamLeadsCsvFile"];
        if (string.IsNullOrEmpty(juniorsCsvFile) || string.IsNullOrEmpty(teamLeadCsvFile))
        {
            throw new ApplicationException("JuniorsCsvFile and TeamLeadsCsvFile must be specified.");
        }

        var currentCsvFile = employeeConfig.Type == "junior" ? juniorsCsvFile : teamLeadCsvFile;
        var desiredCsvFile = employeeConfig.Type == "junior" ? teamLeadCsvFile : juniorsCsvFile;

        var currents = employeeCsvRepository.LoadFromExecutingDirectory(currentCsvFile);
        currents = currents.Where(j => j.Id == employeeConfig.Id).ToList();
        switch (currents.Count)
        {
            case > 1:
                throw new InvalidOperationException(
                    $"There are multiple employees with id {employeeConfig.Id} and type {employeeConfig.Type}.");
            case 0:
                throw new InvalidOperationException(
                    $"There are no employees with id {employeeConfig.Id} and type {employeeConfig.Type}.");
        }

        var current = currents[0];

        var desired = employeeCsvRepository.LoadFromExecutingDirectory(desiredCsvFile);
        var desiredIds = desired.Select(d => d.Id).ToList();
        desiredIds.Shuffle();

        logger.LogInformation("Employee with id {} and name {} is sending preferences.", current.Id, current.Name);

        var message = new EmployeeMessage(
            employeeConfig.Id,
            current.Name,
            employeeConfig.Type,
            desiredIds,
            hackathonId);

        try
        {
            publishEndpoint.Publish(message).Wait();
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send preferences: {}", ex);
        }
    }
}