using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public class Tariff
    {
        [Key]
        public int tId { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
    }
}
