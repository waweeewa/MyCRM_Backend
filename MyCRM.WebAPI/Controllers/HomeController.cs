using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Common;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using MyCRM.Service;
using MyCRM.Service.Common;
using MyCRM.WebAPI.RESTModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MyCRM.WebAPI.Controllers
{
    [Route("pis")]
    public class HomeController : Controller
    {
        protected IService _service { get; private set; }
        private int _requestUserId;

        public HomeController(IService service)
        {
            _service = service;
            _requestUserId = -1;
        }
        [HttpGet]
        [Route("test")]
        public string Test()
        {
            return _service.Test();
        }
        [HttpGet]
        [Route("Users_db")]
        public IEnumerable<PisUsersDResetar> GetAllUsersDb()
        {
            IEnumerable<PisUsersDResetar> userDb = _service.GetAllUsersDb();

            return userDb;
        }
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetAllUsers()
        {

            HttpRequestResponse<IEnumerable<UsersDomain>> response = new HttpRequestResponse<IEnumerable<UsersDomain>>();

            Tuple<IEnumerable<UsersDomain>, List<ErrorMessage>> result = await _service.GetAllUsers();

            if (result != null)
            {
                response.Result = result.Item1;
                response.ErrorMessages = result.Item2;
                return Ok(response);
            }
            else
            {
                response.Result = result.Item1;
                response.ErrorMessages = result.Item2;
                return Ok(response);
            }


        }
        [HttpGet]
        [Route("Users/user_id/{userId}")]
        public async Task<IActionResult> GetUserDomainByUserId(int userId)
        {
            HttpRequestResponse<UsersDomain> response = new HttpRequestResponse<UsersDomain>();

            Tuple<UsersDomain, List<ErrorMessage>> result = await _service.GetUserDomainByUserId(userId);

            if (result != null)
            {
                response.Result = result.Item1; // Assign the single UsersDomain object directly
                response.ErrorMessages = result.Item2;
                return Ok(response);
            }
            else
            {
                // Handle the case where result is null, if needed
                return NotFound(); // Or another appropriate response
            }
        }
        [HttpPost]
        [Route("Users/add")]
        public async Task<IActionResult> AddUserAsync([FromBody] PisUsersDResetar userRest)
        {
            bool lastrequestId = await GetLastUserRequestId();

            if (!lastrequestId)
            {
                return BadRequest("Nije unesen RequestUserId korisnika koji poziva.");

            }
            else
            {

                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        PisUsersDResetar userDomain = new PisUsersDResetar();
                        userDomain.firstName = userRest.firstName;
                        userDomain.lastName = userRest.lastName;
                        userDomain.address = userRest.address;
                        userDomain.zipcode = userRest.zipcode;
                        userDomain.admincheck = userRest.admincheck;
                        userDomain.email = userRest.email;
                        userDomain.birthdate = userRest.birthdate;
                        userDomain.city = userRest.city;
                        userDomain.country = userRest.country;
                        userDomain.password = userRest.password;
                        userDomain.tariffId = userRest.tariffId;

                        bool add_user = await _service.AddUserAsync(userDomain);

                        if (add_user)
                        {

                            return Ok("User dodan!");
                        }
                        else
                        {
                            return Ok("User nije dodan!");
                        }
                    }

                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
                }
            }

        }
        [HttpGet]
        [Route("Users/{userId}/bills")]
        public async Task<IActionResult> GetUserBills(int userId)
        {
            HttpRequestResponse<IEnumerable<Billing>> response = new HttpRequestResponse<IEnumerable<Billing>>();

            IEnumerable<Billing> result = _service.GetUserBills(userId);

            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        #region AdditionalCustomFunctions
        public async Task<bool> GetLastUserRequestId()
        {
            IHeaderDictionary headers = this.Request.Headers;
            if (headers.ContainsKey("RequestUserId"))
            {
                if (int.TryParse(headers["RequestUserId"].ToString(), out _requestUserId))
                {
                    return await _service.IsValidUser(_requestUserId);
                    //return true;
                }
                else return false;
            }
            return false;

        }
        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
        {
            bool isAuthenticated = await _service.AuthenticateUser(credentials.email, credentials.password);

            if (isAuthenticated)
            {
                return Ok("Login successful!");
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }
        }
        #endregion AdditionalCustomFunctions
    }
}