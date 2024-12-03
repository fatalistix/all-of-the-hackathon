using AllOfTheHackathon.Calculator;
using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
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
    services.AddDbContext<HackathonContext>();
    services.AddAutoMapper(typeof(HackathonProfile));
    services.AddAutoMapper(typeof(JuniorProfile));
    services.AddAutoMapper(typeof(JuniorWishlistProfile));
    services.AddAutoMapper(typeof(TeamProfile));
    services.AddAutoMapper(typeof(TeamLeadProfile));
    services.AddAutoMapper(typeof(TeamLeadWishlistProfile));
    services.AddTransient<AverageSatisfactionCalculator>();
    services.AddTransient<EmployeeUpdater>();
    services.AddTransient<HackathonEvent>();
}).Build();

host.Run();