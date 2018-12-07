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
    [Register ("SettingViewController")]
    partial class SettingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockedContacts { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnChangePassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnDeleteAccount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPrivacyPolicy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSilentMode { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTermsOfUse { get; set; }

        [Action ("BtnDeleteAccount_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnDeleteAccount_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnPrivacyPolicy_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnPrivacyPolicy_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnTermsOfUse_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnTermsOfUse_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnBlockedContacts != null) {
                btnBlockedContacts.Dispose ();
                btnBlockedContacts = null;
            }

            if (btnChangePassword != null) {
                btnChangePassword.Dispose ();
                btnChangePassword = null;
            }

            if (btnDeleteAccount != null) {
                btnDeleteAccount.Dispose ();
                btnDeleteAccount = null;
            }

            if (btnPrivacyPolicy != null) {
                btnPrivacyPolicy.Dispose ();
                btnPrivacyPolicy = null;
            }

            if (btnSilentMode != null) {
                btnSilentMode.Dispose ();
                btnSilentMode = null;
            }

            if (btnTermsOfUse != null) {
                btnTermsOfUse.Dispose ();
                btnTermsOfUse = null;
            }
        }
    }
}