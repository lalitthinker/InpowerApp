using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using InPowerIOS.Common;
using SDWebImage;

namespace InPowerIOS.Chats
{
    public partial class GroupMemberListTableViewCell : UITableViewCell
    {
        public GroupMemberListTableViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(GroupMember groupMember, int row)
        {
            if (groupMember != null)
            {
                CommonHelper.SetCircularImage(ivMemberProfilePic);
                lblMemberName.Text = groupMember.MemberName;
                if (!string.IsNullOrEmpty(groupMember.PictureUrl))
                {
                    ivMemberProfilePic.SetImage(new NSUrl(groupMember.PictureUrl), UIImage.FromBundle("default_profile.png"));
                }
                else
                {
                    ivMemberProfilePic.Image = new UIImage("default_profile.png");
                }
            }
        }
    }
}