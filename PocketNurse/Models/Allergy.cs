using System;
using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    public class Allergy
    {
        [Key]
        public Guid AllergyId { get; set; }
        [StringLength(128)]
        public string AllergyName { get; set; }
    }
}
