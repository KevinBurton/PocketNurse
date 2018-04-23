using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Models
{
    public class Patient
    {
        [Key]
        public string PatientId { get; set; }
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
