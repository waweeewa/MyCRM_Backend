using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public class BillExport
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int paidAmount { get; set; }
        public int totalAmount { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int usedPower { get; set; }
    }
}
