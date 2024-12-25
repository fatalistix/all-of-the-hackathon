using AllOfTheHackathon.Internal.Extension;
using AllOfTheHackathon.Repository;
using EmployeeHttpService;
using HttpCommon.Requests;
using Refit;

var employeeType = Environment.GetEnvironmentVariable("EMPLOYEE_TYPE");
if (employeeType == null)
{
    Console.Error.WriteLine("No EMPLOYEE_TYPE variable");
    return;
}

if (employeeType is not ("teamlead" or "junior"))
{
    Console.Error.WriteLine("Unexpected EMPLOYEE_TYPE");
    return;
}

var employeeIdStr = Environment.GetEnvironmentVariable("EMPLOYEE_ID");
if (employeeIdStr == null)
{
    Console.Error.WriteLine("No EMPLOYEE_ID variable");
    return;
}

var success = int.TryParse(employeeIdStr, out var employeeId);
if (!success)
{
    Console.Error.WriteLine("EMPLOYEE_ID does not contain number");
    return;
}

if (employeeId is < 1 or > 5)
{
    Console.Error.WriteLine("EMPLOYEE_ID contains invalid number");
    return;
}

var repository = new EmployeeCsvRepository();
repository.LoadFromExecutingDirectory(employeeType == "junior" ? "Juniors5.csv" : "Teamleads5.csv");

var employees = repository.Get();
var name = employees.First(e => e.Id == employeeId).Name;

var desiredEmployees = new List<int>([1, 2, 3, 4, 5]);
desiredEmployees.Shuffle();

var service = RestService.For<IPreferencesSender>("http://hr-manager:6969");
await service.SendPreferences(new EmployeeRequest(employeeId, name, desiredEmployees, employeeType));