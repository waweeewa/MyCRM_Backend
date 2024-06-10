using System;
using System.Collections.Generic;

namespace MyCRM.Models
{
    public partial class UserDevices
    {
        public int UdId { get; set; }
        public int? UserId { get; set; }
        public int? DeviceId { get; set; }
        public string Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public virtual VerifiedDevices Device { get; set; }
        public virtual Users User { get; set; }
    }
}
