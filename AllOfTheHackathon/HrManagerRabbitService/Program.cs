using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using AllOfTheHackathon.TeamBuildingStrategy;
using EmployeeRabbitService.Configs;
using HrManagerRabbitService.Clients;
using HrManagerRabbitService.Consumers;
using HrManagerRabbitService.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Refit;

var rabbitMqConfig = new RabbitMqConfig();

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<CollectingService>();
builder.Services.AddTransient<HrManagerService>();
builder.Services.AddTransient<HrManager>();
builder.Services.AddTransient<ITeamBuildingStrategy, GaleShapleyTeamBuildingStrategy>();
builder.Services.AddTransient<ITeamSender>(_ => RestService.For<ITeamSender>("http://hr-director:6970"));
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<EmployeeConsumer>();
    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqConfig.Host, "/", rabbitMqHostConfigurator =>
        {
            rabbitMqHostConfigurator.Username(rabbitMqConfig.Username);
            rabbitMqHostConfigurator.Password(rabbitMqConfig.Password);
        });
        cfg.ReceiveEndpoint("hr-manager", e =>
        {
            e.Consumer<EmployeeConsumer>(ctx);
        });
    });
});

var app = builder.Build();

app.Run();