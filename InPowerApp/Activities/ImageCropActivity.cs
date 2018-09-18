using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using InPowerApp.Common;
using InPowerApp.Fragments;
using Java.Lang;

namespace InPowerApp.Activities
{
    [Activity(Label = "Edit Profile")]
    public class ImageCropActivity : AppCompatActivity
    {
        ImageCropFragment mCurrentFragment;
        CropImageViewOptions mCropImageViewOptions = new CropImageViewOptions();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CropImageActivity);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(BitmapHelper._file);
            // Create your application here
            SupportFragmentManager.BeginTransaction()
                  .Replace(Resource.Id.container, ImageCropFragment.NewInstance(CropDemoPreset.CIRCULAR, contentUri))
                  .Commit();
        }
        public void SetCurrentFragment(ImageCropFragment fragment)
        {
            mCurrentFragment = fragment;
        }

        public void SetCurrentOptions(CropImageViewOptions options)
        {
            mCropImageViewOptions = options;
           // UpdateDrawerTogglesByOptions(options);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_crop, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                this.Finish();

           

            if (mCurrentFragment != null && mCurrentFragment.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public void SetCropImage(Bitmap CropImage)
        {
            BitmapHelper.bitmap = CropImage;
            this.Finish();
          

        }
    }
  
}