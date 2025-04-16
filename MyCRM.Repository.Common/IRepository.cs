using MyCRM.DAL.DataModel;
using MyCRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCRM.Repository.Common
{
    public interface IRepository
    {
        IEnumerable<UsersDomain> GetAllUsers();
        IEnumerable<PisUsersDResetar> GetAllUsersDb();
        IEnumerable<Billing> GetUserBills(int userId);
        UsersDomain GetUserDomainByUserId(int userId);
        Task<bool> AddUserAsync(PisUsersDResetar userDomain);
        Task<PisUsersDResetar> IsValidUser(int id);
        Task<BillExport> BillExport(int userId, int month, int year, int loguserId);
        List<int> GetBillingYears(int userId);
        Task<List<int>> GetDashboardData(int userId, int year);
        Task<List<int>> GetReport(int userId, int month_from, int year_from, int month_to, int year_to, string mode);
        Task<List<decimal>> GetReportWithTariffArchive(int userId, int month_from, int year_from, int month_to, int year_to, string mode);
        Task<bool> AddDevice(VerifiedDevice device);
        IEnumerable<VerifiedDevice> GetDevices();
        Task<bool> PutDevice(VerifiedDevice device);
        Task<bool> DeleteDevice(VerifiedDevice device);
        Task<bool> AddTariff(Tariff device);
        IEnumerable<Tariff> GetTariff();
        Task<bool> DeleteTariff(Tariff device);
        public IEnumerable<Invoices> GetInvoices(int loguserId);
        Task<bool> DeleteInvoice(int userId, int month, int year);
        Task<bool> PutInvoice(Billing invoice);
        Task<bool> PostInvoice(Billing invoice, int tariffId);
        Task<bool> AddUserDevice(UserDeviceModel device);
        Task<bool> DeleteUserDevice(int udId);
        PisUsersDResetar GetUser(int userId);
        Task<bool> PutUser(PisUsersDResetar user);
        Task<bool> DeleteUser(int userId);
        IEnumerable<UserDeviceModel> GetUserDevices();
        Task<InvoiceGET> GetInvoiceByMonth(int userId, int month, int year);
        string Test();
        Task<(int lastMonthConsumption, decimal lastMonthPrice, int currentYearConsumption, decimal currentYearTotalPrice, int lastYearConsumption, decimal lastYearTotalPrice, List<int> monthlyConsumption, int consumptionDiff)> GetElectricitySummaryExport(int userId);
        Task<bool> UpdateTariffAndArchive(int month, int year, decimal price, int tariffId);
        Task<bool> PutUserDevice(int deviceId, int day_from, int month_from, int year_from, int day_to, int month_to,int year_to);
    }
}