using System;
using System.Collections.Generic;

namespace MyCRM.Models
{
    public partial class VerifiedDevices
    {
        public VerifiedDevices()
        {
            UserDevices = new HashSet<UserDevices>();
        }

        public int VdId { get; set; }
        public string Model { get; set; }
        public string SerialNum { get; set; }

        public virtual ICollection<UserDevices> UserDevices { get; set; }
    }
}
