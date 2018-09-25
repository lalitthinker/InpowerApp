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
    [Register ("UserProfileViewController")]
    partial class UserProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ivUserProfilePic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAboutMe { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblBlockUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCityState { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblReportUser { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ivUserProfilePic != null) {
                ivUserProfilePic.Dispose ();
                ivUserProfilePic = null;
            }

            if (lblAboutMe != null) {
                lblAboutMe.Dispose ();
                lblAboutMe = null;
            }

            if (lblBlockUser != null) {
                lblBlockUser.Dispose ();
                lblBlockUser = null;
            }

            if (lblCityState != null) {
                lblCityState.Dispose ();
                lblCityState = null;
            }

            if (lblEmail != null) {
                lblEmail.Dispose ();
                lblEmail = null;
            }

            if (lblReportUser != null) {
                lblReportUser.Dispose ();
                lblReportUser = null;
            }
        }
    }
}