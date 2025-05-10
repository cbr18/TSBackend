using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;

namespace TSBackend.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Venue> Venues { get; set; } = null!;
    public DbSet<Hall> Halls { get; set; } = null!;
    public DbSet<Row> Rows { get; set; } = null!;
    public DbSet<Seat> Seats { get; set; } = null!;
    public DbSet<Meeting> Meetings { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Это можно заменить на получение строки подключения из конфигурации
            optionsBuilder.UseNpgsql("Host=localhost;Database=ticketing_system;Username=your_username;Password=your_password");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure City
        modelBuilder.Entity<City>()
            .HasMany(c => c.Venues)
            .WithOne(v => v.City)
            .HasForeignKey(v => v.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Venue
        modelBuilder.Entity<Venue>()
            .HasOne(v => v.City)
            .WithMany(c => c.Venues)
            .HasForeignKey(v => v.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venue>()
            .HasMany(v => v.Halls)
            .WithOne(h => h.Venue)
            .HasForeignKey(h => h.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Hall
        modelBuilder.Entity<Hall>()
            .HasOne(h => h.Venue)
            .WithMany(v => v.Halls)
            .HasForeignKey(h => h.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hall>()
            .HasMany(h => h.Rows)
            .WithOne(r => r.Hall)
            .HasForeignKey(r => r.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hall>()
            .HasMany(h => h.Meetings)
            .WithOne(m => m.Hall)
            .HasForeignKey(m => m.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Row
        modelBuilder.Entity<Row>()
            .HasOne(r => r.Hall)
            .WithMany(h => h.Rows)
            .HasForeignKey(r => r.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Row>()
            .HasMany(r => r.Seats)
            .WithOne(s => s.Row)
            .HasForeignKey(s => s.RowId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Seat
        modelBuilder.Entity<Seat>()
            .HasOne(s => s.Row)
            .WithMany(r => r.Seats)
            .HasForeignKey(s => s.RowId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Seat>()
            .HasMany(s => s.Tickets)
            .WithOne(t => t.Seat)
            .HasForeignKey(t => t.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Meeting
        modelBuilder.Entity<Meeting>()
            .HasOne(m => m.Hall)
            .WithMany(h => h.Meetings)
            .HasForeignKey(m => m.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Meeting>()
            .HasMany(m => m.Tickets)
            .WithOne(t => t.Meeting)
            .HasForeignKey(t => t.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Ticket
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Meeting)
            .WithMany(m => m.Tickets)
            .HasForeignKey(t => t.MeetingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Seat)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasIndex(t => new { t.MeetingId, t.SeatId })
            .IsUnique();

        // Configure User
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure unique constraints
        modelBuilder.Entity<Venue>()
            .HasIndex(v => new { v.CityId, v.Address })
            .IsUnique();

        modelBuilder.Entity<City>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Hall>()
            .HasIndex(h => new { h.VenueId, h.Name })
            .IsUnique();

        modelBuilder.Entity<Row>()
            .HasIndex(r => new { r.HallId, r.RowNumber })
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new { s.RowId, s.SeatNumber })
            .IsUnique();

        modelBuilder.Entity<Meeting>()
            .HasIndex(m => new { m.Name, m.Date, m.HallId })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}