using Foundation;
using System;
using UIKit;
using PCL.Model;
using InPowerIOS.Common;
using SDWebImage;

namespace InPowerIOS.Chats
{
    public partial class SelectGroupContactListTableViewCell : UITableViewCell
    {
        public SelectGroupContactListTableViewCell (IntPtr handle) : base (handle)
        {
            
        }

        public void UpdateCell(ContacSelectListViewModel contacSelectListViewModel, int row)
        {
            if (contacSelectListViewModel != null)
            {
                CommonHelper.SetCircularImage(ivContactImage);
                lblUserName.Text = contacSelectListViewModel.ConatactName;
                if (!string.IsNullOrEmpty(contacSelectListViewModel.ProfileImageUrl))
                {
                    ivContactImage.SetImage(new NSUrl(contacSelectListViewModel.ProfileImageUrl), UIImage.FromBundle("default_profile.png"));
                }
                else
                {
                    ivContactImage.Image = new UIImage("default_profile.png");
                }
             
               
            }
        }
    }
}