using Dental.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dental.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<DentalHistory> DentalHistories { get; set; }
        public DbSet<Tooth> Teeth { get; set; }
        public DbSet<ToothRecord> ToothRecords { get; set; }
        public DbSet<ToothImage> ToothImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.MedicalHistory)
                .WithOne(m => m.Patient)
                .HasForeignKey<MedicalHistory>(m => m.PatientId);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.DentalHistory)
                .WithOne(d => d.Patient)
                .HasForeignKey<DentalHistory>(d => d.PatientId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Teeth)
                .WithOne(t => t.Patient)
                .HasForeignKey(t => t.PatientId);

            modelBuilder.Entity<Tooth>()
                .HasMany(t => t.Records)
                .WithOne(r => r.Tooth)
                .HasForeignKey(r => r.ToothId);

            modelBuilder.Entity<ToothRecord>()
                .HasMany(r => r.Images)
                .WithOne(i => i.ToothRecord)
                .HasForeignKey(i => i.ToothRecordId);
        }
    }
}