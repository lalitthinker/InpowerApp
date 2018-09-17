using Foundation;
using System;
using UIKit;
using InPowerIOS.Common;
using SDWebImage;
using InPowerIOS.Models;
using BigTed;
using MessageUI;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Setting
{
    public partial class InviteFriendsViaSMSTableViewCell : UITableViewCell
    {
        PhoneContactIOSModel _PhoneContactIOSModel = new PhoneContactIOSModel();

        public InviteFriendsViaSMSTableViewCell (IntPtr handle) : base (handle)
        {
        }



        public void SendInvites()
        {
            try
            {
                var lstContactNumbers = new List<string>();
                lstContactNumbers.Add(_PhoneContactIOSModel.PhoneNumbers);
                BTProgressHUD.Show("Sending Invite", maskType: ProgressHUD.MaskType.Black);
              
                if (lstContactNumbers.Count == 0)
                {
                    new UIAlertView("Alert", "Contact Number Not Available", null, "OK", null).Show();
                    return;
                }

                var smsController = new MFMessageComposeViewController();
              
                if (MFMessageComposeViewController.CanSendText)
                {
                    
                    smsController.Body = "Hey! I just installed ImPower, with  messaging & all of my favorite Book Intrest on one app. Download it now at www.ImPower.com";
                    smsController.Recipients = lstContactNumbers.ToArray();
                    smsController.Finished += (object sender, MFMessageComposeResultEventArgs e) =>
                    {

                        if (e.Result == MessageComposeResult.Sent)
                        {
                            new UIAlertView("Alert", "Invite has been sent", null, "OK", null).Show();
                        }
                        else
                        {
                            new UIAlertView("Alert", e.Result.ToString(), null, "OK", null).Show();
                        }
                    };

                 
                }
             

                BTProgressHUD.Dismiss();
                BTProgressHUD.ShowSuccessWithStatus("Invite has been sent", 2000);

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                CommonHelper.PrintException("Err sending social invite", ex);
                BTProgressHUD.Dismiss();
                BTProgressHUD.ShowErrorWithStatus("Unable to send Invites.", 1500);
            }
        }

        public void UpdateCell(PhoneContactIOSModel phoneContact, int row)
        {
            _PhoneContactIOSModel = phoneContact;
            if (_PhoneContactIOSModel != null)
            {
                CommonHelper.SetCircularImage(ivPhoneContact);
                lblContactPersonName.Text = _PhoneContactIOSModel.FullName;
                lblContactNo.Text = _PhoneContactIOSModel.PhoneNumbers;
                //ivPhoneContact.Image = new UIImage("default_profile.png");
                if (_PhoneContactIOSModel.ThumbnailImageData != null)
                {
                    ivPhoneContact.SetImage(new NSUrl(_PhoneContactIOSModel.ThumbnailImageData.ToString()), UIImage.FromBundle("default_profile.png"));
                }
                else
                {
                    ivPhoneContact.Image = new UIImage("default_profile.png");
                }

                btnInvite.TouchUpInside += BtnInvite_TouchUpInside;
            }
        }

        public void BtnInvite_TouchUpInside(object sender, EventArgs e)
        {
            SendInvites();
        }

    }
} 