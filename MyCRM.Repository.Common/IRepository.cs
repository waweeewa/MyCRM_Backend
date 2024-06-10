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
        Task<bool> AuthenticateUser(string email, string password);
        string Test();
    }
}