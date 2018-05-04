using Microsoft.EntityFrameworkCore;

namespace PocketNurse.Models
{
    public class PocketNurseContext : DbContext
    {
        public PocketNurseContext(DbContextOptions<PocketNurseContext> options)
            : base(options)
        {
        }

        public DbSet<PocketNurse.Models.Patient> Patient { get; set; }
        public DbSet<PocketNurse.Models.MedicationOrder> MedicationOrder { get; set; }
        public DbSet<PocketNurse.Models.NotInFormulary> NotInFormulary { get; set; }
        public DbSet<PocketNurse.Models.PatientAllergy> PatientAllergy { get; set; }
        public DbSet<PocketNurse.Models.Allergy> Allergy { get; set; }
        public DbSet<PocketNurse.Models.Cabinet> Cabinet { get; set; }
        public DbSet<PocketNurse.Models.CabinetSession> CabinetSession { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasKey(table => new
            {
                table.PatientId,
                table.CabinetId
            });
        }
    }
}
