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
    [Register ("ChatViewContaroller")]
    partial class ChatViewContaroller
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblChat { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtCHatInput { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tblChat != null) {
                tblChat.Dispose ();
                tblChat = null;
            }

            if (txtCHatInput != null) {
                txtCHatInput.Dispose ();
                txtCHatInput = null;
            }
        }
    }
}