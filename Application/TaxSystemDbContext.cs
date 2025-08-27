using Application.ReadModels;
using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public class TaxSystemDbContext : DbContext
    {
        public TaxSystemDbContext(DbContextOptions<TaxSystemDbContext> options) : base(options)
        {
        }

        // Read Models Tables
        public DbSet<Taxpayer> Taxpayers { get; set; }
        public DbSet<TaxpayerAddress> TaxpayerAddresses { get; set; }
        
        // Event Sourcing Tables
        public DbSet<ProjectorCheckpoint> ProjectorCheckpoints { get; set; }
        public DbSet<AppliedEvent> AppliedEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Taxpayers table
            modelBuilder.Entity<Taxpayer>(entity =>
            {
                entity.ToTable("Taxpayers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MiddleName).HasMaxLength(100);
                entity.Property(e => e.DateOfBirth).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.SocialSecurityNumber).HasMaxLength(11);
                entity.Property(e => e.TaxIdentificationNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.FilingStatus).HasMaxLength(50);
                entity.Property(e => e.AnnualIncome).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EmploymentStatus).HasMaxLength(50);
                entity.Property(e => e.EmployerName).HasMaxLength(255);
                entity.Property(e => e.EmployerIdentificationNumber).HasMaxLength(20);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(100);
                entity.Property(e => e.TaxLiability).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxPaid).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxRefund).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxFilingDate).HasColumnType("timestamp with time zone");
                
                // Indexes for performance
                entity.HasIndex(e => e.SocialSecurityNumber);
                entity.HasIndex(e => e.TaxIdentificationNumber);
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure TaxpayerAddresses table
            modelBuilder.Entity<TaxpayerAddress>(entity =>
            {
                entity.ToTable("TaxpayerAddresses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.TaxpayerId).IsRequired();
                entity.Property(e => e.StreetAddress).IsRequired().HasMaxLength(255);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.State).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AddressType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsPrimary).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.LastModifiedDate).HasColumnType("timestamp with time zone");
                
                // Foreign key relationship
                entity.HasOne<Taxpayer>()
                    .WithMany(t => t.Addresses)
                    .HasForeignKey(a => a.TaxpayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Indexes for performance
                entity.HasIndex(e => e.TaxpayerId);
                entity.HasIndex(e => e.IsPrimary);
                entity.HasIndex(e => new { e.TaxpayerId, e.IsPrimary });
            });

            // Configure ProjectorCheckpoints table
            modelBuilder.Entity<ProjectorCheckpoint>(entity =>
            {
                entity.ToTable("ProjectorCheckpoints");
                entity.HasKey(e => e.ProjectorName);
                entity.Property(e => e.ProjectorName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.LastPosition).IsRequired();
                entity.Property(e => e.LastUpdated).IsRequired().HasColumnType("timestamp with time zone");
            });

            // Configure AppliedEvents table
            modelBuilder.Entity<AppliedEvent>(entity =>
            {
                entity.ToTable("AppliedEvents");
                entity.HasKey(e => new { e.EventId, e.ProjectorName });
                entity.Property(e => e.EventId).IsRequired();
                entity.Property(e => e.ProjectorName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AppliedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.Position).IsRequired();
                
                // Indexes for performance
                entity.HasIndex(e => e.ProjectorName);
                entity.HasIndex(e => e.Position);
                entity.HasIndex(e => new { e.ProjectorName, e.Position });
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Configure all DateTime properties to be stored as UTC
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<DateTimeToUtcConverter>();
        }
    }

    public class DateTimeToUtcConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
    {
        public DateTimeToUtcConverter() : base(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}
