using MyCRM.Common;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCRM.Service.Common
{
    public interface IService
    {
        string Test();
        Task<Tuple<IEnumerable<UsersDomain>, List<ErrorMessage>>> GetAllUsers();
        IEnumerable<PisUsersDResetar> GetAllUsersDb();
        IEnumerable<Billing> GetUserBills(int userId);

        Task<Tuple<UsersDomain, List<ErrorMessage>>> GetUserDomainByUserId(int userId);
        Task<bool> AddUserAsync(PisUsersDResetar userDomain);
        Task<bool> IsValidUser(int id);
        List<int> GetBillingYears(int userId);
        Task<List<int>> GetDashboardData(int userId, int year);
        Task<List<int>> GetReport(int userId, int month_from, int year_from, int month_to, int year_to, string mode);
        PisUsersDResetar GetUser(int userId);
        Task<bool> PutUser(PisUsersDResetar user);
        Task<bool> AddUserDevice(UserDeviceModel device);
        Task<Tuple<UsersDomain, List<ErrorMessage>>> AuthenticateUser(string email, string password);
        Task<bool> UpdateTariffAndArchive(int month, int year, decimal price, int tariffId);
    }
}