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

namespace InPowerIOS
{
    [Register ("GroupChatAttachmentCell")]
    partial class GroupChatAttachmentCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView AttachmentImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AttahmentImageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BackgroundImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSenderName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView StatusTick { get; set; }

        [Action ("AttahmentImageButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AttahmentImageButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AttachmentImageView != null) {
                AttachmentImageView.Dispose ();
                AttachmentImageView = null;
            }

            if (AttahmentImageButton != null) {
                AttahmentImageButton.Dispose ();
                AttahmentImageButton = null;
            }

            if (BackgroundImageView != null) {
                BackgroundImageView.Dispose ();
                BackgroundImageView = null;
            }

            if (lblMessage != null) {
                lblMessage.Dispose ();
                lblMessage = null;
            }

            if (lblSenderName != null) {
                lblSenderName.Dispose ();
                lblSenderName = null;
            }

            if (lblTime != null) {
                lblTime.Dispose ();
                lblTime = null;
            }

            if (StatusTick != null) {
                StatusTick.Dispose ();
                StatusTick = null;
            }
        }
    }
}