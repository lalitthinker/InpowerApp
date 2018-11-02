using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using InPowerIOS.Common;
using SDWebImage;

namespace InPowerIOS.Chats
{
    public partial class ContactListTableViewCell : UITableViewCell
    {
        public ContactListTableViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(Contact contact, int position)
        {
            switch (position)
            {
                case 0:
                    lblContactPersonName.Text = "New Group";
                    ProfileImage.Image = new UIImage("addGroup.png");
                    break;

                default:
                    if (contact != null)
                    {
                        CommonHelper.SetCircularImage(ProfileImage);
                        lblContactPersonName.Text = contact.name;
                        if (!string.IsNullOrEmpty(contact.contactPicUrl))
                        {
                            ProfileImage.SetImage(new NSUrl(contact.contactPicUrl), UIImage.FromBundle("default_profile.png"));
                        }
                        else
                        {
                            ProfileImage.Image = new UIImage("default_profile.png");
                        }
                    }
                    break;
            }
        }
    }
}