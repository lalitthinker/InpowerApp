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
    [Register ("CreateGroupViewController")]
    partial class CreateGroupViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField cmbInterestPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl GroupTypeUISC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ivGroupImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMakeThisGroupPrivate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch switchMakeThisGroupPrivate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtGroupName { get; set; }

        [Action ("GroupTypeUISC_ValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GroupTypeUISC_ValueChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
            if (cmbInterestPicker != null) {
                cmbInterestPicker.Dispose ();
                cmbInterestPicker = null;
            }

            if (GroupTypeUISC != null) {
                GroupTypeUISC.Dispose ();
                GroupTypeUISC = null;
            }

            if (ivGroupImage != null) {
                ivGroupImage.Dispose ();
                ivGroupImage = null;
            }

            if (lblMakeThisGroupPrivate != null) {
                lblMakeThisGroupPrivate.Dispose ();
                lblMakeThisGroupPrivate = null;
            }

            if (switchMakeThisGroupPrivate != null) {
                switchMakeThisGroupPrivate.Dispose ();
                switchMakeThisGroupPrivate = null;
            }

            if (txtDescription != null) {
                txtDescription.Dispose ();
                txtDescription = null;
            }

            if (txtGroupName != null) {
                txtGroupName.Dispose ();
                txtGroupName = null;
            }
        }
    }
}