using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using InPowerIOS.Common;
using SDWebImage;
using PCL.Service;
using InPowerIOS.Repositories;
using BigTed;
using PCL.Model;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Setting
{
    public partial class BlockedContactsTableViewCell : UITableViewCell
    {
        ChatConversation chatConversation = new ChatConversation();
        public BlockedContactsTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public void UpdateCell(ChatConversation chatConversation)
        {
            this.chatConversation = chatConversation;
            if (chatConversation != null)
            {
                CommonHelper.SetCircularImage(contactPic);
                var BlockListContact = Repositories.ContactRepository.GetContactbyUserId(Convert.ToInt32(chatConversation.ChatId));


                if (BlockListContact != null)
                {
                    txtSenderName.Text = BlockListContact.screenName;

                    if (BlockListContact.contactPicUrl == "")
                    {
                        contactPic.SetImage(new NSUrl(BlockListContact.contactPicUrl), UIImage.FromBundle("default_profile.png"));
                    }
                    else
                    {
                        contactPic.Image = new UIImage("default_profile.png");
                    }

                    btnUnblock.TouchUpInside += BtnUnblock_TouchUpInside;

                }

               
            }

        }

        void BtnUnblock_TouchUpInside(object sender, EventArgs e1)
        {

            var alert = new UIAlertView("Blocked Contacts", "Do you want to UnBlock this user?", null, "No", "Yes");

            alert.Clicked += (object asender, UIButtonEventArgs e) =>
            {
                if (e.ButtonIndex == 1)
                {
                    RemoveFromBlackListAsync(chatConversation.ChatId);
                }
                else
                    new UIAlertView("Blocked Contacts", "Cancelled!", null, "OK", null).Show();
                alert.DismissWithClickedButtonIndex(0, true);
            };
            alert.Show();

        }


        private async Task RemoveFromBlackListAsync(long id)
        {
            try
            {
                BTProgressHUD.Show("UnBlocked Contacts", maskType: ProgressHUD.MaskType.Black);
                var model = new userdetails
                {
                    BlockUserID = id
                };

                var result = await new SettingService().PostUnBlockUserInterest(model);
                if (result.Status == 1)
                {

                    ChatConversationRepository.UpdateUnBlock(chatConversation.ChatId);
                    new UIAlertView("Blocked Contacts", result.Message, null, "OK", null).Show();
                    BTProgressHUD.Dismiss();
                }
                else
                {
                    new UIAlertView("Blocked Contacts", result.Message, null, "OK", null).Show();
                    BTProgressHUD.Dismiss();
                }

                BTProgressHUD.Dismiss();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                BTProgressHUD.Dismiss();
            }


        }
    }
}