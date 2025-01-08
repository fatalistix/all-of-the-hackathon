using HrManagerHttpService.Databases.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagerHttpService.Databases.Contexts;

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
}