using AllOfTheHackathon.Repository;

namespace EmployeeRabbitService.Services;

public class StatusService
{
    public readonly string EmployeeType;
    public readonly int EmployeeId;
    public readonly string EmployeeName;
    
    public StatusService(IEmployeeCsvRepository employeeCsvRepository)
    {
        var employeeType = Environment.GetEnvironmentVariable("EMPLOYEE_TYPE");
        if (employeeType == null)
        {
            Console.Error.WriteLine("No EMPLOYEE_TYPE variable");
            throw new InvalidOperationException("No EMPLOYEE_TYPE variable");
        }
        
        if (employeeType is not ("teamlead" or "junior"))
        {
            Console.Error.WriteLine("Unexpected EMPLOYEE_TYPE");
            throw new InvalidOperationException("Unexpected EMPLOYEE_TYPE");
        }

        EmployeeType = employeeType;
        
        var employeeIdStr = Environment.GetEnvironmentVariable("EMPLOYEE_ID");
        if (employeeIdStr == null)
        {
            Console.Error.WriteLine("No EMPLOYEE_ID variable");
            throw new InvalidOperationException("No EMPLOYEE_ID variable");
        }
        
        var success = int.TryParse(employeeIdStr, out var employeeId);
        if (!success)
        {
            Console.Error.WriteLine("EMPLOYEE_ID does not contain number");
            throw new InvalidOperationException("EMPLOYEE_ID does not contain number");
        }
        
        if (employeeId is < 1 or > 5)
        {
            Console.Error.WriteLine("EMPLOYEE_ID contains invalid number");
            throw new InvalidOperationException("EMPLOYEE_ID contains invalid number");
        }

        EmployeeId = employeeId;
        
        employeeCsvRepository.LoadFromExecutingDirectory(employeeType == "junior" ? "Juniors5.csv" : "Teamleads5.csv");
        
        var employees = employeeCsvRepository.Get();
        var employeeName = employees.First(e => e.Id == employeeId).Name;

        EmployeeName = employeeName;
    }
}