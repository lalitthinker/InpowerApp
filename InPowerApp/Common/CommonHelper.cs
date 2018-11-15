using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using PCL.Common;
using Refractored.Controls;
using Square.Picasso;

namespace InPowerApp.Common
{
     public   class CommonHelper
    {
       

        public static void SetUserPreferences(string userrId,string password, string AccessToken, string email, string AWSAccessKey,
         string AWSSecretKey, string distance, string unit)
        {

            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var prefEditor = prefs.Edit();


            if (userrId != null)
            {
                prefEditor.PutString(GlobalConstant.PREF_USERID, userrId);

            }
            if (AccessToken != null)
            {
                prefEditor.PutString(GlobalConstant.PREF_AccessKEY, AccessToken);

            }
            if (email != null)
            {
                prefEditor.PutString(GlobalConstant.PREF_Email, email);

            }
            if (AWSAccessKey != null)
            {
                prefEditor.PutString(GlobalConstant.AWS_ACCESS_KEY, AWSAccessKey);

            }
            if (AWSSecretKey != null)
            {
                prefEditor.PutString(GlobalConstant.AWS_SECRET_KEY, AWSSecretKey);

            }

            if (password != null)
            {
                prefEditor.PutString(GlobalConstant.PREF_Password, password);

            }


            prefEditor.Commit();
        }

        public static Bitmap GetImageBitmapFromUrl(string url,Bitmap defaultImage)
        {
            Bitmap imageBitmap = null;
            if (url != null)
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }

                return imageBitmap;
            }
            else
            {
                return defaultImage;
            }
        }
        public static Bitmap GetDefaultProfile()
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(GlobalConstant.DefaultProfile);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }



        public static bool GetShouOut()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            return prefs.GetBoolean(GlobalConstant.PREF_SHOUT_OUT, false);
        }

        public static void SetShouOut(bool shoutOut)
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean(GlobalConstant.PREF_SHOUT_OUT, shoutOut);
            prefEditor.Apply();
            prefEditor.Commit();
        }





        public static string PREF_Password()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var password = prefs.GetString(GlobalConstant.PREF_Password, null);
            return password;
        }


        public static string GetAWS_ACCESS_KEY()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var AccessKey = prefs.GetString(GlobalConstant.AWS_ACCESS_KEY, null);
            return AccessKey;
        }
        public static string GetAWS_SECRET_KEY()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var SecretKey = prefs.GetString(GlobalConstant.AWS_SECRET_KEY, null);
            return SecretKey;
        }
        public static bool IsUserLogin()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);

            if (string.IsNullOrEmpty(prefs.GetString(GlobalConstant.PREF_USERID, null)))
            {
                return false;
            }
            return true;
        }
        public static void ClearPreferences()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.Clear();
            prefEditor.Commit();


        }
        public static string GetAuthKey()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            string AccessKey = prefs.GetString(GlobalConstant.PREF_AccessKEY, null);

            return AccessKey;
        }
        public static int GetUserId()
        {
            var prefs = Application.Context.GetSharedPreferences("UserDefault", FileCreationMode.Private);
            var UserId = Convert.ToInt32(prefs.GetString(GlobalConstant.PREF_USERID, null));

            return UserId;
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


      

        public static string GetRealPathFromURI(Activity act, Android.Net.Uri contentURI)
        {
            try
            {
                ICursor cursor = act.ContentResolver.Query(contentURI, null, null, null, null);
                cursor.MoveToFirst();
                string documentId = cursor.GetString(0);
                documentId = documentId.Split(':')[1];
                cursor.Close();

                cursor = act.ContentResolver.Query(
                MediaStore.Images.Media.ExternalContentUri,
                null, MediaStore.Images.Media.InterfaceConsts.Id + " = ? ", new[] { documentId }, null);
                cursor.MoveToFirst();
                string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data));
                cursor.Close();

                return path;
            }
            catch (Java.Lang.Exception e)
            {
                //Log.Debug("TAG_DATA", e.ToString());
                return "";
            }
        }

        public static void CreateDirectoryForPictures()
        {
            BitmapHelper._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "Inpower");
            if (!BitmapHelper._dir.Exists())
            {
                BitmapHelper._dir.Mkdirs();
            }
        }

        public static void SetImageOnUIImageView(ImageView imageView, string url, Context context,int height,int width,bool fit=false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }
           
            var filepath = new Java.IO.File(
               Android.OS.Environment.GetExternalStoragePublicDirectory(
                   Android.OS.Environment.DirectoryPictures), "Inpower");
            var filePath = System.IO.Path.Combine(filepath.ToString(), System.IO.Path.GetFileName(url));
            
            if (!File.Exists(filePath))
            {
               
                CommonHelper.CreateDirectoryForPictures();
                var target = new Target(filePath);
                if (fit==false)
                {
                    Picasso.With(context)

                  .Load(url) // thumbnail url goes here
                  .Placeholder(Resource.Drawable.imagesPlaceholder)
                  .Resize(height, width)
                  .Into(target);
                }
                else
                {
                    Picasso.With(context)

                .Load(url) // thumbnail url goes here
                .Placeholder(Resource.Drawable.imagesPlaceholder)
                .Fit()
                .Into(target);
                }

            }
            else
            {
                var fileAndpath = new Java.IO.File(
             Android.OS.Environment.GetExternalStoragePublicDirectory(
                 Android.OS.Environment.DirectoryPictures), System.IO.Path.Combine("Inpower", System.IO.Path.GetFileName(url)));
                Android.Net.Uri uri = Android.Net.Uri.FromFile(fileAndpath);
                if (fit == false)
                {
                    Picasso.With(context).Load(uri)
                 .Placeholder(Resource.Drawable.imagesPlaceholder)
                 .Resize(height, width).Into(imageView);
                }
                else
                {
                    Picasso.With(context).Load(uri)
             .Placeholder(Resource.Drawable.imagesPlaceholder)
             .Fit()
             .Into(imageView);
                }
              


            }

        }

        private class Target : Java.Lang.Object, ITarget
        {
            string PathURL;
            public Target(string URL)
            {
                this.PathURL = URL;
            }

            public void OnBitmapFailed(Drawable p0)
            {
              //  throw new NotImplementedException();
            }

            public void OnBitmapLoaded(Bitmap p0, Picasso.LoadedFrom p1)
            {
                try
                {
                    if (!File.Exists(PathURL))
                    {

                        using (var os = new System.IO.FileStream(PathURL, System.IO.FileMode.CreateNew))
                        {
                            p0.Compress(Bitmap.CompressFormat.Jpeg, 50, os);
                            os.Close();
                        }
                    }

                }
                catch (FileNotFoundException e)
                {
                    // e.printStackTrace();
                }
            }

            public void OnPrepareLoad(Drawable p0)
            {
              //  throw new NotImplementedException();
            }
        }
       public static  Android.App.ProgressDialog progress = null;    
        public static void ShowProgressBar(Context context)
        {
            progress = new Android.App.ProgressDialog(context);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Loading... Please wait...");
            progress.SetCancelable(false);
            progress.Show();
        }
        public static void DismissProgressBar(Context context)
        {
            progress.Dismiss();
        }

        public static void Hidekeyboard(Context context,Window window)
        {
            InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(window.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

        }
        public byte[] ConvertBitMapToByteArray(Bitmap bitmap)
        {
            byte[] result = null;
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, stream);
                result = stream.ToArray();
            }
            return result;
        }


    }
}