using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyCRM.DAL.DataModel
{
    public partial class PisUsersDResetar
    {

        [Key]
        public int uId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public int? zipcode { get; set; }
        public string? country { get; set; }
        public string birthdate { get; set; }
        public int? tariffId { get; set; }
        public byte? admincheck { get; set; }
    }
}
