using System;
namespace InPowerIOS.Models
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

    public class SocialConstants
    {
        //static NSArray* const fbPermissions = ["basic_info","read_friendlists","user_status","publish_stream","email","user_birthday","user_location","offline_access","publish_actions"];
        public const string appName = "InPower App";
        public const string fbClientId = "2072664406301614";
        public const string fbClientSecret = "306f530c8e3aedddfebbc7ba34662c16";
        public const string lPermissions = "r_emailaddress"; // New permissions

        public static readonly string[] fbPermissionsWrite = { "publish_actions" };

        public static readonly string[] fbPermissionsRead =
        {
            "email","public_profile","user_friends"
        };
        public static readonly string[] fbPermissionsReadTest =
      {

            "email"
        };
        public static readonly string[] fbPermissionsfeed = { "user_posts" };
    }
    }