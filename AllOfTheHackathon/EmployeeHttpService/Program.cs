using AllOfTheHackathon.Repository;
using EmployeeHttpService;
using EmployeeHttpService.Configs;
using EmployeeHttpService.Services;
using Refit;

var employeeConfig = EmployeeConfigReader.Read();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(employeeConfig);
builder.Services.AddHostedService<EmployeeService>();
builder.Services.AddTransient<IEmployeeCsvRepository, EmployeeCsvRepository>();
builder.Services.AddTransient<IPreferencesSender>(_ => RestService.For<IPreferencesSender>("http://hr-manager:6969"));
builder.Configuration.AddJsonFile("appsettings.Employee.json", true, true);

var app = builder.Build();

app.Run();