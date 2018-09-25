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
    [Register ("MenuListItemCell")]
    partial class MenuListItemCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_menuIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_MenuName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (img_menuIcon != null) {
                img_menuIcon.Dispose ();
                img_menuIcon = null;
            }

            if (lbl_MenuName != null) {
                lbl_MenuName.Dispose ();
                lbl_MenuName = null;
            }
        }
    }
}