using Foundation;
using System;
using UIKit;
using System.IO;

namespace InPowerIOS.Setting
{
    public partial class TAC_AND_PP_ViewController : UIViewController
    {
        public TAC_AND_PP_ViewController (IntPtr handle) : base (handle)
        {
        }

        public bool isPrivacyPolicy { get; set; }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            //var fileName = "termsofuse.htm"; // remember case-sensitive
            //Title = "Terms of Use";
            //if (isPrivacyPolicy)
            //{
            //    fileName = "privacypolicy.htm";
            //    Title = "Privacy Policy";
            //}
            //var localHtmlUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);

            //TermsofuseWebview.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
        }
    }
}