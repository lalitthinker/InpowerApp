using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Iid;
using InPowerApp.Activities;
using InPowerApp.Common;
using InPowerApp.Fragments;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace InPowerApp
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
       
        private String[] navMenuTitles;
    private TypedArray navMenuIcons;
    public static List<NavDrawerItem> navDrawerItems;
    ListView mDrawerList;
    TextView msgText;
    DrawerLayout drawer;
    Android.Support.V4.App.Fragment fragment = null;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        try
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetTitle(Resource.String.app_name);

            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();



            AppCenter.Start("4d956e9e-e46b-41bf-8def-b32613ebb6c2", typeof(Analytics), typeof(Crashes));
            var ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Add(Resource.Id.content_frame, new ChatGroupContactFragment(0));
            ft.Commit();
          

            setProfileDetails();


        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            Log.Debug("From : Main Activity OnCreate ", e.Message);
        }
    }




    private void setProfileDetails()
    {
        UserProfile loginUserDetails = UserProfileRepository.GetUserProfile(CommonHelper.GetUserId());

        NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

        navigationView.SetNavigationItemSelectedListener(this);

        View header = navigationView.GetHeaderView(0);

        ImageView profileImage = header.FindViewById<ImageView>(Resource.Id.ProfileImageView);
        TextView userName = header.FindViewById<TextView>(Resource.Id.txtProfileUserName);
        TextView userEmail = header.FindViewById<TextView>(Resource.Id.txtProfileUserEmail);

        userName.Text = loginUserDetails.FirstName + " " + loginUserDetails.LastName;
        userEmail.Text = loginUserDetails.Email;

        if (loginUserDetails.ProfileImageUrl != null)
        {
            CommonHelper.SetImageOnUIImageView(profileImage, loginUserDetails.ProfileImageUrl, this, 400, 400);

        }

        //FloatingActionButton fab = header.FindViewById<FloatingActionButton>(Resource.Id.fab);
        //fab.Click += FabOnClick;
        //TextView username = FindViewById<TextView>(Resource.Id.txtProfileUserName);
        //username.Text = loginUserDetails.FirstName + " " + loginUserDetails.LastName;
        //TextView userEmail = FindViewById<TextView>(Resource.Id.txtProfileUserEmail);
        //userEmail.Text = loginUserDetails.Email;

        profileImage.Click += ProfileImage_Click;
    }


    private void FabOnClick(object sender, EventArgs eventArgs)
    {
        View view = (View)sender;
        Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
    }

    private void ProfileImage_Click(object sender, EventArgs e)
    {
        this.Finish();
        StartActivity(new Intent(Application.Context, typeof(UpdateProfileActivity)));
    }

    int id;
    int indexer = 0;
    public bool OnNavigationItemSelected(IMenuItem menuItem)
    {

        id = menuItem.ItemId;

        var ft = SupportFragmentManager.BeginTransaction();

        if (id == Resource.Id.nav_chat)
        {
            indexer = 0;
            SelectedTabForChat.TabName = "Chat";
            fragment = new ChatGroupContactFragment(0);
        }
        else if (id == Resource.Id.nav_contact)
        {
            indexer = 1;
            SelectedTabForChat.TabName = "Contact";
            fragment = new ChatGroupContactFragment(1);

        }
        else if (id == Resource.Id.nav_bookshelf)
        {
            indexer = 2;
            fragment = new BooksShelfFragment();
        }
        else if (id == Resource.Id.nav_setting)
        {
            indexer = 3;
            fragment = new SettingsFragment();
        }
        else if (id == Resource.Id.nav_logout)
        {
            indexer = 4;
            CommonHelper.ClearPreferences();
            this.Finish();
            StartActivity(new Intent(Application.Context, typeof(LoginForm)));
        }

        drawer.CloseDrawer(GravityCompat.Start);

        if (fragment != null)
        {
            ft.Replace(Resource.Id.content_frame, fragment);
            ft.Commit();

        }


        return true;
    }

    public override void OnBackPressed()
    {
        if (indexer == 0)
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Exit");
            alert.SetMessage("Do you want to Exit?");
            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {
                this.FinishAffinity();

            });

            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
                alert.Dispose();
            });


            Dialog dialog = alert.Create();
            dialog.Show();
        }


        else
        {

            var ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Add(Resource.Id.content_frame, new ChatGroupContactFragment(0));
            ft.Commit();
            indexer = 0;

        }

    }


}
}

