﻿using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
IdentityUserToken<int>>(options)
{
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.User)
        .HasForeignKey(ur => ur.UserId)
        .IsRequired();

        modelBuilder.Entity<AppRole>()
         .HasMany(ur => ur.UserRoles)
         .WithOne(u => u.Role)
         .HasForeignKey(ur => ur.RoleId)
         .IsRequired();

        modelBuilder.Entity<UserLike>()
        .HasKey(k => new { k.SourceUserId, k.TargetUderId });

        modelBuilder.Entity<UserLike>()
        .HasOne(s => s.SourceUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
        .HasOne(s => s.TargetUser)
        .WithMany(l => l.LikedByUsers)
        .HasForeignKey(s => s.TargetUderId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
        .HasOne(x => x.Sender)
        .WithMany(m => m.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
        .HasOne(x => x.Recipient)
        .WithMany(m => m.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
