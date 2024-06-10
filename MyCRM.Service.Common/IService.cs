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
        Task<bool> AuthenticateUser(string email, string password);
    }
}