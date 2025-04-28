using Microsoft.EntityFrameworkCore;
using MittoServer.Models;

namespace MittoServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Topic> Topics { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description);
            entity.HasMany(e => e.Clients)
				.WithMany(e => e.Topics)
                .UsingEntity<ClientTopic>(
					j => j.HasOne(ct => ct.Client).WithMany().HasForeignKey(ct => ct.ClientId),
					j => j.HasOne(ct => ct.Topic).WithMany().HasForeignKey(ct => ct.TopicId)
				 );
		});

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientSecret).IsRequired();
            entity.HasMany(e => e.Topics)
                .WithMany(e => e.Clients)
				.UsingEntity<ClientTopic>(
                    j => j.HasOne(ct => ct.Topic).WithMany().HasForeignKey(ct => ct.TopicId),
					j => j.HasOne(ct => ct.Client).WithMany().HasForeignKey(ct => ct.ClientId)
                 );
        });
    }
} 