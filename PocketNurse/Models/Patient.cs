using System;
using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    public class Patient
    {
        [Key]
        public string PatientId { get; set; }
        [Required]
        public int CabinetId { get; set; }
        public Cabinet Cabinet { get; set; }
        public string MRN { get; set; }
        [StringLength(128)]
        public string First { get; set; }
        [StringLength(128)]
        public string Last { get; set; }
        [StringLength(255)]
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
    }
}
