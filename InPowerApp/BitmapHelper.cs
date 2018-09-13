using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;

using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

using Android.Media;

namespace InPowerApp
{
    public static class BitmapHelper
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
        public static Android.Net.Uri captureImageUri;

        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            // Images are being saved in landscape, so rotate them back to portrait if they were taken in portrait
            Matrix mtx = new Matrix();
            ExifInterface exif = new ExifInterface(fileName);
           

            var orientation = (Android.Media.Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Undefined);

            switch (orientation)
            {
                case Android.Media.Orientation.Undefined: // Nexus 7 landscape...
                    break;
                case Android.Media.Orientation.Normal: // landscape
                    break;
                case Android.Media.Orientation.FlipHorizontal:
                    break;
                case Android.Media.Orientation.Rotate180:
                    mtx.PreRotate(180);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case Android.Media.Orientation.FlipVertical:
                    break;
                case Android.Media.Orientation.Transpose:
                    break;
                case Android.Media.Orientation.Rotate90: // portrait
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case Android.Media.Orientation.Transverse:
                    break;
                case Android.Media.Orientation.Rotate270: // might need to flip horizontally too...
                    mtx.PreRotate(270);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                default:
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
            }

            return resizedBitmap;
        }
     
    }
}