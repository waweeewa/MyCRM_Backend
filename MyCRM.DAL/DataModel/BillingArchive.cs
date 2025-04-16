using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public class BillingArchive
    {
        [Key]
        public int billArchId { get; set; }
        public int billId { get; set; }
        public int logUserId { get; set; }
    }
}
