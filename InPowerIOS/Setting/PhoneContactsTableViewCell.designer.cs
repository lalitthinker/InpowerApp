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
    [Register ("InviteFriendsViaSMSTableViewCell")]
    partial class InviteFriendsViaSMSTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnInvite { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ivPhoneContact { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblContactNo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblContactPersonName { get; set; }

        [Action ("BtnInvite_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnInvite_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnInvite != null) {
                btnInvite.Dispose ();
                btnInvite = null;
            }

            if (ivPhoneContact != null) {
                ivPhoneContact.Dispose ();
                ivPhoneContact = null;
            }

            if (lblContactNo != null) {
                lblContactNo.Dispose ();
                lblContactNo = null;
            }

            if (lblContactPersonName != null) {
                lblContactPersonName.Dispose ();
                lblContactPersonName = null;
            }
        }
    }
}