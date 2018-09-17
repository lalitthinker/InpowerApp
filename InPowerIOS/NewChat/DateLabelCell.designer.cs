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
    [Register ("DateLabelCell")]
    partial class DateLabelCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton lblDate { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblDate != null) {
                lblDate.Dispose ();
                lblDate = null;
            }
        }
    }
}