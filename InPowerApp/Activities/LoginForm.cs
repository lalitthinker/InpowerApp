
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Firebase;
using Firebase.Iid;
using InPowerApp.Common;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Newtonsoft.Json;
using Org.Json;
using PCL.Common;
using PCL.Model;
using PCL.Service;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using static InPowerApp.Activities.LoginForm;

namespace InPowerApp.Activities
{
    [Activity(Label = "Welcome to InPower")]
    public class LoginForm : Activity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
    {
        const int RC_SIGN_IN = 9001;
        protected const int REQUEST_LOCATION = 0x1;
        private ICallbackManager mFBCallManager;
        private MyProfileTracker mprofileTracker;
        Activity context;
        private ProfilePictureView mprofile;
        string email, facebookpicture;
        EditText txtLoginUserName, txtLoginUserPassword;
        LoginButton BtnFBLogin;
        Button btnLogInWithEmail /*BtnFBLogin*/;
        TextView lblOrRegisterHere;
        string InstanceToken = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FacebookSdk.SdkInitialize(this.ApplicationContext);
            mprofileTracker = new MyProfileTracker();
            mprofileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mprofileTracker.StartTracking();

            SetContentView(Resource.Layout.Loginfrm);
            FirebaseApp.InitializeApp(this);
            InstanceToken = FirebaseInstanceId.Instance.Token.ToString();
            BtnFBLogin = FindViewById<LoginButton>(Resource.Id.btnFacebook);
            btnLogInWithEmail = FindViewById<Button>(Resource.Id.btnLogin);
            lblOrRegisterHere = FindViewById<TextView>(Resource.Id.lblOrRegisterHere);
            txtLoginUserName = FindViewById<EditText>(Resource.Id.txtUsername);
            txtLoginUserPassword = FindViewById<EditText>(Resource.Id.txtUserPassword);

            btnLogInWithEmail.Click += BtnLogInWithEmail_Click;
            lblOrRegisterHere.Click += LblOrRegisterHere_Click;



            BtnFBLogin.SetReadPermissions(new List<string> { "user_friends", "public_profile", "email", "user_birthday" });
            mFBCallManager = CallbackManagerFactory.Create();
            BtnFBLogin.RegisterCallback(mFBCallManager, this);


            var requiredPermissions = new String[]
           {
                    Manifest.Permission.Internet,
                     Manifest.Permission.WriteExternalStorage,
                      Manifest.Permission.ReadExternalStorage,
                      Manifest.Permission.Camera,
                    Manifest.Permission.ReadContacts
           };
            ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
        }



        private void LblOrRegisterHere_Click(object sender, System.EventArgs e)
        {
            this.Finish();
            StartActivity(typeof(TellUsAboutYourselfActivity));
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            CommonHelper.Hidekeyboard(this, Window);
            return base.OnTouchEvent(e);
        }
        private async void BtnLogInWithEmail_Click(object sender, System.EventArgs e)
        {
            InpowerResult Result = null;
            Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
            icon_error.SetBounds(0, 0, 50, 50);
            try
            {
                if (txtLoginUserName.Text != "")
                {
                    if (txtLoginUserPassword.Text != "")
                    {
                        if (CrossConnectivity.Current.IsConnected)
                        {
                            CommonHelper.ShowProgressBar(this);

                            Result = await new AccountService().Login(new UserLoginRequestViewModel
                            {
                                Email = txtLoginUserName.Text,
                                Password = txtLoginUserPassword.Text,
                                AndroidToken = InstanceToken
                            }, GlobalConstant.AccountUrls.LoginServiceUrl);
                            if (Result != null)
                            {
                                if (Result.Status == 1)
                                {
                                    var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());
                                    var ResultToken = await new AccountService().AccessToken(new TokenRequestViewModel
                                    {

                                        UserName = modelReporeg.Email,
                                        password = modelReporeg.Password,
                                        grant_type = "password"

                                    });
                                    if (ResultToken != null)
                                    {
                                        var profile = new UserProfile
                                        {
                                            UserId = modelReporeg.UserId,
                                            Email = modelReporeg.Email,
                                            FirstName = modelReporeg.FirstName,
                                            LastName = modelReporeg.LastName,
                                            Password = modelReporeg.Password,
                                            ZipCode = modelReporeg.ZipCode,
                                            City = modelReporeg.City,
                                            State = modelReporeg.State,
                                            Country = modelReporeg.Country,
                                            ProfileImageUrl = modelReporeg.ProfileImageUrl,
                                            AboutMe = modelReporeg.AboutMe,
                                            AccessToken = ResultToken.access_token,
                                            isActive = modelReporeg.isActive,
                                            isShoutout = modelReporeg.isShoutout,
                                            AndroidToken = InstanceToken
                                        };


                                        UserProfileRepository.SaveUserProfile(profile);
                                        CommonHelper.SetUserPreferences(modelReporeg.UserId.ToString(), modelReporeg.Password.ToString(), ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey,
                                                       modelReporeg.AWSSecretKey, null, null);
                                        GlobalConstant.AccessToken = ResultToken.access_token;
                                        CommonHelper.DismissProgressBar(this);
                                        this.Finish();
                                        StartActivity(typeof(MainActivity));
                                    }
                                }
                                else
                                {
                                    CommonHelper.DismissProgressBar(this);
                                    Toast.MakeText(this, Result.Message, ToastLength.Short).Show();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "You're not connected to a Network", ToastLength.Short).Show();
                            return;
                        }
                    }
                    else
                    {
                        txtLoginUserPassword.RequestFocus();
                        txtLoginUserPassword.SetError("Password is not blank ", icon_error);
                    }
                }
                else
                {
                    txtLoginUserName.RequestFocus();
                    txtLoginUserName.SetError("Email Id is not blank ", icon_error);
                }
            }
            catch (Exception ex)
            {
                CommonHelper.DismissProgressBar(this);
            }

        }


        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Exit");
            alert.SetMessage("Do you want to Exit?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                Finish();
            });

            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
                alert.Dispose();
            });


            Dialog dialog = alert.Create();
            dialog.Show();

        }



        #region LogInWith Facebook


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        public void OnCancel() { }
        public void OnError(FacebookException p0) { }
        public void OnSuccess(Java.Lang.Object p0) { }













        void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            LoginManager.Instance.LogInWithReadPermissions(this, new List<string> { "user_friends", "public_profile" });
            if (e.mProfile != null)
            {
                try
                {
                    var sc = new SocialLoginData();
                    sc.scFirstName = e.mProfile.FirstName;
                    sc.scLastName = e.mProfile.LastName;
                    sc.scUserName = e.mProfile.FirstName + e.mProfile.LastName;
                    sc.scSocialId = e.mProfile.Id;
                    facebookpicture = "http://graph.facebook.com/" + e.mProfile.Id + "/picture?type=large";
                    sc.scProfileImgUrl = facebookpicture;
                    sc.scEmail = email ?? "";

                    sc.scSource = "facebook";
                    sc.scAccessUrl = "http://facebook.com/profile.php?id=" +
                                     e.mProfile.Id;

                    sc.scSocialOauthToken = AccessToken.CurrentAccessToken.Token;
                    sc.scAccount = AccessToken.CurrentAccessToken.UserId;


                    GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);

                    Bundle parameters = new Bundle();
                    parameters.PutString("fields", "id,name,email,picture");
                    request.Parameters = parameters;
                    request.ExecuteAsync();



                    StartRegistration(sc);

                }

                catch (Java.Lang.Exception ex) { }
            }


        }






        public async Task StartRegistration(SocialLoginData SocialData)
        {
            InpowerResult Result = null;

            try
            {

                Result = await new AccountService().Registration(new UserRegisterRequestViewModel

                {
                    FirstName = SocialData.scFirstName,
                    LastName = SocialData.scLastName,
                    Email = SocialData.scFirstName + SocialData.scLastName + SocialData.scSocialId + "@InPower.com",
                    Password = "Inpower@8118"
                }, GlobalConstant.AccountUrls.RegisterServiceUrl);



                var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());
                var ResultToken = await new AccountService().AccessToken(new TokenRequestViewModel
                {
                    UserName = modelReporeg.Email,
                    password = modelReporeg.Password,
                    grant_type = "password"

                });

                if (Result.Message == "Detail Successfully Updated")
                {
                    var token = AccessToken.CurrentAccessToken != null;
                    SocialData.scSocialOauthToken = AccessToken.CurrentAccessToken?.Token;
                    UserProfile loginUserDetails = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());
                    var intent = new Intent(this, typeof(MainActivity));
                    intent.AddFlags(ActivityFlags.SingleTop);
                    StartActivity(intent);
                    CommonHelper.SetUserPreferences(modelReporeg.UserId.ToString(), modelReporeg.Password.ToLower(), ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey,
                                    modelReporeg.AWSSecretKey, null, null);
                    GlobalConstant.AccessToken = ResultToken.access_token;
                    this.Finish();

                }
                else
                {

                    var token = AccessToken.CurrentAccessToken != null;
                    if (ResultToken != null)
                    {
                        if (token)
                        {
                            SocialData.scSocialOauthToken = AccessToken.CurrentAccessToken?.Token;
                            UserProfile userProfile;




                            userProfile = new UserProfile
                            {


                                Email = email ?? "",
                                ProfileImageUrl = facebookpicture ?? "",

                                UserId = modelReporeg.UserId,
                                FirstName = modelReporeg.FirstName,
                                LastName = modelReporeg.LastName,
                                Password = modelReporeg.Password,
                                AccessToken = ResultToken.access_token,
                                isActive = modelReporeg.isActive,
                                isShoutout = modelReporeg.isShoutout
                            };




                            UserProfileRepository.SaveUserProfile(userProfile);
                            CommonHelper.SetUserPreferences(modelReporeg.UserId.ToString(), modelReporeg.Password.ToLower(), ResultToken.access_token, userProfile.Email, modelReporeg.AWSAccessKey,
                                 modelReporeg.AWSSecretKey, null, null);
                            GlobalConstant.AccessToken = ResultToken.access_token;
                            Toast.MakeText(this, Result.Message, ToastLength.Short).Show();

                            // CommonHelper.DismissProgressBar(this);
                            //  this.Finish();
                            //var UserprofileRepo = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());
                            //if (modelReporeg.UserId==)
                            this.Finish();
                            var intent = new Intent(this, typeof(ComplateYourProfileActivity));
                            intent.PutExtra("UserProfile", JsonConvert.SerializeObject(userProfile));
                            intent.AddFlags(ActivityFlags.SingleTop);
                            StartActivity(intent);







                        }

                    }

                    else
                    {
                        //  CommonHelper.DismissProgressBar(this);
                        Toast.MakeText(this, Result.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //  CommonHelper.DismissProgressBar(this);
                string ErrorMsg = ex.ToString();
                Toast.MakeText(this, ErrorMsg, ToastLength.Long).Show();
            }
        }




















        public class MyProfileTracker : ProfileTracker
        {
            public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;
            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
            {
                if (mOnProfileChanged != null)
                {
                    mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(newProfile));
                }
            }
        }
        public class OnProfileChangedEventArgs : EventArgs
        {
            public Profile mProfile;
            public OnProfileChangedEventArgs(Profile profile)
            {
                mProfile = profile;
            }

        }

        private Bitmap GetImageBitmapFromUrl(String url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
                return imageBitmap;
            }
            catch (IOException e) { }
            return null;
        }

        public void OnCompleted(Org.Json.JSONObject json, GraphResponse response)
        {
            string data = json.ToString();

            email = json.GetString("email");

            var result = JsonConvert.DeserializeObject(data);



        }
        #endregion


    }


}