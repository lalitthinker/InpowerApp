using Foundation; using System; using UIKit; using InPowerIOS.Common; using PCL.Model; using PCL.Service;
using Newtonsoft.Json;
using InPowerIOS.Model;
using InPowerIOS.Repositories;
using PCL.Common;
using InPowerIOS.Registration;
using Plugin.Connectivity;
using BigTed;
using InPowerIOS.SideBarMenu;
using Facebook.LoginKit;
using Facebook.CoreKit;
using InPowerIOS.Models;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Firebase.CloudMessaging;
using Firebase.InstanceID;

namespace InPowerIOS.Login {     public partial class LoginInpowerViewController : UITableViewController     {
                 public event EventHandler OnLoginSuccess;         public LoginInpowerViewController (IntPtr handle) : base (handle)         {             //continuetoMainScreen("Test", "Test", "Test");         }

        partial void BtnLogin_TouchUpInside(UIButton sender)
        {
            LoginProcess();
        } 
        #region LogIn with Email         
        private async void LoginProcess()
        {             var token =Messaging.SharedInstance.FcmToken;             try             {
                InpowerResult Result = null;
                if (ValidateData())
                {
                    BTProgressHUD.Show("LogIn", maskType: ProgressHUD.MaskType.Black);
                    Result = await new AccountService().Login(new UserLoginRequestViewModel
                    {
                        Email = txt_UserName.Text,
                        Password = txtPassword.Text,                         IOSToken = token
                    }, CommonConstant.AccountUrls.LoginServiceUrl);

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
                                    isShoutout = modelReporeg.isShoutout


                                };
                                UserProfileRepository.SaveUserProfile(profile);
;
                                ContinuetoMainScreen(modelReporeg.UserId.ToString(), modelReporeg.Password, ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey,
                                               modelReporeg.AWSSecretKey);


                            }

                        }
                        else
                        {
                            BTProgressHUD.Dismiss();
                            new UIAlertView("LogIn", Result.Message, null, "OK", null).Show();
                            return;
                        }
                    }
                }             }              catch (Exception ex)             {                 BTProgressHUD.Dismiss();             }            
        }
         public bool ValidateData()         {             bool validate = true;             if (string.IsNullOrEmpty(txtPassword.Text))                 validate = false;             if (string.IsNullOrEmpty(txt_UserName.Text))                 validate = false;             if (CrossConnectivity.Current.IsConnected)             {                 validate = true;             }             else             {                 new UIAlertView("LogIn", "You're not connected to a Network", null, "OK", null).Show();                  validate = false;             }                             return validate;         }

        #endregion LogIn with Email          public void FacebookLoginProcess(LoginButtonCompletedEventArgs e)         {             if (e.Error != null)             {                 Console.WriteLine(e.Error.ToString());                 new UIAlertView("Facebook",                     "We were not able to authenticate you with Facebook. Please login again.", null, "OK", null)                     .Show();             }             else if (e.Result.IsCancelled)             {                 Console.WriteLine("Result was cancelled");                 new UIAlertView("Facebook", "You cancelled the Facebook login process. Please login again.", null,                     "OK", null).Show();             }             else if (!e.Result.GrantedPermissions.ToString().Contains("email"))             {                 // Check that we have email as a permission, otherwise show that we can't signup                 Console.WriteLine("Email permission not found");                 new UIAlertView("Facebook", "Email permission is required to sign in, Please login again.", null,                     "OK", null).Show();              }             else             {                   var meRequest = new GraphRequest("/me", new NSDictionary("fields", "first_name,last_name,name,email,picture"), "GET");                 var requestConnection = new GraphRequestConnection();                 requestConnection.AddRequest(meRequest, (connection, meResult, meError) =>                 {                     var client = new WebClient();                     if (meError != null)                     {                         Console.WriteLine(meError.ToString());                         new UIAlertView("Facebook", "Unable to login to facebook.", null, "OK", null).Show();                         return;                     }                      var user = meResult as NSDictionary;                     var sc = new SocialLoginData();                     sc.scFirstName = user.ValueForKey(new NSString("first_name")).Description;                     sc.scLastName = user.ValueForKey(new NSString("last_name")).Description;                     sc.scUserName = user.ValueForKey(new NSString("name")).Description;                     sc.scSocialId = user.ValueForKey(new NSString("id")).Description;                     sc.scEmail = user.ValueForKey(new NSString("email")).Description;                     sc.scProfileImgUrl = ""; //user.ValueForKey(new NSString("picture")).Description;                     sc.scSource = "facebook";                     sc.scAccessUrl = "http://facebook.com/profile.php?id=" +                                      user.ValueForKey(new NSString("id")).Description;                     sc.scSocialOauthToken = AccessToken.CurrentAccessToken.TokenString;                     sc.scAccount = AccessToken.CurrentAccessToken.TokenString;                                      StartRegistration(sc);                 });                 requestConnection.Start();              }         }           public async void StartRegistration(SocialLoginData SocialData)         {             InpowerResult Result = null;             UserProfile userProfile = new UserProfile();             try             {                 string  isscEmail = "";                 if (SocialData.scEmail != "" || SocialData.scEmail != null) { isscEmail = SocialData.scEmail; }else{isscEmail = SocialData.scFirstName + SocialData.scLastName + SocialData.scSocialId + "@InPower.com";}                 Result = await new AccountService().Registration(new UserRegisterRequestViewModel                  {                     FirstName = SocialData.scFirstName,                     LastName = SocialData.scLastName,                     Email = isscEmail ,                     Password = "Inpower@8118"                 }, GlobalConstant.AccountUrls.RegisterServiceUrl);                                   var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());                 var ResultToken = await new AccountService().AccessToken(new TokenRequestViewModel                 {                     UserName = modelReporeg.Email,                     password = modelReporeg.Password,                     grant_type = "password"                  });                  if (Result.Message == "Detail Successfully Updated")                 {                     ContinuetoMainScreen(modelReporeg.UserId.ToString(), modelReporeg.Password, ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey,modelReporeg.AWSSecretKey);                 }                 else                 {                      var token = AccessToken.CurrentAccessToken != null;                 if (ResultToken != null)                 {                                   
                        userProfile = new UserProfile
                        {
                            ProfileImageUrl = "",//respose.JSONObject.GetJSONObject("picture").GetJSONObject("data").GetString("url"),                                      UserId = modelReporeg.UserId,                                     FirstName = modelReporeg.FirstName,                                     LastName = modelReporeg.LastName,                             Email = modelReporeg.Email,                                     Password = modelReporeg.Password,                                     AccessToken = ResultToken.access_token,                                     isActive = modelReporeg.isActive,                                     isShoutout = modelReporeg.isShoutout                                 };                                  //var pictureUrl = respose.JSONObject.GetJSONObject("picture").GetJSONObject("data").GetString("url");                                 //var pictureData = client.DownloadData(pictureUrl);                                     UserProfileRepository.SaveUserProfile(userProfile);                         CommonHelper.SetUserPreferences(modelReporeg.UserId.ToString(), modelReporeg.Password.ToLower(), ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey,                                  modelReporeg.AWSSecretKey);                                 GlobalConstant.AccessToken = ResultToken.access_token;                         new UIAlertView("Facebook Login", Result.Message, null, "OK", null).Show();                         this.DismissViewController(true,null);                                                          InvokeOnMainThread(delegate                       {                           UIStoryboard storyboard = this.Storyboard;                             PleaseComplateYourProfileViewController viewController = (PleaseComplateYourProfileViewController)                                 storyboard.InstantiateViewController("PleaseComplateYourProfileViewController");                             viewController.userProfile = userProfile;                           this.PresentViewController(viewController, true, null);                       });                              }                                  else                 {                         new UIAlertView("Facebook Login", Result.Message, null, "OK", null).Show();                         return;                 }             }             }             catch (Exception ex)             {                 Crashes.TrackError(ex);                 new UIAlertView("Facebook Login", ex.Message, null, "OK", null).Show();                 return;             }         }

        #region LogIn with Facebook 

        #endregion LogIn with Facebook
        void ContinuetoMainScreen(string UserId, string Password,string AccessToken, string Email,string AWSAccessKey, string AWSSecretKey)         {             BTProgressHUD.Dismiss();             GlobalConstant.AccessToken = AccessToken;             CommonHelper.SetUserPreferences(UserId,Password, AccessToken, Email, AWSAccessKey, AWSSecretKey);             var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;             var mainStoryboard = appDelegate.MainStoryboard;             var rootViewMainController = appDelegate.GetViewController(mainStoryboard, "RootViewController");             appDelegate.SetRootViewController(rootViewMainController, true);          }
         string[] extendedPermissions = new[] { "email", "public_profile" };//"user_about_me", "read_stream"         string[] publishPermissions = new[] { "publish_stream" };         public override void ViewDidLoad()         {             base.ViewDidLoad();              this.txt_UserName.ShouldReturn += (textField) =>             {                 textField.ResignFirstResponder();                 txtPassword.BecomeFirstResponder();                 return true;             };             this.txtPassword.ShouldReturn += (textField) =>             {                 textField.ResignFirstResponder();                 btnLogin.SendActionForControlEvents(UIControlEvent.TouchUpInside);                 return true;             };              var imageView = new UIImageView(UIImage.FromBundle("LoginScreen")); imageView.Frame = new CoreGraphics.CGRect(50, 50, imageView.Image.CGImage.Width, imageView.Image.CGImage.Height);              this.TableView.BackgroundView = imageView;              //btnLogInWithFacebook.ReadPermissions = extendedPermissions;             //btnLogInWithFacebook.LoginBehavior = LoginBehavior.SystemAccount;             //// Handle actions once the user is logged in             //btnLogInWithFacebook.Completed += (sender, e) => {             //    FacebookLoginProcess(e);             //};              //btnLogInWithFacebook.LoggedOut += (sender, e) => {              //};              var g = new UITapGestureRecognizer(() => View.EndEditing(false));             g.CancelsTouchesInView = false;             View.AddGestureRecognizer(g);         }     } } 