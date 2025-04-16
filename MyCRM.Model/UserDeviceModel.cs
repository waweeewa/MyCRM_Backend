using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MyCRM.DAL.DataModel;

namespace MyCRM.Model
{
    public class UserDeviceModel
    {
        public UserDeviceModel() { }

        public UserDeviceModel(UserDevice device, PisUsersDResetar user)
        {
            udId = device.udId;
            userId = device.userId;
            email = user.email;
            name = device.name;
            from_date = device.from_date.ToString();
            to_date = device.to_date.ToString();
        }

        public int udId { get; set; }
        public int userId { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
    }
}
