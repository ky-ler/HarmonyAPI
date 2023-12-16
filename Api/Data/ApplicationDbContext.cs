using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Server> Servers { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Channel> Channels { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Server>()
            .HasMany(s => s.Members)
            .WithOne(m => m.Server)
            .HasForeignKey(m => m.ServerId)
            .HasPrincipalKey(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Servers)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Channel>()
            .HasMany(u => u.Messages)
            .WithOne(m => m.Channel)
            .HasForeignKey(m => m.ChannelId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Server>()
            .HasMany(u => u.Channels)
            .WithOne(m => m.Server)
            .HasForeignKey(m => m.ServerId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Server>()
            .HasIndex(s => s.InviteCode)
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasOne(m => m.Server)
            .WithMany(s => s.Members)
            .HasForeignKey(m => m.ServerId)
            .HasPrincipalKey(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

