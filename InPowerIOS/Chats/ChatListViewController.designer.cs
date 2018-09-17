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
    [Register ("ChatListViewController")]
    partial class ChatListViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblChatList { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tblChatList != null) {
                tblChatList.Dispose ();
                tblChatList = null;
            }
        }
    }
}