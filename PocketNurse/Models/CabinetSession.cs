using System;
using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    public class CabinetSession
    {
        [Key]
        public string CabinetSessionId { get; set; }
        public DateTime TimeStamp { get; set; }
        public Cabinet Cabinet { get; set; }
    }
}
