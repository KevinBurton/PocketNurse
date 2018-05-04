using System;
using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    public class MedicationOrder
    {
        [Key]
        public int MedicationOrderId { get; set; }
        [StringLength(128)]
        public string PocketNurseItemId { get; set; }
        [StringLength(128)]
        public string MedicationName { get; set; }
        [StringLength(64)]
        public string Dose { get; set; }
        [StringLength(64)]
        public string Frequency { get; set; }
        [StringLength(64)]
        public string Route { get; set; }
        public Patient Patient { get; set; }
    }
}
