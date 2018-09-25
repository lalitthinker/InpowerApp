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

namespace InPowerIOS.Book
{
    [Register ("BookShelfReadListViewController")]
    partial class BookShelfReadListViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAddBooks { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblBookReadList { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAddBooks != null) {
                btnAddBooks.Dispose ();
                btnAddBooks = null;
            }

            if (tblBookReadList != null) {
                tblBookReadList.Dispose ();
                tblBookReadList = null;
            }
        }
    }
}