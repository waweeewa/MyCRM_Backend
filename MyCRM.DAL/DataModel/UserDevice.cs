using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public class UserDevice
    {

        [Key]
        public int udId { get; set; }
        public int userId { get; set; }
        public string name { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
    }
}
