using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStock.Models
{
    public class Stock
    {
        public int StockId { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int ShoeId { get; set; }
    }
}
