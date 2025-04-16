using MyCRM.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;

namespace MyCRM.Model
{
    public class BillExport
    {
        public BillExport()
        {

        }
        public BillExport(PisUsersDResetar user, Billing billing)
        {
            firstName = user.firstName;
            lastName = user.lastName;
            paidAmount = billing.Paid;
            totalAmount = 0;
            month = billing.Month;
            year = billing.Year;
            usedPower = 0;
            unitPrice = 0;
            devices = new List<Device>(); // Initialize the devices collection
        }

        public int UserId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public decimal paidAmount { get; set; }
        public decimal totalAmount { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int usedPower { get; set; }
        public decimal unitPrice { get; set; }
        public List<Device> devices { get; set; } // Property to store the devices

        public class Device
        {
            public string Name { get; set; }
            public int PowerUsage { get; set; }
        }
    }
}
