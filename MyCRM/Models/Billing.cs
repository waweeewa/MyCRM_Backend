using System;
using System.Collections.Generic;

namespace MyCRM.Models
{
    public partial class Billing
    {
        public int BId { get; set; }
        public int? UserId { get; set; }
        public int? Paid { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? UsedPower { get; set; }

        public virtual Users User { get; set; }
    }
}
