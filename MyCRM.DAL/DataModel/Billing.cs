using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    [Table("billing")]
    public class Billing
    {
        [Key]
        [Column("bId")]
        public int BId { get; set; }

        [Column("userId")]
        public int UserId { get; set; }

        [Column("paid")]
        public int Paid { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("usedPower")]
        public int UsedPower { get; set; }
    }

}
