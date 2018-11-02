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

namespace InPowerIOS.Chats
{
    [Register ("ContactListTableViewCell")]
    partial class ContactListTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblContactPersonName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfileImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblContactPersonName != null) {
                lblContactPersonName.Dispose ();
                lblContactPersonName = null;
            }

            if (ProfileImage != null) {
                ProfileImage.Dispose ();
                ProfileImage = null;
            }
        }
    }
}