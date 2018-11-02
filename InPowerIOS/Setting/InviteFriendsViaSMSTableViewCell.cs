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
        UIViewController uIViewController;

        public InviteFriendsViaSMSTableViewCell (IntPtr handle) : base (handle)
        {
        }

        partial void BtnInvite_TouchUpInside(UIButton sender)
        {
            SendInvites();
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
                    smsController.Body = "Hey! I just installed InPower, with  messaging & all of my favorite Book Intrest on one app. Download it now at https://play.google.com/store/apps/details?id=thethiinker.inPower.app";
                    smsController.Recipients = lstContactNumbers.ToArray();
                    smsController.Finished += (object sender, MFMessageComposeResultEventArgs e) =>
                    {
                        if (e.Result == MessageComposeResult.Sent)
                        {
                            BTProgressHUD.ShowSuccessWithStatus("Invite has been sent", 2000);
                        }
                        else
                        {
                            new UIAlertView("Alert", e.Result.ToString(), null, "OK", null).Show();
                        }
                        this.uIViewController.DismissViewController(false,HandleAction);
                      
                    };
                    this.uIViewController.PresentViewController(smsController, true, null);
                }
                BTProgressHUD.Dismiss();
               
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                CommonHelper.PrintException("Err sending social invite", ex);
                BTProgressHUD.Dismiss();
                BTProgressHUD.ShowErrorWithStatus("Unable to send Invites.", 1500);
            }
        }

        void HandleAction()
        {
            
        }


        public void UpdateCell(PhoneContactIOSModel phoneContact, int row, UIViewController uiViewNew)
        {
            this.uIViewController = uiViewNew;
            _PhoneContactIOSModel = phoneContact;
            if (_PhoneContactIOSModel != null)
            {
                CommonHelper.SetCircularImage(ivPhoneContact);
                lblContactPersonName.Text = _PhoneContactIOSModel.FullName;
                lblContactNo.Text = _PhoneContactIOSModel.PhoneNumbers;
                //ivPhoneContact.Image = new UIImage("default_profile.png");
                if (_PhoneContactIOSModel.ThumbnailImageData != null)
                {
                    ivPhoneContact.Image = UIImage.LoadFromData(_PhoneContactIOSModel.ThumbnailImageData);//  .SetImage(UIImage.LoadFromData(_PhoneContactIOSModel.ThumbnailImageData), UIImage.FromBundle("default_profile.png"));
                }
                else
                {
                    ivPhoneContact.Image = new UIImage("default_profile.png");
                }

               // btnInvite.TouchUpInside += BtnInvite_TouchUpInside;
            }
        }

        public void BtnInvite_TouchUpInside(object sender, EventArgs e)
        {
           
        }

    }
} 