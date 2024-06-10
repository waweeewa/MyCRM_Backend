using MyCRM.Common;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using MyCRM.Repository.Common;
using MyCRM.Service.Common;
using System;
using System.Collections.Generic;
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

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            IEnumerable<PisUsersDResetar> usersDb = _repository.GetAllUsersDb();
            foreach (PisUsersDResetar user in usersDb)
            {
                if (user.email == email && user.password == password)
                {
                    return true;
                }
            }
            return false;
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
        #endregion AdditionalCustomFunctions
    }
}