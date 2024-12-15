using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using AllOfTheHackathon.TeamBuildingStrategy;
using HrManagerHttpService.Clients;
using HrManagerHttpService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Refit;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
builder.Services.AddSingleton<CollectingService>();
builder.Services.AddTransient<HrManagerService>();
builder.Services.AddTransient<HrManager>();
builder.Services.AddTransient<ITeamBuildingStrategy, GaleShapleyTeamBuildingStrategy>();
builder.Services.AddTransient<ITeamSender>(_ => RestService.For<ITeamSender>("http://hr-director:6970"));
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(6969);
});
var app = builder.Build();

app.MapControllers();

app.Run();