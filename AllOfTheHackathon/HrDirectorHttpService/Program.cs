using AllOfTheHackathon.Calculator;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
using AllOfTheHackathon.Service.Transient;
using HrDirectorHttpService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
builder.Services.AddTransient<HrDirectorService>();
builder.Services.AddTransient<HrDirector>();
builder.Services.AddTransient<HackathonContext>();
builder.Services.AddTransient<ICalculator, HarmonicMeanCalculator>();
builder.Services.AddAutoMapper(typeof(HackathonProfile));
builder.Services.AddAutoMapper(typeof(JuniorProfile));
builder.Services.AddAutoMapper(typeof(JuniorWishlistProfile));
builder.Services.AddAutoMapper(typeof(TeamProfile));
builder.Services.AddAutoMapper(typeof(TeamLeadProfile));
builder.Services.AddAutoMapper(typeof(TeamLeadWishlistProfile));
builder.Services.AddDbContext<HackathonContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(
        "Host=db;" + 
        "Port=5432;" + 
        "Database=hr-director;" + 
        "Username=all-of-the-hackathon-owner;" +
        "Password=all-of-the-hackathon-password");
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(6970);  
});
var app = builder.Build();

app.MapControllers();

app.Run();