using Foundation;
using System;
using UIKit;
using InPowerIOS.Common;
using PCL.Service;
using BigTed;
using InPowerIOS.SideBarMenu;

namespace InPowerIOS.Setting
{
    public partial class SettingViewController : BaseController
    {
        public SettingViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            Title = "Setting";
        }

        partial void BtnDeleteAccount_TouchUpInside(UIButton sender)
        {
            var alert = new UIAlertView("Setting", "Do you want to delete this account?", null, "No", "Yes");

            alert.Clicked += (object asender, UIButtonEventArgs e) =>
            {
                if (e.ButtonIndex == 1)
                {
                    DeleteAccountAsync();
                }
                else
                    new UIAlertView("Setting", "Cancelled!", null, "OK", null).Show();
                    alert.DismissWithClickedButtonIndex(0, true);
            };
            alert.Show();
        }

        private void DeleteAccountAsync()
        {
            string VerifyPassword = null;
            UIAlertView alert = new UIAlertView();
            alert = new UIAlertView("Delete Account", "Enter your password", null, "No", "Yes");
            alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            alert.GetTextField(0).Placeholder = "Enter Password";

            alert.Clicked += async (object asender, UIButtonEventArgs e) =>
            {
                if (e.ButtonIndex == 1)
                {
                    BTProgressHUD.Show("Delete Account", maskType: ProgressHUD.MaskType.Black);
                    VerifyPassword = alert.GetTextField(0).Text;
                    if (VerifyPassword != "")
                    {
                        if (VerifyPassword != null && VerifyPassword == CommonHelper.PREF_Password())
                        {
                            var result = await new SettingService().PostDeleteAccountInterest(CommonHelper.GetUserId());

                            new UIAlertView("Delete Account", result.Message, null, "OK", null).Show();
                            BTProgressHUD.Dismiss();
                        }
                        else
                        {
                            new UIAlertView("Delete Account", "Enter your valid password!", null, "OK", null).Show();
                            BTProgressHUD.Dismiss();
                        }
                    }
                }
                else
                    new UIAlertView("Delete Account", "Cancelled!", null, "OK", null).Show();
                alert.DismissWithClickedButtonIndex(0, true);
            };
            alert.Show();
        }

        partial void BtnPrivacyPolicy_TouchUpInside(UIButton sender)
        {
            var tac_AND_PP_ViewController = (TAC_AND_PP_ViewController)Storyboard.InstantiateViewController("TAC_AND_PP_ViewController");
            tac_AND_PP_ViewController.isPrivacyPolicy = true;
            NavigationController.PushViewController(tac_AND_PP_ViewController, true);
        }

        partial void BtnTermsOfUse_TouchUpInside(UIButton sender)
        {
            var tac_AND_PP_ViewController = (TAC_AND_PP_ViewController)Storyboard.InstantiateViewController("TAC_AND_PP_ViewController");
            tac_AND_PP_ViewController.isPrivacyPolicy = false;
            NavigationController.PushViewController(tac_AND_PP_ViewController, true);
        }
    }
}