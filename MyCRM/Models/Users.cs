using System;
using System.Collections.Generic;

namespace MyCRM.Models
{
    public partial class Users
    {
        public Users()
        {
            Billing = new HashSet<Billing>();
            UserDevices = new HashSet<UserDevices>();
        }

        public int UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int? Zipcode { get; set; }
        public string Birthdate { get; set; }
        public int? TariffId { get; set; }
        public byte? Admincheck { get; set; }
        public string Country { get; set; }

        public virtual Tariffs Tariff { get; set; }
        public virtual ICollection<Billing> Billing { get; set; }
        public virtual ICollection<UserDevices> UserDevices { get; set; }
    }
}
