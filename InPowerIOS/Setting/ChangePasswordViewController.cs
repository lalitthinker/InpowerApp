using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using BigTed;
using PCL.Service;
using PCL.Model;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Setting
{
    public partial class ChangePasswordViewController : UIViewController
    {

        public ChangePasswordViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            Title = "Change Password";
            txtUserOldPassword.BecomeFirstResponder();
        }


        partial void BtnSave_TouchUpInside(UIButton sender)
        {
            UserChangePassword();
        }

        partial void BtnReset_TouchUpInside(UIButton sender)
        {
            clearAll();
        }

        private void UserChangePassword()
        {

            try
            {
                if (txtUserOldPassword.Text != "")
                {
                    if (txtUserNewPassword.Text != "")
                    {
                        if (txtUserNewConfirmPassword.Text != "")
                        {
                            if (txtUserNewPassword.Text == txtUserNewConfirmPassword.Text)
                            {
                                var RquestChangePasswordViewModel = new ChangePasswordViewModel
                                {
                                    OldPassword = txtUserOldPassword.Text,
                                    NewPassword = txtUserNewPassword.Text,
                                    ConfirmPassword = txtUserNewConfirmPassword.Text,
                                };

                                BTProgressHUD.Show("Please Wait", maskType: ProgressHUD.MaskType.Black);
                                var result = new SettingService().PostChangePasswordInterest(RquestChangePasswordViewModel);


                                if (result != null)
                                {
                                    new UIAlertView("Change Password", "password change succesfull", null, "OK", null).Show();
                                    BTProgressHUD.Dismiss();
                                    clearAll();
                                    this.DismissViewController(true, null);
                                
                                }
                                else
                                {
                                    new UIAlertView("Change Password", "wrong old password", null, "OK", null).Show();

                                }
                        }
                        else
                        {
                                txtUserNewConfirmPassword.BecomeFirstResponder();
                                new UIAlertView("Change Password", "Your password and confirmation password do not match", null, "OK", null).Show();
                        }
                    }
                    else
                    {
                            txtUserNewConfirmPassword.BecomeFirstResponder();
                            new UIAlertView("Change Password", "Please Enter Confirm Password First", null, "OK", null).Show();
                    }
                }
                else
                {
                        txtUserNewPassword.BecomeFirstResponder();
                        new UIAlertView("Change Password", "Please Enter New Password First", null, "OK", null).Show();
                }
            }
                else
                {
                    txtUserOldPassword.BecomeFirstResponder();
                    new UIAlertView("Change Password", "Please Enter Old Password  First", null, "OK", null).Show();
            }


        }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                BTProgressHUD.Dismiss();
                string ErrorMsg = ex.ToString();
                new UIAlertView("Change Password", ErrorMsg, null, "OK", null).Show();
            }
        }

        public void clearAll()
        {
            txtUserOldPassword.Text = "";
            txtUserNewPassword.Text = "";
            txtUserNewConfirmPassword.Text = ""; 
            txtUserOldPassword.BecomeFirstResponder();
        }

    }
}