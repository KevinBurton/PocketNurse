using System.Linq;
using System.Collections.Generic;
using System.IO;

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
            Patients.Add($"{_session.From,10}{_session.To,10}{"PA",16}\\site:{_session.SiteId}\\pid:{patient.PatientId}\\pna:{patient.FullName}\\dob:0000{patient.DOB.Month:00}{patient.DOB.Day:00}00000000\\alrgy:{allergies}\\mrn:{patient.MRN}");
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
        public void WriteToFiles()
        {
            if(Patients != null && Patients.Any())
                File.WriteAllLines(@"Patients.txt", Patients);
            else
            {
                if (File.Exists(@"Patients.txt")) File.Delete(@"Patients.txt");
            }
            if (MedicationOrders != null && MedicationOrders.Any())
                File.WriteAllLines(@"MedicationOrders.txt", MedicationOrders);
            else
            {
                if (File.Exists(@"MedicationOrders.txt")) File.Delete(@"MedicationOrders.txt");
            }
            if (NotInFormulary != null && NotInFormulary.Any())
                File.WriteAllLines(@"Items.txt", NotInFormulary);
            else
            {
                if (File.Exists(@"Items.txt")) File.Delete(@"Items.txt");
            }
        }
        public List<string> Patients { get; set; }
        public List<string> MedicationOrders { get; set; }
        public List<string> NotInFormulary { get; set; }
    }
}
