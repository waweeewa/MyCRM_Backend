using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public class VerifiedDevice
    {
        [Key]
        public int vdId { get; set; }
        public string model { get; set; }
        public string serialNum { get; set; }
    }
}
