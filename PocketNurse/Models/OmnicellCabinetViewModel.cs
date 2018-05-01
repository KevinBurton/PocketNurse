using System.Collections.Generic;

namespace PocketNurse.Models
{
    public class OmnicellCabinetViewModel
    {
        public CabinetSession Session;
        public OmnicellCabinetViewModel(CabinetSession session)
        {
            Session = session;
            Patients = new List<PatientDescription>();
            NotInFormulary = new List<NotInFormulary>();
        }
        public List<PatientDescription> Patients { get; set; }
        public List<NotInFormulary> NotInFormulary { get; set; }
    }
}
