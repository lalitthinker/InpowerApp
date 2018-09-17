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

namespace InPowerIOS.Setting
{
    [Register ("BlockedContactsTableViewCell")]
    partial class BlockedContactsTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnUnblock { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView contactPic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtSenderName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnUnblock != null) {
                btnUnblock.Dispose ();
                btnUnblock = null;
            }

            if (contactPic != null) {
                contactPic.Dispose ();
                contactPic = null;
            }

            if (txtSenderName != null) {
                txtSenderName.Dispose ();
                txtSenderName = null;
            }
        }
    }
}