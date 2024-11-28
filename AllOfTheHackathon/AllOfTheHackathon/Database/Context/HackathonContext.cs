using AllOfTheHackathon.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace AllOfTheHackathon.Database.Context;

public sealed class HackathonContext : DbContext
{
    public DbSet<HackathonEntity> Hackathons { get; set; } 
    public DbSet<JuniorEntity> Juniors { get; set; } 
    public DbSet<JuniorWishlistEntity> JuniorWishlists { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<TeamLeadEntity> TeamLeads { get; set; } 
    public DbSet<TeamLeadWishlistEntity> TeamLeadWishlists { get; set; }
    
    public HackathonContext()
    {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;" + 
            "Port=5434;" + 
            "Database=all-of-the-hackathon;" + 
            "Username=all-of-the-hackathon-owner;" +
            "Password=all-of-the-hackathon-password");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HackathonEntity>()
            .HasMany(h => h.Juniors)
            .WithMany()
            .UsingEntity("HackathonsJuniors");

        modelBuilder.Entity<HackathonEntity>()
            .HasMany(h => h.TeamLeads)
            .WithMany()
            .UsingEntity("HackathonsTeamLeads");

        modelBuilder.Entity<HackathonEntity>()
            .HasMany(h => h.JuniorWishlists)
            .WithOne();

        modelBuilder.Entity<HackathonEntity>()
            .HasMany(h => h.TeamLeadWishlists)
            .WithOne();

        modelBuilder.Entity<HackathonEntity>()
            .HasMany(h => h.Teams)
            .WithOne();
    }
}