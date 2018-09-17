using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using PCL.Common;
using SDWebImage;
using UIKit;

namespace InPowerIOS.Common
{
    public class CommonHelper
    {
      
        public static string GetHelpImageName(string imageName, double width, double height)
        {
            if (width >= 414)
                // iPhone 6 Plus
                return imageName + "_6p";
            if (width >= 375)
                // iPhone 6
                return imageName + "_6";
            if (width >= 320 && height >= 568)
                // iPhone 5
                return imageName + "_5";
            if (width >= 320)
                // iPhone 4
                return imageName + "_4";
            return imageName + "_4";
        }

        public static void ClearUserPreferences()
        {
            SetUserPreferences("","","", "", "", "");
        }

        public static string GetVideoThumbUrl(string videoUrl)
        {
            string videoThumbUrl = null;
            //          string videoThumbUrl = Path.GetDirectoryName (videoUrl) + "/" + Path.GetFileNameWithoutExtension (videoUrl) + "-thumb.jpg";
            var ext = Path.GetExtension(videoUrl);
            if (ext.Equals(".3gp"))
            {
                videoThumbUrl = videoUrl.Replace(".3gp", "-thumb.jpg");
            }
            else if (ext.Equals(".mp4"))
            {
                videoThumbUrl = videoUrl.Replace(".mp4", "-thumb.jpg");
            }
            return videoThumbUrl;
        }

        public static string GetImageThumbUrl(string imageUrl)
        {
            if (!imageUrl.Contains("inpower"))
                return imageUrl;
            var thumbImage = Path.GetFileNameWithoutExtension(imageUrl);
            return imageUrl.Replace(thumbImage, thumbImage + "-bigger");
        }

        public static void SetUserPreferences(string userrId, string password, string AccessToken, string email, string AWSAccessKey,
         string AWSSecretKey)
        {
           
            if (userrId != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(userrId, CommonConstant.PREF_USERID);
            }
            if (AccessToken != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(AccessToken, CommonConstant.PREF_AccessKEY);
            }
            if (email != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(email, CommonConstant.PREF_Email);
            }

            if (AWSAccessKey != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(AWSAccessKey, CommonConstant.AWS_ACCESS_KEY);
            }

            if (AWSSecretKey != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(AWSSecretKey, CommonConstant.AWS_SECRET_KEY);
            }


            if (password != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(password, CommonConstant.PREF_Password);
            }


            NSUserDefaults.StandardUserDefaults.Synchronize();
            GlobalConstant.AccessToken = GetAccessToken();
            CommonConstant.AccessToken = GetAccessToken();
        }

        public static bool IsUserLoggedIn()
        {
            if (string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(CommonConstant.PREF_USERID)))
            {
                return false;
            }
            return true;
        }

        public static string PREF_Password()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(CommonConstant.PREF_Password);
        }

        public static string GetAccessToken()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(CommonConstant.PREF_AccessKEY);
        }
        public static string GetAWSAccessKey()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(CommonConstant.AWS_ACCESS_KEY);
        }
        public static string GetAWSSecretKey()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(CommonConstant.AWS_SECRET_KEY);
        }
        public static int GetUserId()
        {
            var UserId = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.StringForKey(CommonConstant.PREF_USERID));
            return UserId;
        }
      
        // resize the image to be contained within a maximum width and height, keeping aspect ratio
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

        // resize the image (without trying to maintain aspect ratio)
        public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new CGSize(width, height));
            sourceImage.Draw(new CGRect(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        // crop the image, without resizing
        public static UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
        {
            var imgSize = sourceImage.Size;
            UIGraphics.BeginImageContext(new CGSize(width, height));
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new CGRect(0, 0, width, height);
            context.ClipToRect(clippedRect);
            var drawRect = new CGRect(-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
            sourceImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return modifiedImage;
        }

        // crop the image, without resizing
        public static UIImage CropSquareImage(UIImage sourceImage, float dimension)
        {
            var resizedImage = MaxResizeImage(sourceImage, dimension, dimension);
            var imgSize = resizedImage.Size;
            var largerDimension = imgSize.Width > imgSize.Height ? imgSize.Width : imgSize.Height;
            var crop_y = largerDimension == imgSize.Width ? 0 : (largerDimension - imgSize.Width) / 2;
            var crop_x = largerDimension == imgSize.Height ? 0 : (largerDimension - imgSize.Height) / 2;

            UIGraphics.BeginImageContext(new CGSize(dimension, dimension));
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new CGRect(0, 0, dimension, dimension);
            context.ClipToRect(clippedRect);
            var drawRect = new CGRect(-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
            resizedImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return modifiedImage;
        }

        public static UIImage GetThumbnailFromVideoURL(NSUrl videoURL)
        {
            UIImage theImage = null;
            var asset = AVAsset.FromUrl(videoURL);
            var generator = AVAssetImageGenerator.FromAsset(asset);
            generator.AppliesPreferredTrackTransform = true;
            NSError error = null;
            var time = new CMTime(1, 60);
            CMTime actTime;
            var img = generator.CopyCGImageAtTime(time, out actTime, out error);

            if (error != null)
            {
                Console.WriteLine(error.ToString());
                return UIImage.FromBundle("videoimage.png");
            }
            Console.WriteLine(img.ToString());
            theImage = new UIImage(img);
            var path = videoURL.RelativePath;
            var imgData = SuperimposeImage(theImage, UIImage.FromBundle("btn_play.png")).AsJPEG(0.5f);
            NSError err = null;
            var jpgFilename = path.Replace(Path.GetExtension(path), "-thumb.jpg");
            if (imgData.Save(jpgFilename, false, out err))
            {
                if (err == null)
                {
                    Console.WriteLine("Thumbnail saved");
                }
                else
                {
                    Console.WriteLine("error in saving " + err);
                }
            }
            else
            {
                Console.WriteLine("Thumbnail not saved");
            }

            return theImage;
        }

        public static UIImage SuperimposeImage(UIImage image1, UIImage image2)
        {
            UIImage combinedImage;
            UIGraphics.BeginImageContext(image1.Size);
            image1.Draw(new CGRect(
                0, 0, image1.Size.Width, image1.Size.Height));

            image2.Draw(new CGRect(
                image1.Size.Width / 4,
                image1.Size.Height / 4,
                image1.Size.Width / 2,
                image1.Size.Height / 2));

            combinedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return combinedImage;
        }

        public static void SaveVideoThumbnail(UIImage image, string thumbname)
        {
            var img = UIImage.FromBundle("btn_play.png");
            var imgData = SuperimposeImage(image, img).AsJPEG(0.5f);
            NSError err = null;
            //          thumbname = "bitmob-" + Guid.NewGuid () + ".jpg";
            //          string jpgFilename = System.IO.Path.Combine (CommonHelper.GetDirectoryForVideo (), thumbname);
            if (imgData.Save(thumbname, false, out err))
            {
                if (err == null)
                {
                    Console.WriteLine("Thumbnail saved");
                }
                else
                {
                    Console.WriteLine("error in saving " + err);
                }
            }
            else
            {
                Console.WriteLine("Thumbnail not saved");
            }
        }

        public static void SaveImage(UIImage image, string thumbname)
        {
            var imgData = image.AsJPEG(0.5f);
            NSError err = null;
            if (imgData.Save(thumbname, false, out err))
            {
                if (err == null)
                {
                    Console.WriteLine("Thumbnail saved");
                }
                else
                {
                    Console.WriteLine("error in saving " + err);
                }
            }
            else
            {
                Console.WriteLine("Thumbnail not saved");
            }
        }

        public static void SetVideoThumbnailOnUIImageView(UIImageView imageView, string url, int type = 0,
            bool addBtn = true)
        {
            var filePath = Path.Combine(GetDirectoryForVideo(), GetVideoThumbUrl(Path.GetFileName(url)));
            //Check image exists
            if (!File.Exists(filePath))
            {
                var vdoThumb = GetVideoThumbUrl(url);
                imageView.SetImage(
                    new NSUrl(vdoThumb),
                    UIImage.FromBundle("default_noimage.png"),
                    SDWebImageOptions.ProgressiveDownload,
                    (image, error, cacheType, finished) =>
                    {
                        if (image != null)
                        {
                            SaveVideoThumbnail(image, filePath);
                        }
                    }
                    );
            }
            else
            {
                imageView.SetImage(NSUrl.FromFilename(filePath));
            }
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            if (addBtn)
            {
                //              UIImageView myImageView = new UIImageView (imageView.Frame);
                //              myImageView.ContentMode = UIViewContentMode.Center;
                //              myImageView.Image = UIImage.FromFile ("btn_play.png");
                //              imageView.Add (myImageView);
            }
        }

        public static void SetBitmobVideoThumbnailOnUIImageView(UIImageView imageView, string url, int type = 0)
        {
            var filePath = Path.Combine(GetDirectoryForVideo(), GetVideoThumbUrl(Path.GetFileName(url)));
            if (!File.Exists(filePath))
            {
                var vdoThumb = GetVideoThumbUrl(url);
                imageView.SetImage(
                    new NSUrl(vdoThumb),
                    UIImage.FromBundle("default_noimage.png"),
                    SDWebImageOptions.ProgressiveDownload,
                    (image, error, cacheType, finished) =>
                    {
                        if (image != null)
                        {
                            SaveVideoThumbnail(image, filePath);
                        }
                    }
                    );
            }
            else
            {
                imageView.SetImage(NSUrl.FromFilename(filePath));
            }
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //          if(type == 0)
            //              imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //          else if(type == 1)
            //              imageView.ContentMode = UIViewContentMode.Left;
            //          else if(type == 2)
            //              imageView.ContentMode = UIViewContentMode.Right;
        }

        // type = 0 -> Center
        // type = 1 -> Left
        // type = 2 -> Right
        public static void SetImageOnUIImageView(UIImageView imageView, string url, int type = 0)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                imageView.Image = UIImage.FromBundle("default_noimage.png");
            }

            var filePath = Path.Combine(GetDirectoryForPictures(), Path.GetFileName(url));

            if (!File.Exists(filePath))
            {
                imageView.SetImage(
                    new NSUrl(url),
                    UIImage.FromBundle("default_noimage.png"),
                    SDWebImageOptions.RetryFailed,
                    (image, error, cacheType, finished) =>
                    {
                        if (image != null)
                        {
                            SaveImage(image, filePath);
                        }
                    }
                    );
            }
            else
            {
                imageView.SetImage(NSUrl.FromFilename(filePath));
            }
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //          if(type == 0)
            //              imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //          else if(type == 1)
            //              imageView.ContentMode = UIViewContentMode.Left;
            //          else if(type == 2)
            //              imageView.ContentMode = UIViewContentMode.Right;
        }

        //      + (UIImage *)thumbnailFromVideoAtURL:(NSURL *)contentURL {
        //          UIImage *theImage = nil;
        //          AVURLAsset *asset = [[AVURLAsset alloc] initWithURL:contentURL options:nil];
        //          AVAssetImageGenerator *generator = [[AVAssetImageGenerator alloc] initWithAsset:asset];
        //          generator.appliesPreferredTrackTransform = YES;
        //          NSError *err = NULL;
        //          CMTime time = CMTimeMake(1, 60);
        //          CGImageRef imgRef = [generator copyCGImageAtTime:time actualTime:NULL error:&err];
        //
        //          theImage = [[[UIImage alloc] initWithCGImage:imgRef] autorelease];
        //
        //          CGImageRelease(imgRef);
        //          [asset release];
        //          [generator release];
        //
        //          return theImage;
        //      }

        public static UIImage GetMenuIcon()
        {
            return MaxResizeImage(UIImage.FromFile("icon-menu.png"), 24, 24);
        }

        public static UIImage GetStartChatIcon()
        {
            return MaxResizeImage(UIImage.FromFile("startnewchatimage.png"), 24, 24);
        }

        public static UIImage GetCorrectIcon()
        {
            return MaxResizeImage(UIImage.FromFile("correct32.png"), 32, 32);
        }
        public static UIImage GetEditIcon()
        {
            return MaxResizeImage(UIImage.FromFile("editicon.png"), 26, 26);
        }

        public static UIImage GetEditIconNew()
        {
            return MaxResizeImage(UIImage.FromBundle("editicon_new.png"), 26, 26);
        }

        public static UIImage GetClipIcon()
        {
            return MaxResizeImage(UIImage.FromFile("attachmentclip.png"), 28, 28);
        }

        public static UIImage GetInfoIcon()
        {
            return MaxResizeImage(UIImage.FromFile("infoicon.png"), 26, 26);
        }

        public static UIImage GetDotsMenu()
        {
            return MaxResizeImage(UIImage.FromFile("threedotsmenuimage.png"), 5, 28);
        }

        public static UIImage GetSearchBarMenu()
        {
            return MaxResizeImage(UIImage.FromFile("search.png"), 28, 28);
        }

        public static UIImage GetLogoMenu()
        {
            return MaxResizeImage(UIImage.FromFile("Icon-Small.png"), 28, 28);
        }

        public static UIImage GetRadiousChatNavigationLogoMenu()
        {
            return MaxResizeImage(UIImage.FromFile("cloud-new1.png"), 30, 30);
        }
        public static string GetDirectoryForPictures()
        {
            //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg")
            var _dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "InpowerPictures");
            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }
            return _dirPath;
        }

        public static string GetDirectoryForAudio()
        {
            //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg")
            var _dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "InpowerAudio");
            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }
            return _dirPath;
        }

        public static string GetDirectoryForVideo()
        {
            //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg")
            var _dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "InpowerVideo");
            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }
            return _dirPath;
        }

        public static string Base64Encode(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }





        public static string GetMediaTypeByExtension(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                    return "image/jpg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                default:
                    return "";
            }
        }

        public static void PrintException(string message, Exception e)
        {
            Console.WriteLine(message + "\n Err StackTrace :" + e.StackTrace + "\n Err Message :" + e.Message);
        }


        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            try
            {
                var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
                return new DateTime(unixStart.Ticks + unixTimeStampInTicks);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err for parsing to date => ", ex.StackTrace);
                throw ex;
            }
        }

        public static string DateTimeToUnixTimestamp(DateTime dateTime)
        {
            try
            {
                var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var unixTimeStampInTicks = (dateTime - unixStart).Ticks;
                var ticks = unixTimeStampInTicks / TimeSpan.TicksPerSecond;
                return ticks.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err for parsing to string => ", ex.StackTrace);
                throw ex;
            }
        }





        // http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time
        public static string GetRelativeTime(DateTime dt, bool fromUTC = true)
        {
            TimeSpan ts;
            if (fromUTC)
                ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            else
                ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
            var delta = Math.Abs(ts.TotalSeconds);
            if (delta < 0)
                delta = 0;
            if (delta < 60)
            {
                var time = ts.Seconds == 1 ? "1s ago" : ts.Seconds + "s ago";
                if (time.Contains("-"))
                    time = "1s ago";
                return time;
            }
            if (delta < 120)
            {
                return "1m ago";
            }
            if (delta < 2700)
            {
                // 45 * 60
                return ts.Minutes + "m ago";
            }
            if (delta <= 5400)
            {
                // 90 * 60
                return "1hr ago";
            }
            if (delta < 86400)
            {
                // 24 * 60 * 60
                return Math.Round((decimal)(delta / 5400), 0) + "h ago"; // ts.Hours + "h ago"
            }
            if (delta < 172800)
            {
                // 48 * 60 * 60
                return "yesterday";
            }
            if (delta < 2592000)
            {
                // 30 * 24 * 60 * 60
                return Math.Round((decimal)(delta / 172800), 0) + "d ago"; // ts.Days
            }
            if (delta < 31104000)
            {
                // 12 * 30 * 24 * 60 * 60
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }



        public static void SetCircularImage(UIImageView ImageView, bool IsShowCorner=false)
        {
            ImageView.Layer.CornerRadius = ImageView.Frame.Size.Width / 2;
            ImageView.ClipsToBounds = true;
            if(IsShowCorner)
            ImageView.Layer.BorderWidth = 1.0f;
        }
    }
}