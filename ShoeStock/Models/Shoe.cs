using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStock.Models
{
    public class Shoe
    {
        public int ShoeId { get; set; }
        public string Model { get; set; }
        public DateTime FirstIntroducedOn { get; set; }
        public bool Active { get; set; } 
        public string Picture { get; set; }
    }
   
    
}
