using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Java.IO;
using Newtonsoft.Json;
using PCL.Common;
using PCL.Model;
using PCL.Service;
using Plugin.Connectivity;
using Square.Picasso;
using Uri = Android.Net.Uri;

namespace InPowerApp.Activities
{
    [Activity(Label = "Welcome to InPower")]
    public class ComplateYourProfileActivity : AppCompatActivity
    {
        protected const int REQUEST_LOCATION = 0x1;
        Android.App.AlertDialog dlgAlert;
        EditText txtZipCode, txtCity, txtState, txtAboutMe, txtCountry;
        //AutoCompleteTextView txtCountry;
        ImageView profileChange;
        protected static int CAMERA_REQUEST = 1337;
        public static readonly int PickImageId = 1000;
        List<string> _lstDataItem = new List<string>();
        Button btnContinueCYP;
        ProgressDialog progressDialog;
        Uri contentUri;
        string ProfileImageURL;
        string filePath;
        UserProfile UserProfile;
        LinearLayout hidekeybordlayout;
        string mediaType;
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
           
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                if (data.Data != null)
                {
                    Android.Net.Uri uri = data.Data;

                    filePath = CommonHelper.GetRealPathFromURI(this, data.Data);
                    int height = 200;
                    int width = 200;
                    BitmapHelper._file = new File(filePath);

                    Picasso.With(this).Load(BitmapHelper._file)
                        .Transform(new CircleTransformation())
                         .CenterCrop()
                    .Resize(200, 150).Into(profileChange);

                    BitmapHelper.bitmap = BitmapHelper._file.Path.LoadAndResizeBitmap(width, height);
                    try
                    {
                        using (var os = new System.IO.FileStream(BitmapHelper._dir + String.Format("myProfile_{0}.jpg", Guid.NewGuid()), System.IO.FileMode.CreateNew))
                        {
                            BitmapHelper.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, os);
                            filePath = os.Name;
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                    }

                    mediaType = "Photo";
                }
            }

            if ((requestCode == CAMERA_REQUEST))
            {
                if (BitmapHelper._file.AbsolutePath!= null)
                {
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    Android.Net.Uri contentUri = Android.Net.Uri.FromFile(BitmapHelper._file);
                    mediaScanIntent.SetData(contentUri);
                    //  profileChange.SetImageURI(contentUri);
                    int height = 200;
                    int width = 200;

                    Picasso.With(this).Load(contentUri)
                        .Transform(new CircleTransformation())
                         .CenterCrop()
                    .Resize(200, 150).Into(profileChange);


                    BitmapHelper.bitmap = BitmapHelper._file.Path.LoadAndResizeBitmap(width, height);
                    try
                    {
                        using (var os = new System.IO.FileStream(BitmapHelper._dir + String.Format("myProfile_{0}.jpg", Guid.NewGuid()), System.IO.FileMode.CreateNew))
                        {
                            BitmapHelper.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, os);
                            filePath = os.Name;
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                    }

                    mediaType = "Photo";
                }
            }
           

        }

        private  async void uploadMedia(string filePath,string mediaType, UserRegisterResponseViewModel model)
        {
            var mediaName = System.IO.Path.GetFileName(filePath); //AWSUploader.SetMediaName (mediaType);
            var url = "";
            try
            {
                // BTProgressHUD.Show("Processing media..", maskType: ProgressHUD.MaskType.Black);
                if (mediaType == "Photo")
                    await AWSUploader.AWSUploadImage(filePath, mediaName);
                else
                    await AWSUploader.AWSUploadAudioVideo(filePath, mediaName, mediaType);
                url = AWSUploader.GetMediaUrl(mediaType) + mediaName;

               
                try
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        Toast.MakeText(this, "Check Your Internet Connection", ToastLength.Long).Show();
                       
                    }
                    else
                    {
                     var     Result = await new AccountService().Registration(new UserRegisterRequestViewModel
                        {
                            Email = model.Email,
                            Password = model.Password,

                            ZipCode = model.ZipCode,
                            City = model.City,
                            State = model.State,
                            Country = model.Country,
                            AboutMe = model.AboutMe,
                            ProfileImageUrl = url,

                        }, GlobalConstant.AccountUrls.RegisterServiceUrl);

                        if (Result.Status == 1)
                        {


                            var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());
                            var UserprofileRepo = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());
                            UserprofileRepo.ZipCode = modelReporeg.ZipCode;
                            UserprofileRepo.City = modelReporeg.City;
                            UserprofileRepo.State = modelReporeg.State;
                            UserprofileRepo.Country = modelReporeg.Country;

                            UserprofileRepo.AboutMe = modelReporeg.AboutMe;
                            UserprofileRepo.ProfileImageUrl = modelReporeg.ProfileImageUrl;

                         

                            UserProfileRepository.UpdateUserProfile(UserprofileRepo);

                           

                        }

                    }

                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                   

                }

               
            }
            catch (Java.Lang.Exception e)
            {

                Toast.MakeText(this, e.Message, ToastLength.Long).Show();

            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ComplateYourProfilelayout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            toolbar.TextAlignment = Android.Views.TextAlignment.Center;
            
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetTitle(Resource.String.WelcometoInPower);
            SupportActionBar.SetSubtitle(Resource.String.SignUp);

            _lstDataItem.Add(GetString(Resource.String.Dialog_TakefromCamera));
            _lstDataItem.Add(GetString(Resource.String.Dialog_SelectfromGallery));
            _lstDataItem.Add(GetString(Resource.String.AlrtDialogBuilder_itemCancel));


            //  UserProfile = JsonConvert.DeserializeObject<UserProfile>(Intent.GetStringExtra("UserProfile"));
            hidekeybordlayout = FindViewById<LinearLayout>(Resource.Id.hidekeybordlayout);
            btnContinueCYP = FindViewById<Button>(Resource.Id.btnContinueCYP);
            txtZipCode = FindViewById<EditText>(Resource.Id.txtZipCode);
            txtCity = FindViewById<EditText>(Resource.Id.txtCity);
            txtState = FindViewById<EditText>(Resource.Id.txtState);
            txtCountry = FindViewById<EditText>(Resource.Id.txtCountry);
            txtAboutMe = FindViewById<EditText>(Resource.Id.txtAboutMe);
            profileChange = (ImageView)FindViewById(Resource.Id.imgProfileImage);
           
          //  if (!string.IsNullOrEmpty(UserProfile.ProfileImageUrl))
          //  {
          //     CommonHelper.SetImageOnUIImageView(profileChange, UserProfile.ProfileImageUrl, this, 400, 400);
          //  }

            profileChange.Click += ProfileChange_Click;

            Drawable iconLocation = Resources.GetDrawable(Resource.Drawable.location24);
            Drawable iconAboutMe = Resources.GetDrawable(Resource.Drawable.username24);

            txtZipCode.SetCompoundDrawablesWithIntrinsicBounds(iconLocation, null, null, null);
            txtCity.SetCompoundDrawablesWithIntrinsicBounds(iconLocation, null, null, null);
            txtState.SetCompoundDrawablesWithIntrinsicBounds(iconLocation, null, null, null);
            txtCountry.SetCompoundDrawablesWithIntrinsicBounds(iconLocation, null, null, null);
            txtAboutMe.SetCompoundDrawablesWithIntrinsicBounds(iconAboutMe, null, null, null);

            btnContinueCYP.Click += BtnContinueCYP_Click;
            hidekeybordlayout.Touch += Hidekeybordlayout_Touch;

            var requiredPermissions = new String[]
         {

                    Manifest.Permission.Internet,
                     Manifest.Permission.WriteExternalStorage,
                      Manifest.Permission.ReadExternalStorage,
                      Manifest.Permission.Camera,
                    Manifest.Permission.ReadContacts
         };
            ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);

        }

        private void Hidekeybordlayout_Touch(object sender, View.TouchEventArgs e)
        {
            CommonHelper.Hidekeyboard(this, Window);
        }

        private void ProfileChange_Click(object sender, EventArgs e)
        {
            methodInvokeAlertDialogWithListView();
        }

        public void methodInvokeAlertDialogWithListView()
        {

            dlgAlert = (new Android.App.AlertDialog.Builder(this)).Create();
            dlgAlert.SetTitle("Add Picture");
            var listView = new ListView(this);
            listView.Adapter = new AlertListViewAdapter(this, _lstDataItem);
            listView.ItemClick += ListView_ItemClick;
            dlgAlert.SetView(listView);
            dlgAlert.Show();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int item = e.Position;
            if (item == 0)
            {
                if (IsThereAnAppToTakePictures())
                {
                    CommonHelper.CreateDirectoryForPictures();
                }

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                BitmapHelper._file = new File(BitmapHelper._dir, String.Format("myProfileTemp_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(BitmapHelper._file));
                StartActivityForResult(intent, CAMERA_REQUEST);

            }
            else if (item == 1)
            {
                if (IsThereAnAppToTakePictures())
                {
                    CommonHelper.CreateDirectoryForPictures();
                }
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
            }

            dlgAlert.Dismiss();

        }

        private void CreateDirectoryForPictures()
        {
            BitmapHelper._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "Inpower");
            if (!BitmapHelper._dir.Exists())
            {
                BitmapHelper._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }


        public void clearAll()
        {
            txtZipCode.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtCountry.Text = "";
            txtAboutMe.Text = "";
        }


        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_skip, menu);
            if (menu != null)
            {
                menu.FindItem(Resource.Id.action_menuSkip).SetVisible(true);
                //menu.FindItem(Resource.Id.action_ADDCONTACTS).SetVisible(false);
                //menu.FindItem(Resource.Id.action_SEARCH).SetVisible(false);
                //menu.FindItem(Resource.Id.action_CREATEGROUPS).SetVisible(false);
            }
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                var intent = new Intent(this, typeof(LoginForm));
                StartActivity(intent);
            }
            if (item.ItemId == Resource.Id.action_menuSkip)
            {
                var intent = new Intent(this, typeof(AddBooksToWishListActivity));
                StartActivity(intent);
            }
            return base.OnOptionsItemSelected(item);
        }


        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(LoginForm));
            StartActivity(intent);
        }


        private async void BtnContinueCYP_Click(object sender, EventArgs e)
        {
            InpowerResult Result = null;
            try
            {
          
                Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
                icon_error.SetBounds(0, 0, 50, 50);

                if (txtZipCode.Text != "")
                {
                    if (txtCity.Text != "")
                    {
                        if (txtState.Text != "")
                        {
                            if (txtCountry.Text != "")
                            {
                                if (txtAboutMe.Text != "")
                                {
                                    if (CrossConnectivity.Current.IsConnected)
                                    {

                                        CommonHelper.ShowProgressBar(this);
                                      
                                       
                                            var UserprofileRepo = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());

                                            Result = await new AccountService().Registration(new UserRegisterRequestViewModel
                                            {
                                                Email = UserprofileRepo.Email,
                                                Password = UserprofileRepo.Password,

                                                ZipCode = Convert.ToInt32(txtZipCode.Text),
                                                City = txtCity.Text,
                                                State = txtState.Text,
                                                Country = txtCountry.Text,
                                                AboutMe = txtAboutMe.Text,
                                                ProfileImageUrl = ProfileImageURL,

                                            }, GlobalConstant.AccountUrls.RegisterServiceUrl);

                                            if (Result.Status == 1)
                                            {
                                          

                                            var modelReporeg = JsonConvert.DeserializeObject<UserRegisterResponseViewModel>(Result.Response.ToString());

                                                UserprofileRepo.ZipCode = modelReporeg.ZipCode;
                                                UserprofileRepo.City = modelReporeg.City;
                                                UserprofileRepo.State = modelReporeg.State;
                                                UserprofileRepo.Country = modelReporeg.Country;

                                                UserprofileRepo.AboutMe = modelReporeg.AboutMe;

                                               // UserprofileRepo.ProfileImageUrl = UserProfile.ProfileImageUrl;

                                            if (!string.IsNullOrEmpty(filePath))
                                            {
                                                uploadMedia(filePath, mediaType, modelReporeg);

                                            }

                                           

                                           

                                            UserProfileRepository.UpdateUserProfile(UserprofileRepo);

                                                Toast.MakeText(this, "Profile Complete Successfully", ToastLength.Short).Show();
                                                CommonHelper.DismissProgressBar(this);
                                                clearAll();
                                                this.Finish();
                                               
                                            var intent = new Intent(this, typeof(WhatsAreYourInterestsActivity));
                                            intent.AddFlags(ActivityFlags.SingleTop);

                                            StartActivity(intent);

                                        }

                                            else
                                            {
                                                CommonHelper.DismissProgressBar(this);
                                                Toast.MakeText(this, Result.Message, ToastLength.Short).Show();
                                                return;
                                            }
                                    }

                                    else
                                    {
                                        Toast.MakeText(this, "You're not connected to a Network", ToastLength.Short).Show();
                                        return;
                                    }
                                
                                  
                                   

                                }
                                else
                                {
                                    txtAboutMe.RequestFocus();
                                    txtAboutMe.SetError("Please Enter About Me First", icon_error);
                                }
                            }
                            else
                            {
                                txtCountry.RequestFocus();
                                txtCountry.SetError("Please Enter Country First", icon_error);
                            }
                        }
                        else
                        {
                            txtState.RequestFocus();
                            txtState.SetError("Please Enter State First", icon_error);
                        }
                    }
                    else
                    {
                        txtCity.RequestFocus();
                        txtCity.SetError("Please Enter City First", icon_error);
                    }
                }
                else
                {
                    txtZipCode.RequestFocus();
                    txtZipCode.SetError("Please Enter Zip Code First", icon_error);
                }


            }
            catch (Exception ex)
            {
                CommonHelper.DismissProgressBar(this);

                string ErrorMsg = ex.ToString();
                Toast.MakeText(this, ErrorMsg, ToastLength.Long).Show();
            }
        }


    }
}
