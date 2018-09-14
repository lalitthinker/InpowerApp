using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using InPowerApp.Activities;
using InPowerApp.Common;
using PCL.Service;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;

namespace InPowerApp.Fragments
{
    public class SettingsFragment : Android.Support.V4.App.Fragment
    {
        ToggleButton SlientToggleButton;
        TextView btnChangePassword, btnBlockedContacts, btnIFVEMAIL, InviteViaSMS, btnPrivacyPolicy, btnTermsOfUse, btnDeleteAccount, btnBlockedContactList;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here 
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Settings_Fragment, container, false);

            btnChangePassword = view.FindViewById<TextView>(Resource.Id.btnChangePassword);
            btnBlockedContacts = view.FindViewById<TextView>(Resource.Id.btnBlockedContacts);
            //  btnIFVEMAIL = view.FindViewById<TextView>(Resource.Id.btnIFVEMAIL);
            InviteViaSMS = view.FindViewById<TextView>(Resource.Id.txtInviteViaSMS);
            btnPrivacyPolicy = view.FindViewById<TextView>(Resource.Id.txtPolicyPrivacy);
            btnTermsOfUse = view.FindViewById<TextView>(Resource.Id.txtTermsOfUse);
            btnDeleteAccount = view.FindViewById<TextView>(Resource.Id.txtDeleteAccount);
            btnBlockedContactList = view.FindViewById<TextView>(Resource.Id.btnBlockedContacts);
            SlientToggleButton = (ToggleButton)view.FindViewById(Resource.Id.toggleButtonShoutOut);

            HasOptionsMenu = true;

            btnChangePassword.Click += BtnChangePassword_Click;
            btnBlockedContacts.Click += BtnBlockedContacts_Click;
            //  btnIFVEMAIL.Click += BtnIFVEMAIL_Click;
            InviteViaSMS.Click += InviteViaSMS_Click;
            btnPrivacyPolicy.Click += BtnPrivacyPolicy_Click;
            btnTermsOfUse.Click += BtnTermsOfUse_Click;
            btnDeleteAccount.Click += BtnDeleteAccount_Click;
            SlientToggleButton.Click += SlientToggleButton_Click;


            if (CommonHelper.GetShouOut())
            {
                SlientToggleButton.Checked = true;
            }
            else
            {

                SlientToggleButton.Checked = false;
            }


            return view;
        }

        private void SlientToggleButton_Click(object sender, EventArgs e)
        {

            if (SlientToggleButton.Checked)
            {

                CommonHelper.SetShouOut(true);

                AudioManager am = (AudioManager)Context.GetSystemService(Context.AudioService);
                am.RingerMode = RingerMode.Silent;



            }
            else
            {
                CommonHelper.SetShouOut(false);

                ImageHelper ft = new ImageHelper();

                AudioManager am = (AudioManager)Context.GetSystemService(Context.AudioService);
                am.RingerMode = RingerMode.Normal;


            }




        }






        public class ImageHelper
        {
            static string baseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            private static bool NeedsDownload(string uid)
            {
                return !File.Exists(System.IO.Path.Combine(baseDir, uid));
            }
            public static async void SetImage(Context context, ImageView iv, FacebookFriend f)
            {
                FileInfo fi = new FileInfo(System.IO.Path.Combine(baseDir, f.uid));
                if (!fi.Exists || fi.LastWriteTime < DateTime.Now.AddDays(-7))
                {
                    using (var bmp = await DownloadImageAsync(f.pic_square, f.uid))
                    {
                        iv.SetImageBitmap(bmp);
                    }
                }
                else
                {
                    using (var bmp = BitmapFactory.DecodeFile(System.IO.Path.Combine(baseDir, f.uid)))
                    {
                        iv.SetImageBitmap(bmp);
                    }
                }
            }
            private static Task<Bitmap> DownloadImageAsync(string url, string uid)
            {
                return Task.Run<Bitmap>(() => DownloadImage(url, uid));
            }
            private static Bitmap DownloadImage(string url, string uid)
            {
                Bitmap imageBitmap = null;
                try
                {
                    byte[] imageBytes;
                    using (var mstream = new MemoryStream())
                    {
                        using (var imageUrl = new Java.Net.URL(url))
                        {
                            var options = new BitmapFactory.Options
                            {
                                InSampleSize = 1,
                                InPurgeable = true
                            };

                            var bit = BitmapFactory.DecodeStream(imageUrl.OpenStream(), null, options);
                            bit.Compress(Bitmap.CompressFormat.Jpeg, 70, mstream);
                        }
                        imageBytes = mstream.ToArray();
                        System.IO.File.WriteAllBytes(System.IO.Path.Combine(baseDir, uid), imageBytes);
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            var options = new BitmapFactory.Options
                            {
                                InJustDecodeBounds = true,
                            };
                            // BitmapFactory.DecodeResource() will return a non-null value; dispose of it.
                            using (var dispose = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length))
                            {
                            }
                            var imageHeight = options.OutHeight;
                            var imageWidth = options.OutWidth;
                            var imageType = options.OutMimeType;
                            var height = (float)options.OutHeight;
                            var width = (float)options.OutWidth;
                            var inSampleSize = 1D;

                            if (height > 100 || width > 100)
                            {
                                inSampleSize = width > height
                                                    ? height / 100
                                                    : width / 100;
                            }
                            options.InSampleSize = (int)inSampleSize;
                            options.InJustDecodeBounds = false;
                            return BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogPriority.Error, "GetImageFromBitmap Error", ex.Message);
                }
                return imageBitmap;
            }
        }




        public class FacebookFriend : IEquatable<FacebookFriend>
        {
          
            public string uid = "";
            public string name = "";
            public string pic_square = "";
            public byte[] ImageBytes = null;
            public static bool ContinueImageLoad = true;
            public FacebookFriend() { }
            public static void BeginLoadFriendImages(List<FacebookFriend> friends)
            {
                using (WebClient wc = new WebClient())
                {
                    foreach (FacebookFriend f in friends)
                    {
                        if (ContinueImageLoad)
                        {
                            if (f.ImageBytes == null)
                            {
                                f.ImageBytes = wc.DownloadData(f.pic_square);
                            }
                        }
                    }
                }

            }

            public bool Equals(FacebookFriend other)
            {
                return this.uid == other.uid;
            }
        
    }
       











        private void BtnDeleteAccount_Click(object sender, EventArgs e)
        {

            AlertDialog.Builder alert = new AlertDialog.Builder(Activity);
            alert.SetTitle("Delete Account");
            alert.SetMessage("Do you want to delete this account?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                DeleteAccountAsync();

            });

            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
                Toast.MakeText(Activity, "Cancelled!", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private async System.Threading.Tasks.Task DeleteAccountAsync()

        {

            string VerifyPassword = null;

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(Activity);
            alertDialog.SetTitle("Delete Account");
            alertDialog.SetMessage("Enter your password");

            int IconAttribute = Android.Resource.Attribute.AlertDialogIcon;
            EditText input = new EditText(Activity);
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.MatchParent);
            input.SetHeight(200);
            input.SetWidth(250);

            input.TextAlignment = TextAlignment.Center;

            //input.LayoutParameters(lp);
            alertDialog.SetView(input);
            alertDialog.SetIconAttribute(IconAttribute);
            alertDialog.SetPositiveButton(
                "Ok",
                async (see, ess) =>
                {
                    CommonHelper.ShowProgressBar(Activity);
                    VerifyPassword = input.Text.ToString();
                    if (VerifyPassword != "")
                    {
                        if (VerifyPassword != null && VerifyPassword == CommonHelper.PREF_Password())
                        {
                            var result = await new SettingService().PostDeleteAccountInterest(CommonHelper.GetUserId());
                            if (result.Status == 1)
                            {
                                CommonHelper.ClearPreferences();
                                Activity.FinishAffinity();
                            }
                        
                            HideKeyboard(input);
                            Toast.MakeText(Activity, result.Message, ToastLength.Long).Show();
                            CommonHelper.DismissProgressBar(Activity);

                        }
                        else
                        {
                           
                            HideKeyboard(input);
                            Toast.MakeText(Activity, "Enter your valid password", ToastLength.Long).Show();
                            CommonHelper.DismissProgressBar(Activity);

                        }
                        }
                    
                    
                });

            alertDialog.SetNegativeButton("Cancel",
             (see, ess) =>
             {
                 HideKeyboard(input);

             });

            alertDialog.Show();
            ShowKeyboard(input);
        }


                    private void ShowKeyboard(EditText userInput)
                    {
                        userInput.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
                        imm.ToggleSoftInput(ShowFlags.Forced, 0);
                    }

                    private void HideKeyboard(EditText userInput)
                    {
                        InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
                        imm.HideSoftInputFromWindow(userInput.WindowToken, 0);
                    }
    

    private void BtnTermsOfUse_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(TermsOfUseActivity));
            StartActivity(intent);
        }

        private void BtnPrivacyPolicy_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(PrivacyPolicyActivity));
            StartActivity(intent);
        }

        private void InviteViaSMS_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(PhoneContactsActivity));
            StartActivity(intent);
        }

        const int RequestInvite = 0;    
        private void BtnIFVEMAIL_Click(object sender, EventArgs e)
        {
            //var intent = new AppInviteInvitation.IntentBuilder(GetString(Resource.String.invitation_title))
            //    .SetMessage(GetString(Resource.String.invitation_message))
            //    .SetDeepLink(Android.Net.Uri.Parse(GetString(Resource.String.invitation_deep_link)))
            //    .Build();

            //StartActivityForResult(intent, RequestInvite);
        }

        private void BtnBlockedContacts_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(BlockedContactListActivity));
            StartActivity(intent);
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(ChangePasswordActivity));
            StartActivity(intent);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
           
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
               
            }

            return base.OnOptionsItemSelected(item);
        }

          
        }
}