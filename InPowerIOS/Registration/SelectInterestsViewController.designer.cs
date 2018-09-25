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

namespace InPowerIOS.Registration
{
    [Register ("SelectInterestsViewController")]
    partial class SelectInterestsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BBIDone { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnRequest { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblSuggestInterest { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIToolbar TBSelectYourInterests { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtSuggestInterest { get; set; }

        [Action ("BBIDone_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BBIDone_Activated (UIKit.UIBarButtonItem sender);

        [Action ("BtnRequest_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnRequest_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BBIDone != null) {
                BBIDone.Dispose ();
                BBIDone = null;
            }

            if (btnRequest != null) {
                btnRequest.Dispose ();
                btnRequest = null;
            }

            if (tblSuggestInterest != null) {
                tblSuggestInterest.Dispose ();
                tblSuggestInterest = null;
            }

            if (TBSelectYourInterests != null) {
                TBSelectYourInterests.Dispose ();
                TBSelectYourInterests = null;
            }

            if (txtSuggestInterest != null) {
                txtSuggestInterest.Dispose ();
                txtSuggestInterest = null;
            }
        }
    }
}