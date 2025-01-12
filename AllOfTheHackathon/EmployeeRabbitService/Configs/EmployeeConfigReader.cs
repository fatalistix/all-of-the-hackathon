namespace EmployeeRabbitService.Configs;

public abstract class EmployeeConfigReader
{
    public static EmployeeConfig Read()
    {
        var employeeType = Environment.GetEnvironmentVariable("EMPLOYEE_TYPE");
        if (employeeType == null)
        {
            throw new InvalidOperationException("Missing EMPLOYEE_TYPE");
        }

        if (employeeType is not ("teamlead" or "junior"))
        {
            throw new InvalidOperationException("Invalid EMPLOYEE_TYPE");
        }

        var employeeIdStr = Environment.GetEnvironmentVariable("EMPLOYEE_ID");
        if (employeeIdStr == null)
        {
            throw new InvalidOperationException("Missing EMPLOYEE_ID");
        }

        var success = int.TryParse(employeeIdStr, out var employeeId);
        if (!success)
        {
            throw new InvalidOperationException("Invalid EMPLOYEE_ID");
        }

        return new EmployeeConfig(employeeId, employeeType);
    }
}