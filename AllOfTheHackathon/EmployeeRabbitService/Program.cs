using AllOfTheHackathon.Repository;
using EmployeeRabbitService.Configs;
using EmployeeRabbitService.Consumers;
using EmployeeRabbitService.Services;
using MassTransit;
using RabbitCommon.Configs;

var employeeConfig = EmployeeConfigReader.Read();
var rabbitMqConfig = new RabbitMqConfig();

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton(employeeConfig);
builder.Services.AddTransient<EmployeeService>();
builder.Services.AddTransient<IEmployeeCsvRepository, EmployeeCsvRepository>();
builder.Configuration.AddJsonFile("appsettings.Employee.json", true, true);
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<HackathonStartConsumer>();
    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqConfig.Host, "/", rabbitMqHostConfigurator =>
        {
            rabbitMqHostConfigurator.Username(rabbitMqConfig.Username);
            rabbitMqHostConfigurator.Password(rabbitMqConfig.Password);
        });
        cfg.ReceiveEndpoint($"{employeeConfig.Type}-{employeeConfig.Id}", e =>
        {
            e.Consumer<HackathonStartConsumer>(ctx);
        });
    });
});

var app = builder.Build();

app.Run();
