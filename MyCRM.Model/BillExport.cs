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
            paidAmount = 0;
            totalAmount = 0;
            month = billing.Month;
            year = billing.Year;
            usedPower = billing.UsedPower;
        }

        public int UserId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int paidAmount { get; set; }
        public int totalAmount { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int usedPower { get; set; }
    }
}