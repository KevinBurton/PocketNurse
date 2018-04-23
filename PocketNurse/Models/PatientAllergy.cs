using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Models
{
    public class PatientAllergy
    {
        [Key]
        public Guid PatientAllergyId { get; set; }
        public Allergy Allergy { get; set; }
        public Patient Patient { get; set; }
    }
}
