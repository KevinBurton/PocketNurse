using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
