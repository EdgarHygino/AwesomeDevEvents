using AwesomeDevEvents.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Context
{
    public class DevEventsDBContext: DbContext
    {
        public DevEventsDBContext(DbContextOptions<DevEventsDBContext> options) : base(options)
        {
            
        }

        public DbSet<DevEvent> DevEvents { get; set; }

        public DbSet<DevEventSpeaker> devEventSpeakers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DevEvent>(e =>
            {
                e.HasKey(e => e.Id);

                e.Property(e => e.Title).IsRequired(false);

                e.Property(e => e.Description).HasMaxLength(200).HasColumnType("varchar(200)");

                e.Property(e => e.StartDate).HasColumnName("start_date");

                e.Property(e => e.EndDate).HasColumnName("end_date");

                e.HasMany(e => e.Speakers).WithOne().HasForeignKey(e => e.DevEventId);
            });

            modelBuilder.Entity<DevEventSpeaker>(e =>
            {
                e.HasKey(e => e.Id);

                e.Property(e => e.Name).HasConversion(typeof(string));
            });
        }
    }
}
