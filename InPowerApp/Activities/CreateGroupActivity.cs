using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uri = Android.Net.Uri;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InPowerApp.ListAdapter;
using Java.IO;
using Newtonsoft.Json;
using PCL.Model;
using PCL.Service;
using Refractored.Controls;
using Android.Graphics.Drawables;
using InPowerApp.Common;
using Square.Picasso;
using Android.Content.PM;

namespace InPowerApp.Activities
{

    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;

      
    }


    [Activity(Label = "CreateGroupActivity")]
    public class CreateGroupActivity : AppCompatActivity
    {
        List<string> _lstDataItem = new List<string>();
        string[] interestString;
        Spinner drpInterest, drpGroup;
        protected static int CAMERA_REQUEST = 1337;
        public static readonly int PickImageId = 1000;

        Android.App.AlertDialog dlgAlert;

        private List<KeyValuePair<string, string>> drpGroupList;
        private List<KeyValuePair<string, string>> drpInterestList;
        Button btngroup;
        EditText txtGroupName, txtGroupDesc;
        ImageView GroupIcon;
        RadioGroup SelectGroup;
        int grouptype = 1;

        Uri contentUri;
        string GroupImageURL;
        string filePath;
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
                    .Resize(200, 150).Into(GroupIcon);

                    BitmapHelper.bitmap = BitmapHelper._file.Path.LoadAndResizeBitmap(width, height);
                    try
                    {
                        using (var os = new System.IO.FileStream(System.IO.Path.Combine(BitmapHelper._dir.ToString(), String.Format("myProfile_{0}.jpg", Guid.NewGuid())), System.IO.FileMode.CreateNew))
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
                if (BitmapHelper._file.AbsolutePath != null)
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
                    .Resize(200, 150).Into(GroupIcon);


                    BitmapHelper.bitmap = BitmapHelper._file.Path.LoadAndResizeBitmap(width, height);
                    try
                    {
                        using (var os = new System.IO.FileStream(System.IO.Path.Combine(BitmapHelper._dir.ToString(), String.Format("myPhoto_{0}.jpg", Guid.NewGuid())), System.IO.FileMode.CreateNew))
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

        private async void uploadMedia(string filePath, string mediaType)
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
                        var model = new GroupRequestViewModel
                        {
                            Name = txtGroupName.Text,
                            Description = txtGroupDesc.Text,
                            GroupType = (GroupType)grouptype,
                            InterestId = Convert.ToInt32(drpInterestList[drpInterest.SelectedItemPosition].Value),
                            IsPrivate = privategroup.Checked,
                            PictureUrl = url
                        };
                        Intent intent = new Intent(this, typeof(SelectGroupContactActivity));
                        intent.PutExtra("GroupObject", JsonConvert.SerializeObject(model));
                        StartActivity(intent);
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

        CheckBox privategroup;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateGrouplayout);

            _lstDataItem.Add(GetString(Resource.String.Dialog_TakefromCamera));
            _lstDataItem.Add(GetString(Resource.String.Dialog_SelectfromGallery));
            _lstDataItem.Add(GetString(Resource.String.Dialog_Cancel));

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            drpInterest = FindViewById<Spinner>(Resource.Id.drpInterest);
            // drpGroup = FindViewById<Spinner>(Resource.Id.drpGroup);
            //  btngroup = FindViewById<Button>(Resource.Id.btnGroup);
            txtGroupName = FindViewById<EditText>(Resource.Id.txtGroupName);
            txtGroupDesc = FindViewById<EditText>(Resource.Id.txtGroupDescription);
            GroupIcon = FindViewById<ImageView>(Resource.Id.imgGroupImage);
            SelectGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup);
            GroupIcon.Click += GroupIcon_Click;
            privategroup = FindViewById<CheckBox>(Resource.Id.chkPrivate);
            //  btngroup.Click += Btngroup_Click;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            loadDropDown();
            SelectGroup.CheckedChange += SelectGroup_CheckedChange;
        }

        private void SelectGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            //switch (e.CheckedId)
            //{ 
            //    case Resource.Id.radioGeneralGroup:
            //        grouptype = 1;
            //        break;
            //    case Resource.Id.radioBookClub:
            //        grouptype = 2;
            //        break;
            //    case Resource.Id.radioMaterMind:
            //        grouptype = 3;
            //        break;
            //}
        }

        private void GroupIcon_Click(object sender, EventArgs e)
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
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }
            if (item == 0)
            {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                BitmapHelper._file = new File(BitmapHelper._dir, String.Format("myPhotoTemp_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(BitmapHelper._file));
                StartActivityForResult(intent, CAMERA_REQUEST);

            }
            else if (item == 1)
            {
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

        private void Btngroup_Click(object sender, EventArgs e)
        {

        }


        private void loadDropDown()
        {
            GetAllInterestBooks();
            //  getAllGroup();
        }



        private void GetAllInterestBooks()
        {
            drpInterestList = new List<KeyValuePair<string, string>>();
            var result = new CommonService().GetInterest().Result;
            if (result.Status == 1)
            {

                var InterestResult = JsonConvert.DeserializeObject<List<InterestResponseViewModel>>(result.Response.ToString());
                interestString = new string[InterestResult.Count + 1];
                int i = 1;
                interestString[0] = "--Select Interest--";
                drpInterestList.Add(new KeyValuePair<string, string>("Select Group", "0"));
                foreach (var item in InterestResult)
                {
                    drpInterestList.Add(new KeyValuePair<string, string>(item.Name, item.InterestId.ToString()));
                    interestString[i] = item.Name;
                    i++;
                }

                ArrayAdapter interestArray = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, interestString);
                drpInterest.Adapter = interestArray;
            }
            else
            {
                Toast.MakeText(this, result.Message, ToastLength.Long).Show();
            }

        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menuOK, menu);
            if (menu != null)
            {
                menu.FindItem(Resource.Id.action_menuOKOK).SetVisible(true);
            }
            return base.OnCreateOptionsMenu(menu);
        }



        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_menuOKOK)
            {
                CreateGroup();
            }

            else if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void CreateGroup()
        {
            Drawable icon_error = Resources.GetDrawable(Resource.Drawable.alert);
            icon_error.SetBounds(0, 0, 50, 50);
            if (txtGroupName.Text.Trim() == "")
            {
               
                txtGroupName.SetError("Group Name is empty", icon_error);
                txtGroupName.RequestFocus();
                return;
            }
            if (drpInterest.SelectedItemId == 0)
            {
                Toast.MakeText(this, "Please Select Interest", ToastLength.Long).Show();
                return;
            }

            if(txtGroupDesc.Text == "")
            {

                txtGroupDesc.RequestFocus();
                txtGroupDesc.SetError("Group Description is empty", icon_error);
                return;
            }

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        uploadMedia(filePath, mediaType);


                    }
                    else
                    {
                        var model = new GroupRequestViewModel
                        {
                            Name = txtGroupName.Text,
                            Description = txtGroupDesc.Text,
                            GroupType = (GroupType)grouptype,
                            InterestId = Convert.ToInt32(drpInterestList[drpInterest.SelectedItemPosition].Value),
                            IsPrivate = privategroup.Checked
                        };
                        Intent intent = new Intent(this, typeof(SelectGroupContactActivity));
                        intent.PutExtra("GroupObject", JsonConvert.SerializeObject(model));
                        StartActivity(intent);
                    }
                
              
          
            try
            {
              
              

                //if (txtGroupName.Text== "")
                //{
                //    txtGroupDesc.RequestFocus();
                //    txtGroupDesc.SetError("Group Name is empty", icon_error);

                //}
                //    if (txtGroupDesc.Text == "")
                //    {
                //    txtGroupDesc.RequestFocus();
                //    txtGroupDesc.SetError("Group Description is empty", icon_error);

                //}
                   
                  

                //    if (drpInterest.SelectedItem.ToString() == null)
                //    {
                //        Toast.MakeText(this, "Please Select Interest", ToastLength.Long).Show();
                //    }
                


            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.CreateGroup);
            base.OnResume();
        }
    }
}