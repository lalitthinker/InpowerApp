using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Newtonsoft.Json;
using PCL.Model;
using PCL.Service;
using Refractored.Controls;
using Square.Picasso;

using V7Toolbar = Android.Support.V7.Widget.Toolbar;
namespace InPowerApp.Activities
{
    [Activity(Label = "UserNewprofileActivity")]
    public class UserProfileDetailActivity : AppCompatActivity
    {

        


        ContactViewModel ContactObject;
        TextView txtEmail,txtCity,txtAboutMe,txtBlockContact;
        ImageView CircularimgUser;
        RelativeLayout userImagView;
        LinearLayout BlockedContactLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.UserProfileDetailLayout);
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            ContactObject = JsonConvert.DeserializeObject<ContactViewModel>(Intent.GetStringExtra("ContactObject"));

            CircularimgUser = FindViewById<ImageView>(Resource.Id.UserImageView);

            var ContactInfo = ContactRepository.GetContactbyUserId(Convert.ToInt64(ContactObject.ContactId));
            txtEmail = FindViewById<TextView>(Resource.Id.txtEmail);
            txtCity = FindViewById<TextView>(Resource.Id.txtCity);
            txtAboutMe = FindViewById<TextView>(Resource.Id.txtAboutMe);
            txtBlockContact = FindViewById<TextView>(Resource.Id.BlockContact);
            userImagView = FindViewById<RelativeLayout>(Resource.Id.linprofile_holder);
            BlockedContactLayout = FindViewById<LinearLayout>(Resource.Id.BlockContactLayout);
            var collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
          
            collapsingToolbar.Title = (ContactInfo.name == null ? "" : ContactInfo.name); ;

            BlockedContactLayout.Click += BlockedContactLayout_Click;
             if (ContactObject.IsBlock == true)
            {
                txtBlockContact.Text = "UnBlock";
            }
            else
            {
                txtBlockContact.Text = "Block";
            }
            
            txtEmail.Text ="Email:" + "\t" + "\t" + ( (ContactInfo.email == null) ? "" : ContactInfo.email);
            txtCity.Text = "City/State:" + "\t"+ "\t" + (((ContactInfo.city == null) ? "" : ContactInfo.city)+( (ContactInfo.state == null )? "" : "/"+ContactInfo.state));
            txtAboutMe.Text = "About Me:" + "\t" + "\t" + ((ContactInfo.Aboutme==null)?"": ContactInfo.Aboutme);
            //BlockUser.Click += BlockUser_Click;
            //ReportChatContact.Click += ReportChatContact_Click;
            CircularimgUser.Click += CircularimgUser_Click;
            if (ContactObject.ProfileImageUrl != null && ContactObject.ProfileImageUrl != "")
            {
                

                Picasso.With(this)
              .Load(ContactObject.ProfileImageUrl)
              .Resize(300, 300)
              .CenterCrop()
              .Placeholder(Resource.Drawable.default_profile)
              .Into(CircularimgUser);
               


            }
            else
            {
                CircularimgUser.SetBackgroundResource(Resource.Drawable.default_profile);
            }


        }

        private void BlockedContactLayout_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Block Contact");
            alert.SetMessage("Do you want to block this contact");

            alert.SetButton("OK", (c, ev) =>
            {
                BlockContact(ContactObject.ContactId);
            });
            alert.SetButton2("CANCEL", (c, ev) =>
            {
                alert.Dismiss();

            });
            alert.Show();
        }

        private void CircularimgUser_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);

            var fileAndpath = new Java.IO.File(
             Android.OS.Environment.GetExternalStoragePublicDirectory(
                 Android.OS.Environment.DirectoryPictures), System.IO.Path.Combine("Inpower", System.IO.Path.GetFileName(ContactObject.ProfileImageUrl)));
            Android.Net.Uri uri = Android.Net.Uri.FromFile(fileAndpath);
            intent.SetDataAndType(uri, "image/*");
            this.StartActivity(intent);
        }

     
     


        public async void BlockContact(long id)
        {
          

            CommonHelper.ShowProgressBar(this);

            var model = new userdetails
            {
                BlockUserID = id
            };

            var result = await new SettingService().PostBlockUserInterest(model);
            if(result.Status==1)
            {
                txtBlockContact.Text = "UnBlock";
                ChatConversationRepository.UpdateBlock(ContactObject.ChatConvId);
                CommonHelper.DismissProgressBar(this);
                Toast.MakeText(this, result.Message, ToastLength.Long).Show();
            }
            else
            {
                CommonHelper.DismissProgressBar(this);
                Toast.MakeText(this,result.Message, ToastLength.Long).Show();
            }
            CommonHelper.DismissProgressBar(this);
        }

        

        public void ReportChatContact_Click(object sender, EventArgs e)
        {
          
            //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            //Android.App.AlertDialog alert = dialog.Create();
            //alert.SetTitle("Chat");
            //alert.SetMessage("Do you want to report this message?");
            ////alert.SetIcon(Resource.Drawable.Aler);
            //alert.SetButton("OK", (c, ev) =>
            //{
            //    int position = 0;
            //    ReportChat();
            //});
            //alert.SetButton2("CANCEL", (c, ev) =>
            //{
            //    alert.Dismiss();

            //});
            //alert.Show();

        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }


        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount != 0)
            {
                FragmentManager.PopBackStack();// fragmentManager.popBackStack();
            }
            else
            {
                this.Finish();
                base.OnBackPressed();
            }
        }
        protected override void OnResume()
        {
            //  SupportActionBar.SetTitle(Resource.String.app_name);
            ////  SupportActionBar.SetIcon(Resource.Drawable.ic_Setting);
            base.OnResume();
        }


    }




}