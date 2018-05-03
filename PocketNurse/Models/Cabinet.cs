using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    public class Cabinet
    {
        [Key]
        public int CabinetId { get; set; }
        [StringLength(32)]
        public string State { get; set; }
        [StringLength(32)]
        public string Area { get; set; }
    }
}
