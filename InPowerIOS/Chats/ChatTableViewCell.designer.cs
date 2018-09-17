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
    [Register ("ChatTableViewCell")]
    partial class ChatTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BubbleImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LeftMsgTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LeftUserMsg { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BubbleImageView != null) {
                BubbleImageView.Dispose ();
                BubbleImageView = null;
            }

            if (LeftMsgTime != null) {
                LeftMsgTime.Dispose ();
                LeftMsgTime = null;
            }

            if (LeftUserMsg != null) {
                LeftUserMsg.Dispose ();
                LeftUserMsg = null;
            }
        }
    }
}