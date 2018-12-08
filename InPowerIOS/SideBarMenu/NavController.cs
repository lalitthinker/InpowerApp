using Foundation;
using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using InPowerIOS.Common;

namespace InPowerIOS.SideBarMenu
{
    public partial class NavController : UINavigationController
    {
        public NavController() : base((string)null, null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //         View.BackgroundColor = ColorExtensions.NavigationColor();//UIColor.FromPatternImage(UIImage.FromFile("BlueGreenNav.png"));
            this.NavigationBar.TintColor = UIColor.White;
            this.NavigationBar.BarTintColor = ColorExtensions.NavigationColor();//102, 50, 178
            this.NavigationBar.Translucent = false;
            this.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;
          
            View.TintColor = UIColor.White;
            this.NavigationBar.BackgroundColor = ColorExtensions.NavigationColor();//0,51,102 - dark blue 146, 30, 146 - red purplle
            this.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };

            //var gradient = new CAGradientLayer();
            //gradient.Frame = this.View.Bounds;
            //gradient.NeedsDisplayOnBoundsChange = true;
            //gradient.MasksToBounds = true;

            //gradient.Colors = new CGColor[]
            //{
            //      UIColor.FromRGB(0, 214, 168).CGColor,
            //      UIColor.FromRGB(119, 218, 242).CGColor,

            //};

            //gradient.Transform.Rotate(180, 0, 0, 0);

            //UIGraphics.BeginImageContext(gradient.Bounds.Size);
            //gradient.RenderInContext(UIGraphics.GetCurrentContext());
            //UIImage backImage = UIGraphics.GetImageFromCurrentImageContext();
            //UIGraphics.EndImageContext();
            //this.NavigationBar.SetBackgroundImage(backImage, UIBarMetrics.Default);


            // Perform any additional setup after loading the view, typically from a nib.
        }

      
    }
}