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
    [Register ("GroupDetailsViewController")]
    partial class GroupDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GroupDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView GroupImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GroupIntrest { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GroupType { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLeaveGroup { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblMemberList { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GroupDescription != null) {
                GroupDescription.Dispose ();
                GroupDescription = null;
            }

            if (GroupImageView != null) {
                GroupImageView.Dispose ();
                GroupImageView = null;
            }

            if (GroupIntrest != null) {
                GroupIntrest.Dispose ();
                GroupIntrest = null;
            }

            if (GroupType != null) {
                GroupType.Dispose ();
                GroupType = null;
            }

            if (lblLeaveGroup != null) {
                lblLeaveGroup.Dispose ();
                lblLeaveGroup = null;
            }

            if (tblMemberList != null) {
                tblMemberList.Dispose ();
                tblMemberList = null;
            }
        }
    }
}