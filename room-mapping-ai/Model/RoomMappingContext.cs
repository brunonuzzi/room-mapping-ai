using Microsoft.EntityFrameworkCore;

namespace room_mapping_ai.Model
{
    public class RoomMappingContext : DbContext
    {
        public RoomMappingContext(DbContextOptions<RoomMappingContext> options) : base(options) { }

        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<RoomsResult> RoomsResult { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("vector");
            modelBuilder.Entity<Rooms>().ToTable("rooms", t => t.ExcludeFromMigrations());

            base.OnModelCreating(modelBuilder);
        }
    }
}
