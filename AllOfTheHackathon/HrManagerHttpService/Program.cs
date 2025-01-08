using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Service.Transient;
using AllOfTheHackathon.TeamBuildingStrategy;
using HrManagerHttpService.Clients;
using HrManagerHttpService.Databases.Contexts;
using HrManagerHttpService.Mappers;
using HrManagerHttpService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Refit;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
builder.Services.AddHostedService<CheckingService>();
builder.Services.AddTransient<CollectingService>();
builder.Services.AddTransient<HrManagerService>();
builder.Services.AddTransient<HrManager>();
builder.Services.AddTransient<ITeamBuildingStrategy, GaleShapleyTeamBuildingStrategy>();
builder.Services.AddTransient<ITeamsAndWishlistsSender>(_ =>
    RestService.For<ITeamsAndWishlistsSender>("http://hr-director:6970"));
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
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(6969); });
builder.Configuration.AddJsonFile("appsettings.HrManager.json", true, true);
var app = builder.Build();

app.MapControllers();
app.MapHealthChecks("/healthz");

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<HrManagerContext>();
    context.Database.Migrate();
}

app.Run();