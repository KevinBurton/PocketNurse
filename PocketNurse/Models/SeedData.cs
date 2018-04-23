using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PocketNurseContext(serviceProvider.GetRequiredService<DbContextOptions<PocketNurseContext>>()))
            {
                // Look for any movies.
                if (context.Patient.Any())
                {
                    return;   // DB has been seeded
                }
                context.Patient.AddRange(
                     new Patient
                     {
                         PatientId = "1",
                         MRN = "1",
                         First = "Luke",
                         Last = "Skywalker",
                         FullName = "Luke Skywalker",
                         // 19 BBY
                         DOB = DateTime.Parse("4019-1-1")
                     },
                     new Patient
                     {
                         PatientId = "2",
                         MRN = "2",
                         First = "Leia",
                         Last = "Organa",
                         FullName = "Leia Organa",
                         DOB = DateTime.Parse("4019-1-1")
                     });
                context.SaveChanges();
            }
        }
    }
}
