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
    [Register ("ChatListTableViewCell")]
    partial class ChatListTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblChatLastMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblChatLastTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblChatUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton lblMessageCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfileImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblChatLastMessage != null) {
                lblChatLastMessage.Dispose ();
                lblChatLastMessage = null;
            }

            if (lblChatLastTime != null) {
                lblChatLastTime.Dispose ();
                lblChatLastTime = null;
            }

            if (lblChatUserName != null) {
                lblChatUserName.Dispose ();
                lblChatUserName = null;
            }

            if (lblMessageCount != null) {
                lblMessageCount.Dispose ();
                lblMessageCount = null;
            }

            if (ProfileImage != null) {
                ProfileImage.Dispose ();
                ProfileImage = null;
            }
        }
    }
}