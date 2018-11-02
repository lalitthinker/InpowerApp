// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace InPowerIOS.SideBarMenu
{
    [Register ("SideMenuController")]
    partial class SideMenuController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView profileImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SideHeaderBackgroundView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SideMenuHeaderBackgroundImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tbl_MenuList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel userEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel userName { get; set; }

        [Action ("UIButton7616_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton7616_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (profileImage != null) {
                profileImage.Dispose ();
                profileImage = null;
            }

            if (SideHeaderBackgroundView != null) {
                SideHeaderBackgroundView.Dispose ();
                SideHeaderBackgroundView = null;
            }

            if (SideMenuHeaderBackgroundImage != null) {
                SideMenuHeaderBackgroundImage.Dispose ();
                SideMenuHeaderBackgroundImage = null;
            }

            if (tbl_MenuList != null) {
                tbl_MenuList.Dispose ();
                tbl_MenuList = null;
            }

            if (userEmail != null) {
                userEmail.Dispose ();
                userEmail = null;
            }

            if (userName != null) {
                userName.Dispose ();
                userName = null;
            }
        }
    }
}