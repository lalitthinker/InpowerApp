using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
    public class ContactViewModel
    {
        public long ContactId { get; set; }
        public long ChatId { get; set; }
        public long ChatConvId { get; set; }
        public bool IsBlock { get; set; }
        public string ProfileImageUrl { get; set; }
    }
    public class ContacSelectListViewModel
    {
        public string ProfileImageUrl { get; set; }
        public long ContactId { get; set; }
        public string ConatactName { get; set; }
        public bool ConatactCheck { get; set; }
        public bool isSelected()
        {
            return ConatactCheck;
        }
        public void setSelected(bool selected)
        {
            this.ConatactCheck = selected;
        }
    }
  
    public class UserRegisterRequestViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
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

    public class UserRegisterResponseViewModel
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int ProfileUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public string AboutMe { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public long? UserGroupId { get; set; }
        public bool isActive { get; set; }
        public bool isShoutout { get; set; }
        public string AndroidToken { get; set; }
        public string IOSToken { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
    }

    public class TokenRequestViewModel
    {
        public string UserName { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }
    public class TokenResponsetViewModel
    {
        public string access_token { get; set; }
       
    }
    public class UserLoginRequestViewModel
    {
       
        public string Email { get; set; }

      
        public string Password { get; set; }

     
        public bool RememberMe { get; set; }

        public string AndroidToken { get; set; }

        public string IOSToken { get; set; }
    }
    public class UserRequestViewModel
    {

        public int UserId { get; set; }


    }
}
