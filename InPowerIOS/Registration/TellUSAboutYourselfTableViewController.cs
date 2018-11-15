using Foundation;
using System;
using UIKit;
using PCL.Model;
using InPowerIOS.Model;
using Newtonsoft.Json;
using PCL.Common;
using InPowerIOS.Common;
using Plugin.Connectivity;
using PCL.Service;
using InPowerIOS.Repositories;
using InPowerIOS.Login;
using System.Threading.Tasks;
using BigTed;
using Microsoft.AppCenter.Crashes;
namespace InPowerIOS.Registration
{
    public partial class TellUSAboutYourselfTableViewController : UITableViewController
    {
        public TellUSAboutYourselfTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.NavigationController.NavigationBar.BarTintColor = ColorExtensions.NavigationColor();//102, 50, 178
            this.NavigationController.NavigationBar.BackgroundColor = ColorExtensions.NavigationColor();//0,51,102 - dark blue 146, 30, 146 - red purplle
            this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };

            this.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                InvokeOnMainThread(delegate
                {
                    UIStoryboard storyboard = this.Storyboard;
                    LoginInpowerViewController viewController = (LoginInpowerViewController)
                        storyboard.InstantiateViewController("LoginInpowerViewController");
                    this.PresentViewController(viewController, true, null);
                });
            }), true);

            this.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem("Continue", UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                BBIContinue_Activated();
            }), true);

            Title = "Profile Details";

            //NavigationItem.SetHidesBackButton(true, false);

            //Title = "Tell US About Your self";
            txtFirstName.BecomeFirstResponder();

            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);
            //this.toolbarAboutyourself.TintColor = UIColor.White;
            //this.toolbarAboutyourself.BarTintColor = ColorExtensions.NavigationColor();//102, 50, 178
            //View.TintColor = UIColor.White;
            //this.toolbarAboutyourself.BackgroundColor = ColorExtensions.NavigationColor();//0,51,102 - dark blue 146, 30, 146 - red purplle
            //this.toolbarAboutyourself.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
        }


        public void clearAll()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmailAddress.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtFirstName.BecomeFirstResponder();
        }



        async public void BBIContinue_Activated()
        {
            InpowerResult Result = null;
            try
            {
                //Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
                //icon_error.SetBounds(0, 0, 50, 50);

                if (txtFirstName.Text != "")
                {
                    if (txtLastName.Text != "")
                    {
                        if (txtEmailAddress.Text != "")
                        {
                            if (txtPassword.Text != "")
                            {
                                if (txtConfirmPassword.Text != "")
                                {

                                    if (txtConfirmPassword.Text.Equals(txtConfirmPassword.Text))
                                    {
                                        if (CrossConnectivity.Current.IsConnected)
                                        {
                                            BTProgressHUD.Show("Please Wait", maskType: ProgressHUD.MaskType.Black);
                                            Result = await new AccountService().Registration(new UserRegisterRequestViewModel
                                            {
                                                Email = txtEmailAddress.Text,
                                                Password = txtPassword.Text,

                                                FirstName = txtFirstName.Text,
                                                LastName = txtLastName.Text

                                            }, GlobalConstant.AccountUrls.RegisterServiceUrl);

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
                                                        AccessToken = ResultToken.access_token,
                                                        isActive = modelReporeg.isActive,
                                                        isShoutout = modelReporeg.isShoutout


                                                    };
                                                    UserProfileRepository.SaveUserProfile(profile);
                                                    CommonHelper.SetUserPreferences(modelReporeg.UserId.ToString(), modelReporeg.Password, ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey, modelReporeg.AWSSecretKey);
                                                    GlobalConstant.AccessToken = ResultToken.access_token;
                                                    new UIAlertView("About You", Result.Message, null, "OK", null).Show();
                                                    clearAll();

                                                    BTProgressHUD.Dismiss();
                                                    //this.DismissViewController(true, null);
                                                    InvokeOnMainThread(delegate
                                                    {
                                                        UIStoryboard storyboard = this.Storyboard;
                                                        PleaseComplateYourProfileTableViewController viewController = (PleaseComplateYourProfileTableViewController)
                                                            storyboard.InstantiateViewController("PleaseComplateYourProfileTableViewController");
                                                        this.NavigationController.PushViewController(viewController, true);
                                                    });

                                                }
                                            }

                                            else
                                            {
                                                BTProgressHUD.Dismiss();

                                                new UIAlertView("About You", Result.Message, null, "OK", null).Show();
                                                return;
                                            }

                                        }
                                        else
                                        {

                                            new UIAlertView("About You", "You're not connected to a Network", null, "OK", null).Show();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        txtConfirmPassword.BecomeFirstResponder();
                                        new UIAlertView("About You", "Password Not Match", null, "OK", null).Show();
                                        return;
                                    }
                                }
                                else
                                {
                                    txtConfirmPassword.BecomeFirstResponder();
                                    new UIAlertView("About You", "Please Enter Confirm Password First", null, "OK", null).Show();
                                    return;
                                }
                            }
                            else
                            {
                                txtPassword.BecomeFirstResponder();
                                new UIAlertView("About You", "Please Enter Password First", null, "OK", null).Show();
                                return;
                            }
                        }
                        else
                        {
                            txtEmailAddress.BecomeFirstResponder();
                            new UIAlertView("About You", "Please Enter Email Address First", null, "OK", null).Show();
                            return;
                        }
                    }
                    else
                    {
                        txtLastName.BecomeFirstResponder();
                        new UIAlertView("About You", "Please Enter Last Name First", null, "OK", null).Show();
                        return;
                    }
                }
                else
                {
                    txtFirstName.BecomeFirstResponder();
                    new UIAlertView("About You", "Please Enter First Name First", null, "OK", null).Show();
                    return;
                }


            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                BTProgressHUD.Dismiss();
                string ErrorMsg = ex.ToString();
                new UIAlertView("Error", ErrorMsg, null, "OK", null).Show();
                return;
            }
        }
    }
}