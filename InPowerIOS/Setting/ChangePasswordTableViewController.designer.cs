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
    [Register ("ChangePasswordTableViewController")]
    partial class ChangePasswordTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnReset { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSave { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtUserNewConfirmPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtUserNewPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtUserOldPassword { get; set; }

        [Action ("BtnReset_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnReset_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnSave_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSave_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnReset != null) {
                btnReset.Dispose ();
                btnReset = null;
            }

            if (btnSave != null) {
                btnSave.Dispose ();
                btnSave = null;
            }

            if (txtUserNewConfirmPassword != null) {
                txtUserNewConfirmPassword.Dispose ();
                txtUserNewConfirmPassword = null;
            }

            if (txtUserNewPassword != null) {
                txtUserNewPassword.Dispose ();
                txtUserNewPassword = null;
            }

            if (txtUserOldPassword != null) {
                txtUserOldPassword.Dispose ();
                txtUserOldPassword = null;
            }
        }
    }
}