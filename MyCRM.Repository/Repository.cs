using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using MyCRM.Repository.Automapper;
using MyCRM.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public IEnumerable<Billing> GetUserBills(int userId)
        {

            IEnumerable<Billing> bills = appDbContext.Billings.Where(b => b.UserId == userId).ToList();
            return bills;
        }

        public IEnumerable<UsersDomain> GetAllUsers()
        {
            IEnumerable<PisUsersDResetar> usersDb = appDbContext.PisUsersDResetar.ToList();

            foreach (PisUsersDResetar user in usersDb)
            {
                Debug.WriteLine(user.email);
            }

            IEnumerable<UsersDomain> usersDomain = _mapper.Map<IEnumerable<UsersDomain>>(usersDb);

            Debug.WriteLine("Repository:");

            foreach (UsersDomain user in usersDomain)
            {
                Debug.WriteLine(user.UserEmail);
            }

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
        public async Task<bool> AddUserAsync(PisUsersDResetar userDomain)
        {
            try
            {
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

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            IEnumerable<PisUsersDResetar> usersDb = appDbContext.PisUsersDResetar.ToList();
            foreach (PisUsersDResetar user in usersDb)
            {
                if (user.email == email && user.password == password)
                {
                    return true;
                }
            }
            return false;
        }
    }
}