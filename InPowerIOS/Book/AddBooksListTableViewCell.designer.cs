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
    [Register ("AddBooksListTableViewCell")]
    partial class AddBooksListTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnRead { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnWishList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAutherName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblBookTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ReadBookImage { get; set; }

        [Action ("BtnRead_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnRead_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnWishList_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnWishList_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnRead != null) {
                btnRead.Dispose ();
                btnRead = null;
            }

            if (btnWishList != null) {
                btnWishList.Dispose ();
                btnWishList = null;
            }

            if (lblAutherName != null) {
                lblAutherName.Dispose ();
                lblAutherName = null;
            }

            if (lblBookTitle != null) {
                lblBookTitle.Dispose ();
                lblBookTitle = null;
            }

            if (ReadBookImage != null) {
                ReadBookImage.Dispose ();
                ReadBookImage = null;
            }
        }
    }
}