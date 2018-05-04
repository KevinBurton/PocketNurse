using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    public class PatientAllergy
    {
        [Key]
        public int PatientAllergyId { get; set; }
        public Allergy Allergy { get; set; }
        public Patient Patient { get; set; }
    }
}
