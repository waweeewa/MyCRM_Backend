using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.DAL.DataModel;

namespace MyCRM.Model
{
    public class InvoiceGET
    {
        public InvoiceGET() { }
        public InvoiceGET(PisUsersDResetar user, Billing billing, decimal totalAmount)
        {
            userId = user.uId;
            billId = billing.BId;
            email = user.email;
            paidAmount = billing.Paid;
            this.totalAmount = totalAmount;
            month = billing.Month;
            year = billing.Year;
            usedPower = billing.UsedPower;
            devices = new List<Device>();
            tariffId = billing.TarriffId;
        }

        public int userId { get; set; }
        public int billId { get; set; }
        public string email { get; set; }
        public decimal paidAmount { get; set; }
        public decimal totalAmount { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int usedPower { get; set; }
        public List<Device> devices { get; set; } // Property to store the devices
        public int tariffId { get; set; }

        public class Device
        {
            public int billingId { get; set; }
            public int deviceId { get; set; }
            public string Name { get; set; }
            public int PowerUsage { get; set; }
            // New property to store the raw current usage for future comparisons
            public int RawUsedPower { get; set; }
        }

    }
}
