using HelpDeskWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskWeb.Data
{
    public class AppDbContext : IdentityDbContext<Usser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TicketStatus> TicketStatus { get; set; }
        public DbSet<TicketPriority> TicketPriority { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ===== Usser =====
            builder.Entity<Usser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Usser>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // ===== TicketStatus =====
            builder.Entity<TicketStatus>()
                .Property(ts => ts.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<TicketStatus>()
                .Property(ts => ts.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<TicketStatus>()
                .Property(ts => ts.Name)
                .HasMaxLength(100);

            builder.Entity<TicketStatus>()
                .HasIndex(ts => ts.Name)
                .IsUnique();

            // ===== TicketPriority =====
            builder.Entity<TicketPriority>()
                .Property(tp => tp.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<TicketPriority>()
                .Property(tp => tp.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<TicketPriority>()
                .Property(tp => tp.Name)
                .HasMaxLength(100);

            builder.Entity<TicketPriority>()
                .HasIndex(tp => tp.Name)
                .IsUnique();

            // ===== Ticket =====
            builder.Entity<Ticket>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Ticket>()
                .Property(t => t.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Ticket>()
                .HasOne(t => t.TicketStatus)
                .WithMany()
                .HasForeignKey(t => t.TicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.TicketPriority)
                .WithMany()
                .HasForeignKey(t => t.TicketPriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.ResolvedBy)
                .WithMany()
                .HasForeignKey(t => t.ResolvedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.ClosedBy)
                .WithMany()
                .HasForeignKey(t => t.ClosedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}