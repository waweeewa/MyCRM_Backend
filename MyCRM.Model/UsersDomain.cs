using MyCRM.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.Model
{
    public class UsersDomain
    {
        public UsersDomain()
        {

        }
        public UsersDomain(PisUsersDResetar user)
        {
            UserId = user.uId;
            UserName = user.firstName;
            UserSurname = user.lastName;
            UserEmail = user.email;
            UserApproved = user.admincheck;
        }

        public int UserId { get; set; }
        [Required(ErrorMessage = "Unesite ime korisnika.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Ime korisnika mora biti između 3 i 50 slova")]
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public byte? UserApproved { get; set; }
    }
}