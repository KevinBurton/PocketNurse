using System.Collections.Generic;

namespace PocketNurse.Models
{
    public class OmnicellCabinetViewModel
    {
        public OmnicellCabinetViewModel()
        {
            Patients = new List<PatientDescription>();
            NotInFormulary = new List<NotInFormulary>();
        }
        public List<PatientDescription> Patients { get; set; }
        public List<NotInFormulary> NotInFormulary { get; set; }
    }
}
