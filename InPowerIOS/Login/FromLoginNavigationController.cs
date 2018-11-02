using Foundation;
using InPowerIOS.Common;
using InPowerIOS.Registration;
using System;
using UIKit;

namespace InPowerIOS.Login
{
    public partial class FromLoginNavigationController : UINavigationController
    {
        public FromLoginNavigationController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.NavigationBar.TintColor = UIColor.White;
            this.NavigationBar.BarTintColor = ColorExtensions.NavigationColor();//102, 50, 178
            this.View.TintColor = UIColor.White;
            this.NavigationBar.BackgroundColor = ColorExtensions.NavigationColor();//0,51,102 - dark blue 146, 30, 146 - red purplle
            this.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };


            InvokeOnMainThread(delegate
            {
                var registersViewController = (TellUSAboutYourselfViewController)Storyboard.InstantiateViewController("TellUSAboutYourselfViewController");
                this.PushViewController(registersViewController, true);
            });
        }
    }
}