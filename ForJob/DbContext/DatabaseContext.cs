using ForJob.Domain;
using Microsoft.EntityFrameworkCore;

namespace ForJob.DbContext
{
    public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Domain.Task> Tasks => Set<Domain.Task>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Task>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Domain.Task>().HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<Domain.User>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }

        public string DbPath { get; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                var dbPath = System.IO.Path.Join(path, "kidplayground.db");
                options.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}
