using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public class TariffArchive
    {
        [Key]
        public int tAId { get; set; }
        public int tariffId { get; set; }
        public string month { get; set; } // now stored as text
        public string year { get; set; }  // now stored as text
        public decimal priceArch { get; set; }
    }
}
