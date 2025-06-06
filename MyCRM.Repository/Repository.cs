﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using MyCRM.Repository.Automapper;
using MyCRM.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using iTextSharp.text;
using static MyCRM.Model.BillExport;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace MyCRM.Repository
{
    public class Repository : IRepository
    {
        private readonly PIS_DbContext appDbContext;
        private IRepositoryMappingService _mapper;

        public Repository(PIS_DbContext appDbContext, IRepositoryMappingService mapper)
        {
            this.appDbContext = appDbContext;
            _mapper = mapper;
        }
        public string Test()
        {
            return "I am OK - Repository.";
        }

        public List<int> GetBillingYears(int userId)
        {
            IEnumerable<Billing> bills = appDbContext.Billings.Where(b => b.UserId == userId).ToList();
            List<int> years = new List<int>();
            foreach (Billing bill in bills)
            {
                years.Add(bill.Year);
            }
            return years;
        }

        public async Task<List<int>> GetDashboardData(int userId, int year)
        {
            List<int> monthlyUsedPower = new List<int>(12);
            for (int i = 0; i < 12; i++)
            {
                monthlyUsedPower.Add(0);
            }

            IEnumerable<Billing> billsDb = appDbContext.Billings.Where(b => b.UserId == userId).ToList();
            int previousMonthTotalUsedPower = 0;

            for (int month = 1; month <= 12; month++)
            {
                int totalUsedPower = 0;
                foreach (Billing bill in billsDb)
                {
                    if (bill.Month == month && bill.Year == year)
                    {
                            totalUsedPower += bill.UsedPower;
                    }
                    if (bill.Month == 12 && bill.Year == year - 1 && month == 1)
                    {
                        Debug.WriteLine("Previous month: " + bill.UsedPower);
                        previousMonthTotalUsedPower += bill.UsedPower;
                    }
                }

                monthlyUsedPower[month - 1] = totalUsedPower;
            }
            List<int> powers = new List<int>(12);

            for (int i = 0; i < 12; i++)
            {
                if (i == 0)
                {
                    // First month, just add the value directly
                    powers.Add(monthlyUsedPower[i] - previousMonthTotalUsedPower);
                }
                else
                {
                    if (monthlyUsedPower[i] == 0)
                    {
                        powers.Add(0);
                    }
                    else
                    {
                        // Calculate the difference between the current month and the previous month
                        powers.Add(monthlyUsedPower[i] - monthlyUsedPower[i - 1]);
                    }
                }
            }

            return powers;
        }
        public async Task<List<int>> GetReport(int userId, int month_from, int year_from, int month_to, int year_to, string mode)
        {
            List<int> totalUsedPower = new List<int>();
            DateTime fromDate = new DateTime(year_from, month_from, 1);
            DateTime toDate = new DateTime(year_to, month_to, DateTime.DaysInMonth(year_to, month_to));

            if (mode.Equals("yearly", StringComparison.OrdinalIgnoreCase))
            {
                for (int year = fromDate.Year; year <= toDate.Year; year++)
                {
                    int totalYearConsumption = 0;
                    int start = (year == fromDate.Year) ? fromDate.Month : 1;
                    int end = (year == toDate.Year) ? toDate.Month : 12;
                    for (int month = start; month <= end; month++)
                    {
                        var currentBills = appDbContext.Billings
                            .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
                            .ToList();
                        int monthConsumption = 0;
                        foreach(var group in currentBills.GroupBy(b => b.DeviceId))
                        {
                            int currentPower = group.Sum(b => b.UsedPower);
                            if(currentPower > 0)
                            {
                                int prevYear = (month == 1) ? year - 1 : year;
                                int prevMonth = (month == 1) ? 12 : month - 1;
                                int previousPower = appDbContext.Billings
                                    .Where(b => b.UserId == userId && b.Year == prevYear && b.Month == prevMonth && b.DeviceId == group.Key)
                                    .Sum(b => b.UsedPower);
                                monthConsumption += Math.Max(0, currentPower - previousPower);
                            }
                        }
                        totalYearConsumption += monthConsumption;
                    }
                    totalUsedPower.Add(totalYearConsumption);
                }
            }
            else if (mode.Equals("monthly", StringComparison.OrdinalIgnoreCase))
            {
                for (int year = year_from; year <= year_to; year++)
                {
                    for (int month = 1; month <= 12; month++)
                    {
                        if ((year == year_from && month < month_from) || (year == year_to && month > month_to))
                        {
                            totalUsedPower.Add(0);
                            continue;
                        }
                        var currentBills = appDbContext.Billings
                            .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
                            .ToList();
                        int monthConsumption = 0;
                        foreach(var group in currentBills.GroupBy(b => b.DeviceId))
                        {
                            int currentPower = group.Sum(b => b.UsedPower);
                            if(currentPower > 0)
                            {
                                int prevYear = (month == 1) ? year - 1 : year;
                                int prevMonth = (month == 1) ? 12 : month - 1;
                                int previousPower = appDbContext.Billings
                                    .Where(b => b.UserId == userId && b.Year == prevYear && b.Month == prevMonth && b.DeviceId == group.Key)
                                    .Sum(b => b.UsedPower);
                                monthConsumption += Math.Max(0, currentPower - previousPower);
                            }
                        }
                        totalUsedPower.Add(monthConsumption);
                    }
                }
            }
            return totalUsedPower;
        }

        public async Task<List<decimal>> GetReportWithTariffArchive(int userId, int month_from, int year_from, int month_to, int year_to)
        {
            List<decimal> reportAmounts = new List<decimal>();
            // Build a list of (year, month) tuples across the range
            List<(int year, int month)> period = new List<(int, int)>();
            for (int year = year_from; year <= year_to; year++)
            {
                int startMonth = (year == year_from) ? month_from : 1;
                int endMonth = (year == year_to) ? month_to : 12;
                for (int month = startMonth; month <= endMonth; month++)
                {
                    period.Add((year, month));
                }
            }

            foreach (var (year, month) in period)
            {
                // Get current month's total used power
                var currentBills = appDbContext.Billings.Where(b => b.UserId == userId && b.Year == year && b.Month == month).ToList();
                int currentTotal = currentBills.Sum(b => b.UsedPower);

                // Get previous month's total used power
                int prevYear = (month == 1) ? year - 1 : year;
                int prevMonth = (month == 1) ? 12 : month - 1;
                var prevBills = appDbContext.Billings.Where(b => b.UserId == userId && b.Year == prevYear && b.Month == prevMonth).ToList();
                int prevTotal = prevBills.Sum(b => b.UsedPower);

                int consumption = currentTotal - prevTotal;
                if(consumption < 0)
                    consumption = 0;

                decimal applicablePrice = 0;
                if(currentBills.Any())
                {
                    int tariffId = currentBills.First().TarriffId;
                    // Convert text month and year to int for comparison
                    var archive = appDbContext.TariffArchives
                        .Where(a => a.tariffId == tariffId)
                        .AsEnumerable()
                        .Where(a => int.Parse(a.year) < year || (int.Parse(a.year) == year && int.Parse(a.month) <= month))
                        .OrderByDescending(a => int.Parse(a.year))
                        .ThenByDescending(a => int.Parse(a.month))
                        .FirstOrDefault();
                    if (archive != null)
                    {
                        applicablePrice = archive.priceArch;
                    }
                    else
                    {
                        // Fallback to the current tariff price
                        var tariff = appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId);
                        applicablePrice = tariff != null ? tariff.price : 0;
                    }
                }
                reportAmounts.Add(consumption * applicablePrice);
            }
            return reportAmounts;
        }

        public async Task<List<decimal>> GetReportWithTariffArchive(int userId, int month_from, int year_from, int month_to, int year_to, string mode)
        {
            List<decimal> reportAmounts = new List<decimal>();
            // Build a list of (year, month) tuples across the range
            List<(int year, int month)> period = new List<(int, int)>();
            for (int year = year_from; year <= year_to; year++)
            {
                int startMonth = (year == year_from) ? month_from : 1;
                int endMonth = (year == year_to) ? month_to : 12;
                for (int month = startMonth; month <= endMonth; month++)
                {
                    period.Add((year, month));
                }
            }
            if(mode.Equals("monthly", StringComparison.OrdinalIgnoreCase))
            {
                // Iterate each year and every month to fill missing months with 0
                for (int year = year_from; year <= year_to; year++)
                {
                    for (int month = 1; month <= 12; month++)
                    {
                        // For first year, skip months before month_from; for last year, skip months after month_to.
                        if ((year == year_from && month < month_from) || (year == year_to && month > month_to))
                        {
                            reportAmounts.Add(0);
                            continue;
                        }
                        var currentBills = appDbContext.Billings
                            .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
                            .ToList();
                        int monthConsumption = 0;
                        foreach(var group in currentBills.GroupBy(b => b.DeviceId))
                        {
                            int currentPower = group.Sum(b => b.UsedPower);
                            if(currentPower > 0)
                            {
                                int prevYear = (month == 1) ? year - 1 : year;
                                int prevMonth = (month == 1) ? 12 : month - 1;
                                int previousPower = appDbContext.Billings
                                    .Where(b => b.UserId == userId && b.Year == prevYear && b.Month == prevMonth && b.DeviceId == group.Key)
                                    .Sum(b => b.UsedPower);
                                monthConsumption += Math.Max(0, currentPower - previousPower);
                            }
                        }
                        
                        decimal applicablePrice = 0;
                        if(currentBills.Any())
                        {
                            int tariffId = currentBills.First().TarriffId;
                            var archive = appDbContext.TariffArchives
                                .Where(a => a.tariffId == tariffId)
                                .AsEnumerable()
                                .Where(a => int.Parse(a.year) < year || (int.Parse(a.year) == year && int.Parse(a.month) <= month))
                                .OrderByDescending(a => int.Parse(a.year))
                                .ThenByDescending(a => int.Parse(a.month))
                                .FirstOrDefault();
                            if (archive != null)
                            {
                                applicablePrice = archive.priceArch;
                            }
                            else
                            {
                                var tariff = appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId);
                                applicablePrice = tariff != null ? tariff.price : 0;
                            }
                        }
                        decimal amount = monthConsumption * applicablePrice;
                        reportAmounts.Add(amount);
                    }
                }
            }
            else if(mode.Equals("yearly", StringComparison.OrdinalIgnoreCase))
            {
                // For each year, go through all 12 months ensuring missing months yield 0 consumption.
                for (int year = year_from; year <= year_to; year++)
                {
                    decimal yearlyTotal = 0;
                    for (int month = 1; month <= 12; month++)
                    {
                        // For first year, skip months before month_from; for last year, skip months after month_to.
                        if ((year == year_from && month < month_from) || (year == year_to && month > month_to))
                        {
                            yearlyTotal += 0;
                            continue;
                        }
                        var currentBills = appDbContext.Billings
                            .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
                            .ToList();
                        int monthConsumption = 0;
                        foreach(var grp in currentBills.GroupBy(b => b.DeviceId))
                        {
                            int currentPower = grp.Sum(b => b.UsedPower);
                            if(currentPower > 0)
                            {
                                int prevYear = (month == 1) ? year - 1 : year;
                                int prevMonth = (month == 1) ? 12 : month - 1;
                                int previousPower = appDbContext.Billings
                                    .Where(b => b.UserId == userId && b.Year == prevYear && b.Month == prevMonth && b.DeviceId == grp.Key)
                                    .Sum(b => b.UsedPower);
                                monthConsumption += Math.Max(0, currentPower - previousPower);
                            }
                        }
                        
                        decimal applicablePrice = 0;
                        if(currentBills.Any())
                        {
                            int tariffId = currentBills.First().TarriffId;
                            var archive = appDbContext.TariffArchives
                                .Where(a => a.tariffId == tariffId)
                                .AsEnumerable()
                                .Where(a => int.Parse(a.year) < year || (int.Parse(a.year) == year && int.Parse(a.month) <= month))
                                .OrderByDescending(a => int.Parse(a.year))
                                .ThenByDescending(a => int.Parse(a.month))
                                .FirstOrDefault();
                            if (archive != null)
                            {
                                applicablePrice = archive.priceArch;
                            }
                            else
                            {
                                var tariff = appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId);
                                applicablePrice = tariff != null ? tariff.price : 0;
                            }
                        }
                        yearlyTotal += monthConsumption * applicablePrice;
                    }
                    reportAmounts.Add(yearlyTotal);
                }
            }
            return reportAmounts;
        }

        public IEnumerable<Billing> GetUserBills(int userId)
        {

            IEnumerable<Billing> bills = appDbContext.Billings.Where(b => b.UserId == userId).ToList();
            return bills;
        }

        public IEnumerable<UsersDomain> GetAllUsers()
        {
            IEnumerable<PisUsersDResetar> usersDb = appDbContext.PisUsersDResetar.ToList();

            IEnumerable<UsersDomain> usersDomain = _mapper.Map<IEnumerable<UsersDomain>>(usersDb);

            return usersDomain;
        }

        public IEnumerable<PisUsersDResetar> GetAllUsersDb()
        {
            IEnumerable<PisUsersDResetar> userDb = appDbContext.PisUsersDResetar.ToList();
            return userDb;
        }

        public UsersDomain GetUserDomainByUserId(int userId)
        {
            PisUsersDResetar userDb = appDbContext.PisUsersDResetar.Find(userId);

            UsersDomain user = _mapper.Map<UsersDomain>(userDb);

            return user;
        }
        public PisUsersDResetar GetUser(int userId)
        {
            PisUsersDResetar userDb = appDbContext.PisUsersDResetar.Find(userId);
            return userDb;
        }

        public async Task<bool> PutUser(PisUsersDResetar user)
        {
            var existingUser = await appDbContext.PisUsersDResetar.FindAsync(user.uId);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.firstName = user.firstName;
            existingUser.lastName = user.lastName;
            existingUser.email = user.email;
            existingUser.password = user.password;
            existingUser.city = user.city;
            existingUser.address = user.address;
            existingUser.zipcode = user.zipcode;
            existingUser.birthdate = user.birthdate;
            existingUser.country = user.country;
            existingUser.tariffId = user.tariffId;
            existingUser.admincheck = user.admincheck;

            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var existingUser = await appDbContext.PisUsersDResetar.FindAsync(userId);
            if (existingUser == null)
            {
                return false;
            }

            appDbContext.PisUsersDResetar.Remove(existingUser);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUserAsync(PisUsersDResetar userDomain)
        {
            try
            {
                if (userDomain.admincheck == null)
                {
                    userDomain.admincheck = 0;
                }
                var user_created = await appDbContext.PisUsersDResetar.AddAsync(userDomain);
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        //public async Task<bool> UpdateUserOibAsync(UsersDomain userDomain)
        //{
        //	try
        //	{
        //		PisUsersMmaturanec userDb =await appDbContext.PisUsersMmaturanec.Find(userDomain.UserId);

        //		if(userDb == null)
        //		{
        //		}
        //			return true;
        //	}
        //	catch(Exception ex)
        //	{
        //		return false;
        //	}
        //}

        public async Task<PisUsersDResetar> IsValidUser(int id)
        {
            PisUsersDResetar userDb = await appDbContext.PisUsersDResetar.FindAsync(id);
            return _mapper.Map<PisUsersDResetar>(userDb);

        }
        public async Task<InvoiceGET> GetInvoiceByMonth(int userId, int month, int year)
        {
            PisUsersDResetar userDb = await appDbContext.PisUsersDResetar.FindAsync(userId);
            IEnumerable<Billing> billsDb = appDbContext.Billings.Where(b => b.UserId == userId && b.Month == month && b.Year == year).ToList();
            int lastMonthUsedPower = 0;
            int currentMonthUsedPower = 0;
            decimal amountToBePaid = 0;
            decimal amountLeftToPay = 0;

            // Get the total used power for the previous month
            IEnumerable<Billing> lastMonthBills = appDbContext.Billings.Where(b => b.UserId == userId && b.Month == month - 1 && b.Year == year).ToList();
            foreach (var bill in lastMonthBills)
            {
                lastMonthUsedPower += bill.UsedPower;
            }

            // Get the total used power for the current month
            foreach (var bill in billsDb)
            {
                currentMonthUsedPower += bill.UsedPower;
            }

            // Replace tariffs[0].price lookup with direct tariff fetch using tariff id from the first bill
            int tariffId = billsDb.FirstOrDefault()?.TarriffId ?? 0; // Use tariff id from bill, or default to 0
            
            // New logic: Get the latest archive effective up to (year, month)
            var archive = appDbContext.TariffArchives
                            .Where(a => a.tariffId == tariffId)
                            .AsEnumerable()
                            .Where(a => (int.Parse(a.year) < year) ||
                                        (int.Parse(a.year) == year && int.Parse(a.month) <= month))
                            .OrderByDescending(a => int.Parse(a.year))
                            .ThenByDescending(a => int.Parse(a.month))
                            .FirstOrDefault();
            decimal tariffPrice = archive != null 
                                   ? archive.priceArch 
                                   : (appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId)?.price ?? 0m);
            
            // Calculate amount based on the determined tariff price
            amountToBePaid = (currentMonthUsedPower - lastMonthUsedPower) * tariffPrice;
            amountLeftToPay = amountToBePaid;

            // Add device details and reduce amount left to pay based on previous payments
            List<InvoiceGET.Device> deviceNames = new List<InvoiceGET.Device>();
            foreach (Billing bill in billsDb)
            {
                IEnumerable<UserDevice> devicesForBill = appDbContext.UserDevices.Where(d => d.udId == bill.DeviceId).ToList();
                foreach (UserDevice device in devicesForBill)
                {
                    InvoiceGET.Device deviceUsage = new InvoiceGET.Device
                    {
                        billingId = bill.BId,
                        deviceId = device.udId,
                        Name = device.name,
                        PowerUsage = bill.UsedPower
                    };
                    deviceNames.Add(deviceUsage);
                    amountLeftToPay -= bill.Paid;
                }
            }

            InvoiceGET invoiceGET = new InvoiceGET
            {
                userId = userDb.uId,
                email = userDb.email,
                paidAmount = amountLeftToPay,
                totalAmount = amountToBePaid,
                month = month,
                year = year,
                usedPower = currentMonthUsedPower - lastMonthUsedPower,
                devices = deviceNames,
                tariffId = tariffId
            };
            return invoiceGET;
        }
        public async Task<BillExport> BillExport(int userId, int month, int year, int logUserId) // signature modified
        {
            PisUsersDResetar userDb = await appDbContext.PisUsersDResetar.FindAsync(userId);
            IEnumerable<Billing> billsDb = appDbContext.Billings.Where(b => b.UserId == userId).ToList();
            List<Tariff> tariffs = appDbContext.Tariffs.ToList(); // Fetch all tariffs

            int totalLastMonthUsedPower = 0;
            // Calculate last month totals (unchanged)
            IEnumerable<Billing> billsForLastMonth = billsDb.Where(b => b.Month == (month == 1 ? 12 : month - 1) && b.Year == (month == 1 ? year - 1 : year));
            foreach (Billing bill in billsForLastMonth)
                totalLastMonthUsedPower += bill.UsedPower;

            // NEW: Calculate device-level usage differences
            int overallUsageDifference = 0;
            List<BillExport.Device> deviceNames = new List<BillExport.Device>();

            IEnumerable<Billing> billsForMonth = billsDb.Where(b => b.Month == month && b.Year == year);
            foreach (Billing bill in billsForMonth)
            {
                int previousMonth = (month == 1) ? 12 : month - 1;
                int previousYear = (month == 1) ? year - 1 : year;
                int previousUsage = appDbContext.Billings
                                    .Where(b => b.UserId == userId && b.DeviceId == bill.DeviceId && b.Month == previousMonth && b.Year == previousYear)
                                    .Sum(b => b.UsedPower);
                int deviceUsageDifference = Math.Max(0, bill.UsedPower - previousUsage);
                overallUsageDifference += deviceUsageDifference;

                // Get device details
                var deviceDetails = appDbContext.UserDevices.FirstOrDefault(d => d.udId == bill.DeviceId);
                if (deviceDetails != null)
                {
                    deviceNames.Add(new BillExport.Device { Name = deviceDetails.name, PowerUsage = deviceUsageDifference });
                }
            }

            decimal tariffRate = 0;
            decimal amountToBePaid = 0;
            if (billsForMonth.Any())
            {
                var firstBillOfMonth = billsForMonth.First();
                int tariffId = firstBillOfMonth.TarriffId;
                // NEW: Retrieve the tariff price from TariffArchive effective on or before the {year, month}.
                var archive = appDbContext.TariffArchives
                                .Where(a => a.tariffId == tariffId)
                                .AsEnumerable()
                                .Where(a => int.Parse(a.year) < year || (int.Parse(a.year) == year && int.Parse(a.month) <= month))
                                .OrderByDescending(a => int.Parse(a.year))
                                .ThenByDescending(a => int.Parse(a.month))
                                .FirstOrDefault();
                tariffRate = archive != null ? archive.priceArch : 0;
                amountToBePaid = overallUsageDifference * tariffRate;
            }

            // --- New logic: post BillingArchive records using logUserId ---
            foreach (var bill in billsForMonth)
            {
                var archiveRecord = new DAL.DataModel.BillingArchive
                {
                    billId = bill.BId,
                    logUserId = logUserId
                };
                await appDbContext.BillingArchive.AddAsync(archiveRecord);
            }
            await appDbContext.SaveChangesAsync();
            // --- End new logic ---

            Billing currentMonthBill = billsForMonth.FirstOrDefault();
            return new BillExport
            {
                UserId = userId,
                firstName = userDb.firstName,
                lastName = userDb.lastName,
                month = month,
                year = year,
                usedPower = overallUsageDifference,
                paidAmount = currentMonthBill != null ? amountToBePaid - currentMonthBill.Paid : 0,
                totalAmount = amountToBePaid,
                unitPrice = tariffRate,
                devices = deviceNames
            };
        }

        public async Task<bool> PutDevice(VerifiedDevice device)
        {
            var existingDevice = await appDbContext.VerifiedDevices.FindAsync(device.vdId);
            if (existingDevice == null)
            {
                return false;
            }

            existingDevice.model = device.model;
            existingDevice.serialNum = device.serialNum;

            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteDevice(VerifiedDevice device)
        {
            var existingDevice = await appDbContext.VerifiedDevices.FindAsync(device.vdId);
            if (existingDevice == null)
            {
                return false;
            }
            Debug.WriteLine(existingDevice);
            appDbContext.VerifiedDevices.Remove(existingDevice);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddDevice(VerifiedDevice device)
        {
            var user_created = await appDbContext.VerifiedDevices.AddAsync(device);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public IEnumerable<VerifiedDevice> GetDevices()
        {
            IEnumerable<VerifiedDevice> devices = appDbContext.VerifiedDevices.ToList();
            return devices;
        }
        public async Task<bool> UpdateTariffAndArchive(int month, int year, decimal price, int tariffId)
        {
            var tariff = await appDbContext.Tariffs.FindAsync(tariffId);
            if (tariff == null)
            {
                return false;
            }
            tariff.price = price;
            // Force client-side evaluation so that text vs nvarchar conversion is done later
            var archive = appDbContext.TariffArchives
                            .ToList()
                            .FirstOrDefault(a => a.tariffId == tariffId 
                                              && a.month == month.ToString() 
                                              && a.year == year.ToString());
            if (archive != null)
            {
                archive.priceArch = price;
            }
            else
            {
                archive = new DAL.DataModel.TariffArchive 
                { 
                    tariffId = tariffId, 
                    month = month.ToString(), 
                    year = year.ToString(), 
                    priceArch = price 
                };
                await appDbContext.TariffArchives.AddAsync(archive);
            }
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTariff(Tariff device)
        {
            var existingDevice = await appDbContext.Tariffs.FindAsync(device.tId);
            if (existingDevice == null)
            {
                return false;
            }
            Debug.WriteLine(existingDevice);
            appDbContext.Tariffs.Remove(existingDevice);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddTariff(Tariff device)
        {
            var user_created = await appDbContext.Tariffs.AddAsync(device);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public IEnumerable<Tariff> GetTariff()
        {
            IEnumerable<Tariff> devices = appDbContext.Tariffs.ToList();
            return devices;
        }
        public IEnumerable<Invoices> GetInvoices(int loguserId)
        {
            var usersDb = appDbContext.PisUsersDResetar.ToList();
            var billsDb = appDbContext.Billings.ToList();
            var tariffs = appDbContext.Tariffs.ToList();
            var archives = appDbContext.TariffArchives.ToList();

            List<Invoices> invoices = new List<Invoices>();

            foreach (var user in usersDb)
            {
                // Group bills for this user by Year and Month.
                var groups = billsDb
                    .Where(b => b.UserId == user.uId)
                    .GroupBy(b => new { b.Year, b.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month);

                foreach (var group in groups)
                {
                    int groupConsumption = 0;
                    decimal groupPaid = 0;
                    // Process each bill in this group.
                    foreach (var bill in group)
                    {
                        // Calculate previous month and year.
                        int prevMonth = (group.Key.Month == 1) ? 12 : group.Key.Month - 1;
                        int prevYear = (group.Key.Month == 1) ? group.Key.Year - 1 : group.Key.Year;
                        // Sum previous usage for same device.
                        int prevUsage = appDbContext.Billings
                            .Where(b => b.UserId == user.uId 
                                        && b.DeviceId == bill.DeviceId 
                                        && b.Month == prevMonth 
                                        && b.Year == prevYear)
                            .Sum(b => b.UsedPower);
                        // Consumption is the difference.
                        int consumption = Math.Max(0, bill.UsedPower - prevUsage);
                        groupConsumption += consumption;
                        groupPaid += bill.Paid;
                    }
                    // Retrieve effective tariff for group date using the tariffId from the first bill.
                    int tariffId = group.First().TarriffId;
                    var archive = archives
                        .Where(a => a.tariffId == tariffId)
                        .Where(a => int.Parse(a.year) < group.Key.Year ||
                                    (int.Parse(a.year) == group.Key.Year && int.Parse(a.month) <= group.Key.Month))
                        .OrderByDescending(a => int.Parse(a.year))
                        .ThenByDescending(a => int.Parse(a.month))
                        .FirstOrDefault();
                    decimal effectiveTariff = archive != null ?
                                              archive.priceArch :
                                              (tariffs.FirstOrDefault(t => t.tId == tariffId)?.price ?? 0m);
                    decimal totalAmount = groupConsumption * effectiveTariff;
                    // Determine isArchive: true if every bill in the group has a BillingArchive record with the given loguserId.
                    bool isArchive = group.All(b => appDbContext.BillingArchive.Any(a => a.billId == b.BId && a.logUserId == loguserId));
                    // Build invoice record.
                    invoices.Add(new Invoices
                    {
                        billId = 0,
                        userId = user.uId,
                        email = user.email,
                        month = group.Key.Month,
                        year = group.Key.Year,
                        usedPower = groupConsumption,
                        paidAmount = groupPaid,
                        totalAmount = totalAmount,
                        tariff = effectiveTariff,
                        isArchive = isArchive
                    });
                }
            }
            return invoices;
        }

        public async Task<bool> PostInvoice(Billing invoice, int tariffId)
        {
            try
            {
                Billing billing = invoice;

                await appDbContext.Billings.AddAsync(billing);
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> PutInvoice(Billing invoice)
        {
            Billing existingBilling = await appDbContext.Billings.FindAsync(invoice.BId);
            if (existingBilling == null)
            {
                return false;
            }

            existingBilling.UserId = invoice.UserId;
            existingBilling.Month = invoice.Month;
            existingBilling.Year = invoice.Year;
            existingBilling.UsedPower = invoice.UsedPower;
            existingBilling.Paid = invoice.Paid;
            existingBilling.DeviceId = invoice.DeviceId;


            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PutUserDevice(int deviceId, int day_from, int month_from, int year_from, int day_to, int month_to, int year_to)
        {
            UserDevice existingDevice = await appDbContext.UserDevices.FindAsync(deviceId);
            if (existingDevice == null)
            {
                return false;
            }
            existingDevice.from_date = new DateTime(year_from, month_from, day_from);
            existingDevice.to_date = new DateTime(year_to, month_to, day_to);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteInvoice(int userId, int month, int year)
        {
            var existingBillings = appDbContext.Billings.Where(b => b.UserId == userId && b.Month == month && b.Year == year).ToList();
            if (existingBillings.Count == 0)
            {
                return false;
            }

            appDbContext.Billings.RemoveRange(existingBillings);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUserDevice(UserDeviceModel device)
        {
            try
            {
                IEnumerable<PisUsersDResetar> userDb = appDbContext.PisUsersDResetar.ToList();
                int uid = userDb.FirstOrDefault(u => u.email == device.email).uId;
                UserDevice userDevice = new UserDevice
                {
                    userId = uid,
                    name = device.name,
                    from_date = DateTime.Parse(device.from_date),
                    to_date = DateTime.Parse(device.to_date)
                };
                var user_created = await appDbContext.UserDevices.AddAsync(userDevice);
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserDevice(int udId)
        {
            var existingDevice = await appDbContext.UserDevices.FindAsync(udId);
            if (existingDevice == null)
            {
                return false;
            }

            appDbContext.UserDevices.Remove(existingDevice);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public IEnumerable<UserDeviceModel> GetUserDevices()
        {
            IEnumerable<PisUsersDResetar> userDb = appDbContext.PisUsersDResetar.ToList();
            IEnumerable<UserDevice> userDevices = appDbContext.UserDevices.ToList();

            IEnumerable<UserDeviceModel> verifiedDevices = userDevices.Select(device =>
            {
                PisUsersDResetar user = userDb.FirstOrDefault(u => u.uId == device.userId);
                return new UserDeviceModel
                {
                    udId = device.udId,
                    userId = device.userId,
                    email = user?.email,
                    name = device.name,
                    from_date = device.from_date.ToString("yyyy-MM-dd"),
                    to_date = device.to_date.ToString("yyyy-MM-dd")
                };
            });

            return verifiedDevices;
        }

        public async Task<(int lastMonthConsumption, decimal lastMonthPrice, int currentYearConsumption, decimal currentYearTotalPrice, int lastYearConsumption, decimal lastYearTotalPrice, List<int> monthlyConsumption, int consumptionDiff, double winterAverage, double summerAverage, decimal medianLastYearConsumption, List<double> trendPrediction, string modMonthName)> GetElectricitySummaryExport(int userId)
        {
            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            int currentMonth = now.Month;
            int lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            int lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
            int lastYear = currentYear - 1;

            // Improved GetConsumption function that accounts for multiple devices
            int GetConsumption(int year, int month)
            {
                int totalConsumption = 0;
                // Get all bills for the current month
                var currentMonthBills = appDbContext.Billings
                    .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
                    .ToList();

                // For each device in the current month, find its previous month's reading and calculate difference
                foreach (var bill in currentMonthBills.GroupBy(b => b.DeviceId))
                {
                    int deviceId = bill.Key;
                    int currentPower = bill.Sum(b => b.UsedPower);
                    
                    // Get previous month
                    int prevMonth = month == 1 ? 12 : month - 1;
                    int prevYear = month == 1 ? year - 1 : year;
                    
                    // Get previous month's reading for this device
                    int previousPower = appDbContext.Billings
                        .Where(b => b.UserId == userId && b.DeviceId == deviceId && b.Year == prevYear && b.Month == prevMonth)
                        .Sum(b => b.UsedPower);
                    
                    // Add this device's consumption to the total
                    totalConsumption += Math.Max(0, currentPower - previousPower);
                }
                
                return totalConsumption;
            }

            int lmConsumption = GetConsumption(lastMonthYear, lastMonth);
            decimal lmPricePerUnit = 0;
            var lmBill = appDbContext.Billings
                .FirstOrDefault(b => b.UserId == userId && b.Year == lastMonthYear && b.Month == lastMonth);
            if (lmBill != null)
            {
                int tariffId = lmBill.TarriffId;
                var lmArchive = appDbContext.TariffArchives
                    .Where(a => a.tariffId == tariffId)
                    .AsEnumerable()
                    .FirstOrDefault(a => int.Parse(a.year) == lastMonthYear && int.Parse(a.month) == lastMonth);
                if (lmArchive != null)
                {
                    lmPricePerUnit = lmArchive.priceArch;
                }
                else
                {
                    var tariff = appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId);
                    lmPricePerUnit = tariff != null ? tariff.price : 0;
                }
            }
            decimal lmTotalPrice = lmConsumption * lmPricePerUnit;

            int currentYearConsumption = 0;
            decimal currentYearTotalPrice = 0;
            List<int> monthlyConsumption = new List<int>();
            for (int m = 1; m <= 12; m++)
            {
                int cons = GetConsumption(currentYear, m);
                monthlyConsumption.Add(cons);
                currentYearConsumption += cons;

                decimal pricePerUnit = 0;
                var bill = appDbContext.Billings.FirstOrDefault(b => b.UserId == userId && b.Year == currentYear && b.Month == m);
                if (bill != null)
                {
                    int tariffId = bill.TarriffId;
                    var archive = appDbContext.TariffArchives
                        .Where(a => a.tariffId == tariffId)
                        .AsEnumerable()
                        .FirstOrDefault(a => int.Parse(a.year) == currentYear && int.Parse(a.month) == m);
                    if (archive != null)
                    {
                        pricePerUnit = archive.priceArch;
                    }
                    else
                    {
                        var tariff = appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId);
                        pricePerUnit = tariff != null ? tariff.price : 0;
                    }
                }
                currentYearTotalPrice += cons * pricePerUnit;
            }

            int lastYearConsumption = 0;
            decimal lastYearTotalPrice = 0;
            List<int> lastYearConsumptions = new List<int>();
            for (int m = 1; m <= 12; m++)
            {
                int cons = GetConsumption(lastYear, m);
                lastYearConsumption += cons;
                lastYearConsumptions.Add(cons);

                decimal pricePerUnit = 0;
                var bill = appDbContext.Billings.FirstOrDefault(b => b.UserId == userId && b.Year == lastYear && b.Month == m);
                if (bill != null)
                {
                    int tariffId = bill.TarriffId;
                    var archive = appDbContext.TariffArchives
                        .Where(a => a.tariffId == tariffId)
                        .AsEnumerable()
                        .FirstOrDefault(a => int.Parse(a.year) == lastYear && int.Parse(a.month) == m);
                    if (archive != null)
                    {
                        pricePerUnit = archive.priceArch;
                    }
                    else
                    {
                        var tariff = appDbContext.Tariffs.FirstOrDefault(t => t.tId == tariffId);
                        pricePerUnit = tariff != null ? tariff.price : 0;
                    }
                }
                lastYearTotalPrice += cons * pricePerUnit;
            }

            int currentMonthConsumption = GetConsumption(currentYear, currentMonth);
            int consumptionDiff = currentMonthConsumption - lmConsumption;

            // Calculate averages for winter and summer months
            var winterMonths = lastYearConsumptions.Where((c, index) => index >= 9 || index <= 2);
            var summerMonths = lastYearConsumptions.Where((c, index) => index >= 3 && index <= 8);
            double winterAverage = winterMonths.Any() ? winterMonths.Average() : 0;
            double summerAverage = summerMonths.Any() ? summerMonths.Average() : 0;

            // Calculate median for last year's consumption
            decimal medianLastYearConsumption = lastYearConsumptions.OrderBy(c => c).Skip(lastYearConsumptions.Count() / 2).First();

            List<double> trendPrediction = await GetTrendPrediction(userId, currentYear);

            // --- MOD MONTH LOGIC ---
            // Get all billing data for the user
            var allBillings = await appDbContext.Billings
                .Where(b => b.UserId == userId)
                .ToListAsync();

            // Dictionary to store monthly consumption by year
            var consumptionByMonth = new Dictionary<int, List<double>>();
            for (int month = 1; month <= 12; month++)
                consumptionByMonth[month] = new List<double>();

            var billingsByYear = allBillings
                .GroupBy(b => b.Year)
                .OrderBy(g => g.Key);

            foreach (var yearGroup in billingsByYear)
            {
                int year = yearGroup.Key;
                for (int month = 1; month <= 12; month++)
                {
                    var monthlyBills = yearGroup.Where(b => b.Month == month).ToList();
                    if (!monthlyBills.Any())
                        continue;
                    double totalMonthConsumption = 0;
                    foreach (var deviceGroup in monthlyBills.GroupBy(b => b.DeviceId))
                    {
                        int deviceId = deviceGroup.Key;
                        double currentReading = deviceGroup.Sum(b => b.UsedPower);
                        int prevMonth = month == 1 ? 12 : month - 1;
                        int prevYear = month == 1 ? year - 1 : year;
                        double previousReading = allBillings
                            .Where(b => b.UserId == userId && b.DeviceId == deviceId &&
                                        b.Year == prevYear && b.Month == prevMonth)
                            .Sum(b => b.UsedPower);
                        totalMonthConsumption += Math.Max(0, currentReading - previousReading);
                    }
                    if (totalMonthConsumption > 0)
                        consumptionByMonth[month].Add(totalMonthConsumption);
                }
            }

            var monthlyAverages = new Dictionary<int, double>();
            for (int month = 1; month <= 12; month++)
            {
                var monthData = consumptionByMonth[month];
                monthlyAverages[month] = monthData.Any() ? monthData.Average() : 0;
            }
            int modMonth = monthlyAverages.OrderByDescending(kvp => kvp.Value).First().Key;
            string modMonthName = new DateTime(2000, modMonth, 1).ToString("MMMM");

            return (lmConsumption, lmTotalPrice, currentYearConsumption, currentYearTotalPrice, lastYearConsumption, lastYearTotalPrice, monthlyConsumption, consumptionDiff, winterAverage, summerAverage, medianLastYearConsumption, trendPrediction, modMonthName);
        }
        public async Task<List<double>> GetTrendPrediction(int userId, int year)
        {
            List<double> trendValues = new List<double>(12);
            
            // Check if we're predicting for a future year
            bool isFutureYear = year > DateTime.Now.Year;
            
            // Check if we already have bills for the selected year - fix client-side GroupBy error
            var billsInSelectedYear = await appDbContext.Billings
                .Where(b => b.UserId == userId && b.Year == year)
                .ToListAsync(); // First fetch the data to memory
            
            // Then perform the grouping in memory
            var existingBillsForYear = billsInSelectedYear
                .GroupBy(b => b.Month)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            // Get all previous years' billings for trend calculation
            var allBillings = await appDbContext.Billings
                .Where(b => b.UserId == userId && b.Year < year)
                .ToListAsync(); // Fetch to memory first

            if (!allBillings.Any())
            {
                // If no historical data, just return zeros
                for (int month = 1; month <= 12; month++)
                {
                    trendValues.Add(0);
                }
                return trendValues;
            }
            
            // Get range of years available in historical data
            int minYear = allBillings.Min(b => b.Year);
            int maxYear = allBillings.Max(b => b.Year);
            int yearRange = maxYear - minYear + 1;
            
            // Calculate monthly device-level consumption for each historical year and month
            var monthlyConsumptionByYear = new Dictionary<int, Dictionary<int, double>>();
            
            // Process each year separately
            for (int y = minYear; y <= maxYear; y++)
            {
                var yearData = new Dictionary<int, double>();
                monthlyConsumptionByYear[y] = yearData;
                
                // Process each month in this year
                for (int m = 1; m <= 12; m++)
                {
                    // Get bills for this year and month
                    var monthlyBills = allBillings.Where(b => b.Year == y && b.Month == m).ToList();
                    
                    if (!monthlyBills.Any())
                        continue;
                        
                    double totalMonthConsumption = 0;
                    
                    // Calculate consumption by device for this month
                    foreach (var deviceGroup in monthlyBills.GroupBy(b => b.DeviceId))
                    {
                        int deviceId = deviceGroup.Key;
                        double currentReading = deviceGroup.Sum(b => b.UsedPower);
                        
                        // Get previous month's reading for this device
                        int prevMonth = m == 1 ? 12 : m - 1;
                        int prevYear = m == 1 ? y - 1 : y;
                        
                        double previousReading = allBillings
                            .Where(b => b.UserId == userId && b.DeviceId == deviceId && 
                                  b.Year == prevYear && b.Month == prevMonth)
                            .Sum(b => b.UsedPower);
                        
                        // Calculate and add consumption for this device
                        totalMonthConsumption += Math.Max(0, currentReading - previousReading);
                    }
                    
                    // Store the total month consumption
                    yearData[m] = totalMonthConsumption;
                }
            }
            
            // Now build a time series of monthly aggregated consumption
            List<double> timeSeriesData = new List<double>();
            for (int y = minYear; y <= maxYear; y++)
            {
                if (monthlyConsumptionByYear.TryGetValue(y, out var yearData))
                {
                    for (int m = 1; m <= 12; m++)
                    {
                        if (yearData.TryGetValue(m, out double value))
                        {
                            timeSeriesData.Add(value);
                        }
                    }
                }
            }
            
            if (timeSeriesData.Count == 0)
            {
                // No valid consumption data found
                for (int month = 1; month <= 12; month++)
                {
                    trendValues.Add(0);
                }
                return trendValues;
            }
            
            // Apply linear regression to predict future values
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            int n = timeSeriesData.Count;

            for (int i = 0; i < n; i++)
            {
                double x = i + 1; // Time point
                double y = timeSeriesData[i]; // Consumption value
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;
            
            // Calculate historical averages by month for better predictions
            var monthlyAverages = new Dictionary<int, double>();
            for (int m = 1; m <= 12; m++)
            {
                List<double> valuesForMonth = new List<double>();
                
                foreach (var yearEntry in monthlyConsumptionByYear)
                {
                    if (yearEntry.Value.TryGetValue(m, out double value))
                        valuesForMonth.Add(value);
                }
                
                if (valuesForMonth.Count > 0)
                    monthlyAverages[m] = valuesForMonth.Average();
                else
                    monthlyAverages[m] = 0;
            }

            for (int month = 1; month <= 12; month++)
            {
                if (existingBillsForYear.ContainsKey(month))
                {
                    // Month already has bills, use 0 for prediction
                    trendValues.Add(0);
                }
                else
                {
                    // No bills yet, calculate prediction
                    double trendValue = intercept + slope * (n + month);
                    
                    // For future years like 2025, incorporate historical monthly average
                    if (isFutureYear)
                    {
                        double historicalAvg = monthlyAverages.TryGetValue(month, out double avg) ? avg : 0;
                        
                        if (yearRange > 0)
                        {
                            // Blend trend with historical average, giving more weight to historical data
                            // for a more stable prediction when we have multiple years of history
                            double blendedPrediction = (trendValue + historicalAvg * yearRange) / (yearRange + 1);
                            trendValues.Add(Math.Max(0, blendedPrediction));
                        }
                        else
                        {
                            trendValues.Add(Math.Max(0, trendValue));
                        }
                    }
                    else
                    {
                        trendValues.Add(Math.Max(0, trendValue));
                    }
                }
            }

            return trendValues;
        }

        public async Task<List<double>> GetMonthlyConsumption(int userId, int year)
        {
            List<double> monthlyConsumptions = new List<double>();
            var billings = await appDbContext.Billings
                .Where(b => b.UserId == userId && b.Year == year)
                .OrderBy(b => b.Month)
                .ToListAsync();

            for (int month = 1; month <= 12; month++)
            {
                var currentMonthBilling = billings.FirstOrDefault(b => b.Month == month);
                var lastMonthBilling = billings.FirstOrDefault(b => b.Month == month - 1);

                double currentConsumption = currentMonthBilling?.UsedPower ?? 0;
                double lastConsumption = lastMonthBilling?.UsedPower ?? 0;

                double monthlyConsumption = currentConsumption - lastConsumption;
                monthlyConsumptions.Add(monthlyConsumption);
            }

            return monthlyConsumptions;
        }

        public async Task<int> GetMonthWithHighestUsage(int userId)
        {
            // Get all billing data for the user
            var allBillings = await appDbContext.Billings
                .Where(b => b.UserId == userId)
                .ToListAsync();
            
            if (!allBillings.Any())
                return 0; // Return 0 if no data available
            
            // Dictionary to store monthly consumption by year
            var consumptionByMonth = new Dictionary<int, List<double>>();
            
            // Initialize lists for each month (1-12)
            for (int month = 1; month <= 12; month++)
            {
                consumptionByMonth[month] = new List<double>();
            }
            
            // Group bills by year and month for analysis
            var billingsByYear = allBillings
                .GroupBy(b => b.Year)
                .OrderBy(g => g.Key);
            
            // Calculate device-level consumption for each month in each year
            foreach (var yearGroup in billingsByYear)
            {
                int year = yearGroup.Key;
                
                for (int month = 1; month <= 12; month++)
                {
                    // Get bills for this month and year
                    var monthlyBills = yearGroup.Where(b => b.Month == month).ToList();
                    
                    if (!monthlyBills.Any())
                        continue;
                    
                    double totalMonthConsumption = 0;
                    
                    // Calculate consumption by device for this month
                    foreach (var deviceGroup in monthlyBills.GroupBy(b => b.DeviceId))
                    {
                        int deviceId = deviceGroup.Key;
                        double currentReading = deviceGroup.Sum(b => b.UsedPower);
                        
                        // Get previous month's reading for this device
                        int prevMonth = month == 1 ? 12 : month - 1;
                        int prevYear = month == 1 ? year - 1 : year;
                        
                        double previousReading = allBillings
                            .Where(b => b.UserId == userId && b.DeviceId == deviceId && 
                                   b.Year == prevYear && b.Month == prevMonth)
                            .Sum(b => b.UsedPower);
                        
                        // Add device consumption to month total
                        totalMonthConsumption += Math.Max(0, currentReading - previousReading);
                    }
                    
                    // Only add months with non-zero consumption
                    if (totalMonthConsumption > 0)
                        consumptionByMonth[month].Add(totalMonthConsumption);
                }
            }
            
            // Calculate average consumption for each month
            var monthlyAverages = new Dictionary<int, double>();
            
            for (int month = 1; month <= 12; month++)
            {
                var monthData = consumptionByMonth[month];
                if (monthData.Any())
                {
                    monthlyAverages[month] = monthData.Average();
                }
                else
                {
                    monthlyAverages[month] = 0;
                }
            }
            
            // Find month with highest average consumption
            int highestMonth = monthlyAverages
                .OrderByDescending(kvp => kvp.Value)
                .First().Key;
            
            return highestMonth;
        }
    }
}
