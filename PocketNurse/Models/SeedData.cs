using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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
                var luke =
                      new Patient
                      {
                          PatientId = "1",
                          MRN = "1",
                          First = "Luke",
                          Last = "Skywalker",
                          FullName = "Luke Skywalker",
                          // 19 BBY
                          DOB = DateTime.Parse("4019-1-1"),
                          Cabinet = new Cabinet()
                          {
                              State = "Tatooine",
                              Area = "Farm"
                          }
                      };
                context.Patient.Add(luke);
                context.SaveChanges();

                var leia =
                     new Patient
                     {
                         PatientId = "2",
                         CabinetId = luke.Cabinet.CabinetId,
                         MRN = "2",
                         First = "Leia",
                         Last = "Organa",
                         FullName = "Leia Organa",
                         DOB = DateTime.Parse("4019-1-1")
                     };
                context.Patient.Add(leia);
                context.SaveChanges();
            }
        }
    }
}
