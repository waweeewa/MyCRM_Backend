using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.DAL.DataModel;

namespace MyCRM.Model
{
    public class Invoices
    {
        public Invoices() { }
        public Invoices(PisUsersDResetar user, Billing billing, decimal totalAmount, string deviceId, decimal tariff, bool isArchive)
        {
            userId = user.uId;
            billId = billing.BId;
            email = user.email;
            paidAmount = billing.Paid;
            this.totalAmount = totalAmount;
            month = billing.Month;
            year = billing.Year;
            usedPower = billing.UsedPower;
            this.deviceId = deviceId;
            this.tariff = tariff;
            this.isArchive = isArchive;
        }

        public int userId { get; set; }
        public int billId { get; set; }
        public string email { get; set; }
        public decimal paidAmount { get; set; }
        public decimal totalAmount { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int usedPower { get; set; }
        public string deviceId { get; set; }
        public decimal tariff { get; set; }
        public bool isArchive { get; set; }
    }
}
