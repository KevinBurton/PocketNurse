using System.Collections.Generic;

namespace PocketNurse.Models
{
    public class PatientDescription
    {
        public PatientDescription()
        {
            Allergies = new List<Allergy>();
            MedicationOrders = new List<MedicationOrder>();
        }
        public Patient Patient { get; set; }
        public List<Allergy> Allergies { get; }
        public List<MedicationOrder> MedicationOrders { get; }
    }
}
