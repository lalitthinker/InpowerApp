using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using PCL.Model;
using PCL.Service;
using PCL.Common;
using Newtonsoft.Json;
using InPowerApp.Repositories;
using InPowerApp.Model;
using InPowerApp.Common;
using Plugin.Connectivity;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

namespace InPowerApp.Activities
{
    [Activity(Label = "Welcome to InPower")]
    public class TellUsAboutYourselfActivity : AppCompatActivity
    {
        EditText txtFirstName, txtLastName, txtEmailAddress, txtPassword, txtConfirmPassword;
        Button btnContinue;
        LinearLayout hidekeybordlayout;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TellUsAboutYourselflayout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.WelcometoInPower);
            SupportActionBar.SetSubtitle(Resource.String.SignUpWithEmail);
            hidekeybordlayout = FindViewById<LinearLayout>(Resource.Id.hidekeybordlayout); 
            btnContinue = FindViewById<Button>(Resource.Id.btnContinue);
            txtFirstName = FindViewById<EditText>(Resource.Id.txtFirstName);
            txtLastName = FindViewById<EditText>(Resource.Id.txtLastName);
            txtEmailAddress = FindViewById<EditText>(Resource.Id.txtEmailAddress);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            txtConfirmPassword = FindViewById<EditText>(Resource.Id.txtConfirmPassword);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            Drawable iconEmail = Resources.GetDrawable(Resource.Drawable.email24);
            Drawable iconPassword = Resources.GetDrawable(Resource.Drawable.password24);
            Drawable iconUserName = Resources.GetDrawable(Resource.Drawable.username24);

            txtFirstName.SetCompoundDrawablesWithIntrinsicBounds(iconUserName, null, null, null);
            txtLastName.SetCompoundDrawablesWithIntrinsicBounds(iconUserName, null, null, null);
            txtEmailAddress.SetCompoundDrawablesWithIntrinsicBounds(iconEmail, null, null, null);
            txtPassword.SetCompoundDrawablesWithIntrinsicBounds(iconPassword, null, null, null);
            txtConfirmPassword.SetCompoundDrawablesWithIntrinsicBounds(iconPassword, null, null, null);
            btnContinue.Click += BtnContinue_Click;
            hidekeybordlayout.Touch += Hidekeybordlayout_Touch;
        }

        private void Hidekeybordlayout_Touch(object sender, View.TouchEventArgs e)
        {
            CommonHelper.Hidekeyboard(this, Window);
        }
        
        public void clearAll()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmailAddress.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
        }

        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

      
        private async void BtnContinue_Click(object sender, EventArgs e)
        {
            string inputemail = txtEmailAddress.Text.ToString();
            var emailvalidate = isValidEmail(inputemail);

            InpowerResult Result = null;
            try
            {
                Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
                icon_error.SetBounds(0, 0, 50, 50);

                if (txtFirstName.Text != "")
                {
                    if (txtLastName.Text != "")
                    {
                        if (txtEmailAddress.Text != "")
                        {
                          if (emailvalidate == true)
                            { 
                                if (txtPassword.Text != "")
                             {
                                if (txtConfirmPassword.Text != "")
                                {

                                    if (txtConfirmPassword.Text == txtPassword.Text)
                                    {
                                        if (CrossConnectivity.Current.IsConnected)
                                        {
                                            CommonHelper.ShowProgressBar(this);
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
                                                CommonHelper.SetUserPreferences(modelReporeg.UserId.ToString(),modelReporeg.Password.ToLower(), ResultToken.access_token, modelReporeg.Email, modelReporeg.AWSAccessKey,
                                                     modelReporeg.AWSSecretKey, null, null);
                                                        GlobalConstant.AccessToken = ResultToken.access_token;
                                             Toast.MakeText(this, Result.Message, ToastLength.Short).Show();
                                                clearAll();
                                                CommonHelper.DismissProgressBar(this);
                                                this.Finish();

                                                    var intent = new Intent(this, typeof(ComplateYourProfileActivity));
                                                    intent.AddFlags(ActivityFlags.SingleTop);
                                                   
                                                    StartActivity(intent);
                                                    
                                            }
                                        }

                                        else
                                        {
                                                CommonHelper.DismissProgressBar(this);
                                                Toast.MakeText(this, Result.Message, ToastLength.Short).Show();
                                            return;
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
                                        txtConfirmPassword.RequestFocus();
                                        txtConfirmPassword.SetError("Password Not Match", icon_error);
                                    }
                                }
                                else
                                {
                                    txtConfirmPassword.RequestFocus();
                                    txtConfirmPassword.SetError("Please Enter Confirm Password First", icon_error);
                                }
                            }
                            else
                            {
                                txtPassword.RequestFocus();
                                txtPassword.SetError("Please Enter Password First", icon_error);
                            }

                            }
                            else
                            {
                                txtEmailAddress.RequestFocus();
                                txtEmailAddress.SetError("Email is Not Valid", icon_error);
                            }

                        }
                        else
                        {
                            txtEmailAddress.RequestFocus();
                            txtEmailAddress.SetError("Please Enter Email Address First", icon_error);
                        }

                    }
                    else
                    {
                        txtLastName.RequestFocus();
                        txtLastName.SetError("Please Enter Last Name First", icon_error);
                    }
                }
                else
                {
                    txtFirstName.RequestFocus();
                    txtFirstName.SetError("Please Enter First Name First", icon_error);
                }


            }
            catch (Exception ex)
            {
                CommonHelper.DismissProgressBar(this);
                string ErrorMsg = ex.ToString();
                Toast.MakeText(this, ErrorMsg, ToastLength.Long).Show();
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                var intent = new Intent(this, typeof(LoginForm));
                StartActivity(intent);
            }
           
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(LoginForm));
            StartActivity(intent);
        }

    }
}