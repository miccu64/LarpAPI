using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Coord
    {
        [Key]
        public string Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
