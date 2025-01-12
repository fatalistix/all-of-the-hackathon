using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using AllOfTheHackathon.TeamBuildingStrategy;
using RabbitCommon.Configs;
using HrManagerRabbitService.Clients;
using HrManagerRabbitService.Consumers;
using HrManagerRabbitService.Databases.Contexts;
using HrManagerRabbitService.Mappers;
using HrManagerRabbitService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Refit;

var rabbitMqConfig = new RabbitMqConfig();

var builder = WebApplication.CreateBuilder();
builder.Services.AddTransient<CheckingService>();
builder.Services.AddTransient<CollectingService>();
builder.Services.AddTransient<HrManagerService>();
builder.Services.AddTransient<HrManager>();
builder.Services.AddTransient<ITeamBuildingStrategy, GaleShapleyTeamBuildingStrategy>();
builder.Services.AddTransient<ITeamSender>(_ => RestService.For<ITeamSender>("http://hr-director:6970"));
builder.Services.AddAutoMapper(typeof(EmployeeWithDesiredEmployeesProfile));
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<HrManagerContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(
        "Host=db;" +
        "Port=5432;" +
        "Database=hr-manager;" +
        "Username=all-of-the-hackathon-owner;" +
        "Password=all-of-the-hackathon-password");
    optionsBuilder.EnableSensitiveDataLogging();
    optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<EmployeeConsumer>();
    configure.AddConsumer<HackathonStartConsumer>();
    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqConfig.Host, "/", rabbitMqHostConfigurator =>
        {
            rabbitMqHostConfigurator.Username(rabbitMqConfig.Username);
            rabbitMqHostConfigurator.Password(rabbitMqConfig.Password);
        });
        cfg.ReceiveEndpoint("hr-manager-employee", e => { e.Consumer<EmployeeConsumer>(ctx); });
        cfg.ReceiveEndpoint("hr-manager-hackathon-start", e => { e.Consumer<HackathonStartConsumer>(ctx); });
    });
});
builder.Configuration.AddJsonFile("appsettings.HrManager.json", true, true);

var app = builder.Build();

app.MapHealthChecks("/healthz");

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<HrManagerContext>();
    context.Database.Migrate();
}

app.Run();