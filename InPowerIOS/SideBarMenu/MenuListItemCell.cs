using Foundation;
using System;
using UIKit;

namespace InPowerIOS.SideBarMenu
{
    public partial class MenuListItemCell : UITableViewCell
    {
        public MenuListItemCell(IntPtr handle) : base(handle)
        {
        }

        public void UpdateCell(string menuName, string menuImage, string hostedByImage, bool hide)
        {
            if (hide == false)
            {
                lbl_MenuName.Text = menuName;

                //        img_menuIcon.SetImage(new NSUrl(menuImage),
                //    UIImage.FromBundle("default_noimage.png")
                //);
                img_menuIcon.Image = UIImage.FromBundle(menuImage);

                //CALayer hostedByImageCircle = img_menuIcon.Layer;
                //hostedByImageCircle.CornerRadius = 26;
                //hostedByImageCircle.MasksToBounds = true;

            }



        }

    }
}