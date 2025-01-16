using HrManagerRabbitService.Databases.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagerRabbitService.Databases.Contexts;

public sealed class HrManagerContext : DbContext
{
    public DbSet<JuniorWithDesiredTeamLeadsIds> Juniors { get; set; }
    public DbSet<TeamLeadWithDesiredJuniorsIds> TeamLeads { get; set; }

    public HrManagerContext()
    {
        Database.EnsureCreated();
        Database.Migrate();
    }

    public HrManagerContext(DbContextOptions<HrManagerContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JuniorWithDesiredTeamLeadsIds>().HasKey(x => new { x.Id, x.HackathonId });
        modelBuilder.Entity<TeamLeadWithDesiredJuniorsIds>().HasKey(x => new { x.Id, x.HackathonId });
    }
}