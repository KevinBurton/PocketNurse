using System.Collections.Generic;

namespace PocketNurse.Models
{
    public class MedicationOrderRaw
    {
        public MedicationOrderRaw()
        {
            PatientIdList = new List<string>();
        }
        public string PocketNurseItemId { get; set; }
        public string MedicationName { get; set; }
        public string Dose { get; set; }
        public string Frequency { get; set; }
        public string Route { get; set; }
        public List<string> PatientIdList { get; set; }
    }
}
