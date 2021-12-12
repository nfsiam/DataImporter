using Microsoft.EntityFrameworkCore;
using DataImporter.Core.Entities;
using DataImporter.Core.Seeds;
using DataImporter.Membership.Entities;

namespace DataImporter.Core.Contexts
{
    public class CoreDbContext : DbContext, ICoreDbContext
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;

        public CoreDbContext(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(
                    _connectionString,
                    m => m.MigrationsAssembly(_migrationAssemblyName));
            }

            base.OnConfiguring(dbContextOptionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("AspNetUsers", t => t.ExcludeFromMigrations())
                .HasMany<Group>()
                .WithOne(x => x.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .ToTable("AspNetUsers", t => t.ExcludeFromMigrations())
                .HasMany<Export>()
                .WithOne(x => x.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasMany(x => x.Imports)
                .WithOne(x => x.Group);

            modelBuilder.Entity<Group>()
                .HasMany(x => x.Headers)
                .WithOne(x => x.Group);

            modelBuilder.Entity<Group>()
                .HasMany(x => x.Rows)
                .WithOne(x => x.Group);

            modelBuilder.Entity<Row>()
                .HasMany(x => x.Cells)
                .WithOne(x => x.Row);

            modelBuilder.Entity<Header>()
                .HasMany<Cell>()
                .WithOne(x => x.Header)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Contact>()
                .ToTable(nameof(Contact), t => t.ExcludeFromMigrations())
                .HasNoKey();

            modelBuilder.Entity<Export>()
                .HasMany(x => x.ExportGroups)
                .WithOne(x => x.Export);

            modelBuilder.Entity<Group>()
                .HasMany<ExportGroup>()
                .WithOne(x => x.Group);

            // Seeds
            modelBuilder.Entity<Group>()
                .HasData(DataSeed.Groups());
            modelBuilder.Entity<Header>()
                .HasData(DataSeed.Headers());
            modelBuilder.Entity<Row>()
                .HasData(DataSeed.Rows());
            modelBuilder.Entity<Cell>()
                .HasData(DataSeed.Cells());
            modelBuilder.Entity<Import>()
                .HasData(DataSeed.Imports());
            modelBuilder.Entity<Export>()
                .HasData(DataSeed.Exports());
            modelBuilder.Entity<ExportGroup>()
                .HasData(DataSeed.ExportGroups());

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<Export> Exports { get; set; }
        public DbSet<Header> Headers { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}