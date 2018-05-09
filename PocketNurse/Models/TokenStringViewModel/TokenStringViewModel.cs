using System.Collections.Generic;

namespace PocketNurse.Models
{
    public class TokenStringViewModel
    {
        private OmniSession _session;
        public TokenStringViewModel()
        {
            Patients = new List<string>();
            MedicationOrders = new List<string>();
            NotInFormulary = new List<string>();
        }
        public TokenStringViewModel(OmniSession session)
        {
            _session = session;
            Patients = new List<string>();
            MedicationOrders = new List<string>();
            NotInFormulary = new List<string>();
        }
        public void AddPatient(Patient patient, string allergies)
        {
            Patients.Add($"{_session.From,10}{_session.To,10}{"PA",16}\\site:{_session.SiteId}\\pid:{patient.PatientId}\\pna:{patient.FullName}\\dob:{patient.DOB.ToString("MM/dd")}\\alrgy:{allergies}\\mrn:{patient.MRN}");
        }
        public void AddMedicationOrder(MedicationOrderRaw medicationOrder)
        {
            foreach (var patientId in medicationOrder.PatientIdList)
            {
                MedicationOrders.Add($"{_session.From,10}{_session.To,10}{"MOA",16}\\moid:{0}\\pid:{patientId}\\item{medicationOrder.PocketNurseItemId}\\phyid:{0}\\phynm:{"Doctor"}\\frq:{medicationOrder.Frequency}\\mrte:{medicationOrder.Route}\\dose:{medicationOrder.Dose}");
            }
        }
        public void AddNotInFormulary(NotInFormulary item)
        {
            NotInFormulary.Add($"\\osi:{_session.OmniId}\\item:{item.GenericName}\\ina:{item.Alias}\\dssa:{item.Strength}\\dssu:{item.StrengthUnit}\\dsva:{item.TotalContainerVolume}\\dsa:{item.Volume}\\dsu:{item.VolumeUnit}\\dsf:{item.Route}");
        }

        public List<string> Patients { get; set; }
        public List<string> MedicationOrders { get; set; }
        public List<string> NotInFormulary { get; set; }
    }
}
