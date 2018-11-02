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
    [Register ("PrivateChatListViewController")]
    partial class PrivateChatListViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAttachment { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSend { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblChatList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txt_TextMessage { get; set; }

        [Action ("BtnAttachment_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnAttachment_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnSend_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSend_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnAttachment != null) {
                btnAttachment.Dispose ();
                btnAttachment = null;
            }

            if (btnSend != null) {
                btnSend.Dispose ();
                btnSend = null;
            }

            if (tblChatList != null) {
                tblChatList.Dispose ();
                tblChatList = null;
            }

            if (txt_TextMessage != null) {
                txt_TextMessage.Dispose ();
                txt_TextMessage = null;
            }
        }
    }
}