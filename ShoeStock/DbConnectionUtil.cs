using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStock
{
    public static class DbConnectionUtil
    {
        public static string ConString
        {
            get
            {
                string dbPath = Path.Combine(Path.GetFullPath(@"..\..\"), "ShoeStock.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dbPath};Initial Catalog=ShoeStock;Trusted_Connection=True";
            }
        }
    }
}
