using AllOfTheHackathon.Calculator;
using AllOfTheHackathon.Database.Context;
using AllOfTheHackathon.Mapper;
using AllOfTheHackathon.Service.Transient;
using EmployeeRabbitService.Configs;
using HrDirectorRabbitService.Consumers;
using HrDirectorRabbitService.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var rabbitMqConfig = new RabbitMqConfig();

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
builder.Services.AddTransient<StatusService>();
builder.Services.AddSingleton<HrDirectorService>();
builder.Services.AddTransient<HrDirector>();
builder.Services.AddTransient<HackathonContext>();
builder.Services.AddSingleton<CollectingService>();
builder.Services.AddTransient<ICalculator, HarmonicMeanCalculator>();
builder.Services.AddAutoMapper(typeof(HackathonProfile));
builder.Services.AddAutoMapper(typeof(JuniorProfile));
builder.Services.AddAutoMapper(typeof(JuniorWishlistProfile));
builder.Services.AddAutoMapper(typeof(TeamProfile));
builder.Services.AddAutoMapper(typeof(TeamLeadProfile));
builder.Services.AddAutoMapper(typeof(TeamLeadWishlistProfile));
builder.Services.AddHostedService<HrDirectorHostedService>();
builder.Services.AddDbContext<HackathonContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(
        "Host=db;" + 
        "Port=5432;" + 
        "Database=all-of-the-hackathon;" + 
        "Username=all-of-the-hackathon-owner;" +
        "Password=all-of-the-hackathon-password");
});
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<WishlistsConsumer>();
    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqConfig.Host, "/", rabbitMqHostConfigurator =>
        {
            rabbitMqHostConfigurator.Username(rabbitMqConfig.Username);
            rabbitMqHostConfigurator.Password(rabbitMqConfig.Password);
        });
        cfg.ReceiveEndpoint("hr-director", e =>
        {
            e.Consumer<WishlistsConsumer>(context);
        });
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(6970);  
});

var app = builder.Build();

app.MapControllers();

app.Run();
