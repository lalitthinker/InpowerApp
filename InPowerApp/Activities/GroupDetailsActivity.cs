using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Newtonsoft.Json;
using Square.Picasso;

namespace InPowerApp.Activities
{
    [Activity(Label = "GroupDetailsActivity")]
    public class GroupDetailsActivity : AppCompatActivity
    {
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        GroupMemberListAdapter mAdapter;
        public List<GroupMember> GroupMemberList;
        GroupModel GroupObject;
        ImageView GroupImageView;
        TextView GroupIntrest, GroupDescription, GroupType;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.GroupProfileDetailsLayout);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            GroupObject = JsonConvert.DeserializeObject<GroupModel>(Intent.GetStringExtra("GroupObject"));
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.ChatMemberList);
          
            mLayoutManager = new LinearLayoutManager(mRecyclerView.Context, LinearLayoutManager.Vertical, false);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            GroupImageView = FindViewById<ImageView>(Resource.Id.imgUsercircularsImage);
            GroupIntrest = FindViewById<TextView>(Resource.Id.GroupInterest);
            GroupDescription = FindViewById<TextView>(Resource.Id.GroupDescription);
            GroupType = FindViewById<TextView>(Resource.Id.GroupType);
            var collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            GroupImageView.Click += GroupImageView_Click;

            collapsingToolbar.Title = (GroupObject.GroupName == null ? "" : GroupObject.GroupName); ;

            //  SupportActionBar.Title = (GroupObject.GroupName == null ? "" : GroupObject.GroupName); ;
            //GroupIntrest.Text = "Interest:" + "\t" + "\t" + ((GroupObject.InterestId == null) ? "" : GroupObject.InterestId);
            GroupDescription.Text = "Description:" + "\t" + "\t" + (((GroupObject.GroupDescription == null) ? "" : GroupObject.GroupDescription));
            //   GroupType.Text = "Group Type:" + "\t" + "\t" + ((GroupObject.type == null) ? "" : GroupObject.type);

             loadContactAdapter();

           



            if (GroupObject.GroupPictureUrl != null && GroupObject.GroupPictureUrl != "")
            {

                Picasso.With(this)
              .Load(GroupObject.GroupPictureUrl)
              .Resize(300, 300)
              .CenterCrop()
              .Placeholder(Resource.Drawable.default_profile)
              .Into(GroupImageView);
            }
            else
            {
                GroupImageView.SetBackgroundResource(Resource.Drawable.default_profile);
            }
        }

        private void GroupImageView_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);

            var fileAndpath = new Java.IO.File(
             Android.OS.Environment.GetExternalStoragePublicDirectory(
                 Android.OS.Environment.DirectoryPictures), System.IO.Path.Combine("Inpower", System.IO.Path.GetFileName(GroupObject.GroupPictureUrl)));
            Android.Net.Uri uri = Android.Net.Uri.FromFile(fileAndpath);
            intent.SetDataAndType(uri, "image/*");
            this.StartActivity(intent);
        }

        public void loadContactAdapter()
        {
            try
            {
                GroupMemberList = GroupRepository.GroupMemberList(GroupObject.GroupId);
                
                if (GroupMemberList != null && GroupMemberList.Count > 0)
                {
                    mAdapter = new GroupMemberListAdapter(GroupMemberList, this);
                   // mAdapter.ListReload += ListReload;
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
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