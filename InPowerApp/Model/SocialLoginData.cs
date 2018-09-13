using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace InPowerApp.Model
{
    public class SocialLoginData
    {
        public string scAccessUrl;
        public string scAccount;
        public string scBirthDay;
        public string scFirstName;
        public string scLastName;
        public string scProfileImgUrl;
        public string scSocialId;
        public string scSocialOauthToken;
        public string scSource;
        public string scUserName;
        public string scEmail;
    }

    public class SocialLoginViewModel
    {
        public string SocialNetwork { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
        public string SocialNetworkProfileId { get; set; }
        public string Account { get; set; }
    }
}