using AllOfTheHackathon.Calculator;
using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Repository;
using AllOfTheHackathon.Service;
using AllOfTheHackathon.Service.Transient;
using AllOfTheHackathon.TeamBuildingStrategy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
{
    services.AddHostedService<HackathonWorker>();
    services.AddTransient<Hackathon>();
    services.AddTransient<ITeamBuildingStrategy, GaleShapleyTeamBuildingStrategy>();
    services.AddTransient<HrManager>();
    services.AddTransient<HrDirector>();
    services.AddTransient<EmployeeCsvRepository>();
    services.AddTransient<ICalculator, HarmonicMeanCalculator>();
}).Build();

host.Run();