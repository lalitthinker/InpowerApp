using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace InPowerIOS.Common
{
    public static class ImageClass
    {
        // image resize for side menu commmon function
        public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = (float)Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1)
                return sourceImage;
            var width = maxResizeFactor * (float)sourceSize.Width;
            var height = maxResizeFactor * (float)sourceSize.Height;
            UIGraphics.BeginImageContext(new CGSize(width, height));
            sourceImage.Draw(new CGRect(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        public static UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
                return UIImage.LoadFromData(data);
        }

    }
}
