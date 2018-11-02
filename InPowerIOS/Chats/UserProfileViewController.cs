using Foundation;
using System;
using UIKit;
using PCL.Model;
using InPowerIOS.Repositories;
using InPowerIOS.Common;
using SDWebImage;
using PCL.Service;
using BigTed;

namespace InPowerIOS.Chats
{
    public partial class UserProfileViewController : UIViewController
    {
        public ContactViewModel contactViewModel { get; set; }
        public UserProfileViewController (IntPtr handle) : base (handle)
        {
            this.contactViewModel = contactViewModel;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            lblBlockUser.UserInteractionEnabled = true;
            var selectlblBlockUserTapped = new UITapGestureRecognizer(() => { BlockUser();});

            lblBlockUser.AddGestureRecognizer(selectlblBlockUserTapped);


            lblReportUser.UserInteractionEnabled = true;
            var selectlblReportUserTapped = new UITapGestureRecognizer(() => { });
            lblReportUser.AddGestureRecognizer(selectlblReportUserTapped);

            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);
            getUserdetails();

        }

        private void BlockUser()
        {
            var alert = new UIAlertView("Block Contact", "Do you want to block this contact?", null, "No", "Yes");

            alert.Clicked += (object asender, UIButtonEventArgs e) =>
            {
                if (e.ButtonIndex == 1)
                {
                    BlockContact(contactViewModel.ContactId);
                }
                else
                    new UIAlertView("Block Contact", "Cancelled!", null, "OK", null).Show();
                alert.DismissWithClickedButtonIndex(0, true);
            };
            alert.Show();
        }

        public void getUserdetails()
        {
            CommonHelper.SetCircularImage(ivUserProfilePic);
            var ContactInfo = ContactRepository.GetContactbyUserId(Convert.ToInt64(contactViewModel.ContactId));
            //Title = ContactInfo.name;
            if (contactViewModel.IsBlock == true)
            {
                lblBlockUser.Text = "UnBlock";
            }
            else
            {
                lblBlockUser.Text = "Block";
            }

            lblEmail.Text =  ((ContactInfo.email == null) ? "" : ContactInfo.email);
            lblCityState.Text =  (((ContactInfo.city == null) ? "" : ContactInfo.city) + ((ContactInfo.state == null) ? "" : " / " + ContactInfo.state));
            lblAboutMe.Text =  ((ContactInfo.Aboutme == null) ? "" : ContactInfo.Aboutme);


            if (!string.IsNullOrEmpty(ContactInfo.contactPicUrl))
            {
                ivUserProfilePic.SetImage(new NSUrl(ContactInfo.contactPicUrl), UIImage.FromBundle("default_profile.png"));
            }
            else
            {
                ivUserProfilePic.Image = new UIImage("default_profile.png");
            }
        }


        public async void BlockContact(long id)
        {

            BTProgressHUD.Show("Block Contact", maskType: ProgressHUD.MaskType.Black);
            var model = new userdetails
            {
                BlockUserID = id
            };

            var result = await new SettingService().PostBlockUserInterest(model);
            if (result.Status == 1)
            {
                lblBlockUser.Text = "UnBlock";
                ChatConversationRepository.UpdateBlock(contactViewModel.ChatConvId);
                BTProgressHUD.Dismiss();
                new UIAlertView("Block Contact", result.Message, null, "OK", null).Show();
            }
            else
            {
                BTProgressHUD.Dismiss();
                new UIAlertView("Block Contact", result.Message, null, "OK", null).Show();
            }

            BTProgressHUD.Dismiss();
        }
    }
}