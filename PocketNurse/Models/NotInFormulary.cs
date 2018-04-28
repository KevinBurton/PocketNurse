using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Models
{
    public class NotInFormulary
    {
        [Key]
        public int _id { get; set; }
        public string GenericName { get; set; }
        public string Alias { get; set; }
        public string Strength { get; set; }
        public string StrengthUnit { get; set; }
        public string Volume { get; set; }
        public string VolumeUnit { get; set; }
        public string TotalContainerVolume { get; set; }
        public string Route { get; set; }
    }
}
