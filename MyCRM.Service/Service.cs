using MyCRM.Common;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using MyCRM.Repository.Common;
using MyCRM.Service.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCRM.Service
{
    public class Service : IService
    {
        IRepository _repository;

        public Service(IRepository repository)
        {
            _repository = repository;
        }
        public string Test()
        {
            return _repository.Test();
        }
        //public IEnumerable<UsersDomain> GetAllUsers()
        //{
        //	List<UsersDomain> users = _repository.GetAllUsers().ToList();

        //	if(users == null)
        //	{

        //		throw new Exception("no users found");
        //	}
        //	else
        //	{ 
        //		return users;

        //	}
        //}

        public IEnumerable<PisUsersDResetar> GetAllUsersDb()
        {
            IEnumerable<PisUsersDResetar> userDb = _repository.GetAllUsersDb();
            return userDb;
        }

        public async Task<bool> PutUser(PisUsersDResetar user)
        {
            return await _repository.PutUser(user);
        }

        public List<int> GetBillingYears(int userId)
        {
            List<int> years = _repository.GetBillingYears(userId)
                                        .OrderByDescending(y => y)
                                        .Distinct()
                                        .ToList();
            return years;
        }
        public Task<List<int>> GetDashboardData(int userId, int year)
        {
            Task<List<int>> data = _repository.GetDashboardData(userId, year);

            return data;
        }
        public Task<List<int>> GetReport(int userId, int month_from, int year_from, int month_to, int year_to, string mode)
        {
            Task<List<int>> data = _repository.GetReport(userId, month_from, year_from, month_to, year_to, mode);

            return data;
        }
        public IEnumerable<Billing> GetUserBills(int userId)
        {
            IEnumerable<Billing> bills = _repository.GetUserBills(userId);
            return bills;
        }

        public async Task<Tuple<UsersDomain, List<ErrorMessage>>> GetUserDomainByUserId(int userId)
        {
            //return _repository.GetUserDomainByUserId(userId);
            List<ErrorMessage> erorMessages = new List<ErrorMessage>();
            UsersDomain usersDomain = _repository.GetUserDomainByUserId(userId);


            if (usersDomain != null)
            {
                erorMessages.Add(new ErrorMessage("Podatci su uredu!"));
                erorMessages.Add(new ErrorMessage("Podatci su u ispravnom obliku!"));
            }
            else
            {
                erorMessages.Add(new ErrorMessage("Podatci nisu uredu!"));
                erorMessages.Add(new ErrorMessage("Podatci nisu u ispravnom obliku!"));
            }
            return new Tuple<UsersDomain, List<ErrorMessage>>(usersDomain, erorMessages);
        }

        public async Task<bool> AddUserAsync(PisUsersDResetar userDomain)
        {
            return await _repository.AddUserAsync(userDomain);
        }

        public async Task<Tuple<UsersDomain, List<ErrorMessage>>> AuthenticateUser(string email, string password)
        {
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            IEnumerable<UsersDomain> usersDb = _repository.GetAllUsers();
            foreach (UsersDomain user in usersDb)
            {
                Debug.WriteLine(user.UserApproved);
                if (user.UserEmail == email && user.Password == password)
                {
                    errorMessages.Add(new ErrorMessage("Successful login."));
                    return new Tuple<UsersDomain, List<ErrorMessage>>(user, errorMessages);
                }
            }
            errorMessages.Add(new ErrorMessage("Invalid email or password."));
            return new Tuple<UsersDomain, List<ErrorMessage>>(null, errorMessages);
        }
        #region AdditionalCustomFunctions
        public async Task<bool> IsValidUser(int id)
        {
            PisUsersDResetar user = await _repository.IsValidUser(id);
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<Tuple<IEnumerable<UsersDomain>, List<ErrorMessage>>> GetAllUsers()
        {
            List<ErrorMessage> erorMessages = new List<ErrorMessage>();
            IEnumerable<UsersDomain> usersDomain = _repository.GetAllUsers();

            if (usersDomain != null)
            {
                erorMessages.Add(new ErrorMessage("Podatci su uredu!"));
                erorMessages.Add(new ErrorMessage("Podatci su u ispravnom obliku!"));
            }
            else
            {
                erorMessages.Add(new ErrorMessage("Podatci nisu uredu!"));
                erorMessages.Add(new ErrorMessage("Podatci nisu u ispravnom obliku!"));
            }
            return new Tuple<IEnumerable<UsersDomain>, List<ErrorMessage>>(usersDomain, erorMessages);
        }
        public PisUsersDResetar GetUser(int userId)
        {
            return _repository.GetUser(userId);
        }
        public async Task<bool> AddUserDevice(UserDeviceModel device)
        {
                return await _repository.AddUserDevice(device);
        }

        public async Task<bool> UpdateTariffAndArchive(int month, int year, decimal price, int tariffId)
        {
            return await _repository.UpdateTariffAndArchive(month, year, price, tariffId);
        }
        #endregion AdditionalCustomFunctions
    }
}