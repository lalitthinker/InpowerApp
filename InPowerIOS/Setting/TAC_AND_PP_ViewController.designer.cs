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
    [Register ("TAC_AND_PP_ViewController")]
    partial class TAC_AND_PP_ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WebKit.WKWebView TermsofuseWebview { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TermsofuseWebview != null) {
                TermsofuseWebview.Dispose ();
                TermsofuseWebview = null;
            }
        }
    }
}