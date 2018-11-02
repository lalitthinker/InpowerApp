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

namespace InPowerIOS.Setting
{
    [Register ("InviteFriendsViaSMSViewController")]
    partial class InviteFriendsViaSMSViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblPhoneContacts { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tblPhoneContacts != null) {
                tblPhoneContacts.Dispose ();
                tblPhoneContacts = null;
            }
        }
    }
}