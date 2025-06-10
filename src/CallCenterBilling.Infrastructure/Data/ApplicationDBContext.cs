using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Call> Calls { get; set; }
    public DbSet<AgentSession> AgentSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Rename Identity tables (optional)
        builder.Entity<ApplicationUser>().ToTable("Users");

        builder.Entity<Agent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.Email).IsUnique();
            });

        // Seed data
        builder.Entity<Agent>().HasData(
            new Agent { Id = 1, Name = "Sarah Johnson", Email = "sarah.johnson@company.com", Status = AgentStatus.Active, TotalCalls = 89, TotalRevenue = 2150, Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new Agent { Id = 2, Name = "Mike Chen", Email = "mike.chen@company.com", Status = AgentStatus.Active, TotalCalls = 76, TotalRevenue = 1980, Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-25) },
            new Agent { Id = 3, Name = "Lisa Rodriguez", Email = "lisa.rodriguez@company.com", Status = AgentStatus.OnBreak, TotalCalls = 82, TotalRevenue = 1840, Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Agent { Id = 4, Name = "David Smith", Email = "david.smith@company.com", Status = AgentStatus.Active, TotalCalls = 71, TotalRevenue = 1720, Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Agent { Id = 5, Name = "Emma Wilson", Email = "emma.wilson@company.com", Status = AgentStatus.Offline, TotalCalls = 68, TotalRevenue = 1650, Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Agent { Id = 6, Name = "Alex Brown", Email = "alex.brown@company.com", Status = AgentStatus.Active, TotalCalls = 65, TotalRevenue = 1580, Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-5) }
        );

        // Call configuration
        builder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerPhoneNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Revenue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Agent)
                  .WithMany()
                  .HasForeignKey(e => e.AgentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.AgentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartTime);
        });

        // AgentSession configuration
        builder.Entity<AgentSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ConnectionId).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Agent)
                  .WithMany()
                  .HasForeignKey(e => e.AgentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.AgentId);
            entity.HasIndex(e => e.ConnectionId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.LastHeartbeat);
        });
    }
}