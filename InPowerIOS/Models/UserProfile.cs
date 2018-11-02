using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace InPowerIOS.Model
{
     public class UserProfile
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public string ProfileImageUrl { get; set; }
        public string AboutMe { get; set; }
        public bool isActive { get; set; }
        public bool isShoutout { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string AndroidToken { get; set; }
        public string IOSToken { get; set; }
        
    }

}