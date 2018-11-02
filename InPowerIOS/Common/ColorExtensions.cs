using System;
using UIKit;

namespace InPowerIOS.Common
{
    //common file to change the theme colors
    public static class ColorExtensions
    {
        public static UIColor LoginHeader()
        {
            return UIColor.FromRGB(36, 36, 36);
        }

        public static UIColor LoginBackgound()
        {
            return UIColor.FromRGB(36, 36, 36);
        }

		public static UIColor NavigationColor()
		{
            return UIColor.FromRGB(36, 36, 36);
		}

		public static UIColor SideHeaderBackground()
		{
            return UIColor.FromRGB(36, 36, 36);
		}

        public static UIColor ErrorToast()
        {
            return UIColor.FromRGB(196, 198, 200);
        }

        public static UIColor SuccessToast()
        {
            return UIColor.FromRGB(51, 153, 255);
        }

        public static UIColor DefaultToast()
        {
            return UIColor.FromRGB(36, 36, 36);
        }

    }

}
