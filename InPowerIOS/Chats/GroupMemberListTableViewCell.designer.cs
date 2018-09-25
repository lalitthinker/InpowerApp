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
    [Register ("GroupMemberListTableViewCell")]
    partial class GroupMemberListTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ivMemberProfilePic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMemberName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ivMemberProfilePic != null) {
                ivMemberProfilePic.Dispose ();
                ivMemberProfilePic = null;
            }

            if (lblMemberName != null) {
                lblMemberName.Dispose ();
                lblMemberName = null;
            }
        }
    }
}