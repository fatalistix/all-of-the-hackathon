using AllOfTheHackathon.Repository;
using EmployeeRabbitService.Configs;
using EmployeeRabbitService.Consumers;
using EmployeeRabbitService.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var rabbitMqConfig = new RabbitMqConfig();

var type = Environment.GetEnvironmentVariable("EMPLOYEE_TYPE");
var id = Environment.GetEnvironmentVariable("EMPLOYEE_ID");

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<StatusService>();
builder.Services.AddTransient<IEmployeeCsvRepository, EmployeeCsvRepository>();
builder.Services.AddTransient<EmployeeService>();
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
        cfg.ReceiveEndpoint($"{type}-{id}", e =>
        {
            e.Consumer<HackathonStartConsumer>(ctx);
        });
    });
});

var app = builder.Build();

app.Run();
