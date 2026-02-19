using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Api.Models;

namespace SupportTicketSystem.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets=>Set<Ticket>();
        public DbSet<TicketComment> TicketComments=>Set<TicketComment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ticket>()
            .HasOne(t=>t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t=>t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
            .HasOne(t=>t.AssignedAgent)
            .WithMany()
            .HasForeignKey(t=>t.AssignedAgentId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketComment>()
            .HasOne(tc=>tc.Ticket)
            .WithMany(t=>t.Comments)
            .HasForeignKey(tc=>tc.TicketId);

            modelBuilder.Entity<TicketComment>()
            .HasOne(tc=>tc.User)
            .WithMany()
            .HasForeignKey(tc=>tc.UserId);
        }
    } 
}
