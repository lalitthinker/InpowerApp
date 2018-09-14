
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using PCL.Service;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InPowerApp.Activities
{
    [Activity(Label = "BlockedContactListActivity")]
    public class BlockedContactListActivity : AppCompatActivity
    {
        List<ChatConversation> BlockedContactList = new List<ChatConversation>();
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        BlockedContactListAdapter mAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BlockedContactListLayout);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.BlockedContactList);

            mLayoutManager = new LinearLayoutManager(mRecyclerView.Context, LinearLayoutManager.Vertical, false);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            loadLocalData();
            LoadBlockedContactListAsync();


        }




        public void loadLocalData()
        {
          

            if (BlockedContactList != null && BlockedContactList.Count > 0)
            {
                mAdapter = new BlockedContactListAdapter(BlockedContactList, this);
                // mAdapter.ListReload += ListReload;
                mRecyclerView.SetAdapter(mAdapter);
                mAdapter.NotifyDataSetChanged();
            }
        }

        private async Task LoadBlockedContactListAsync()
        {
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var result = await new SettingService().getBlockedContactAll();
                    loadLocalData();
                }
                else
                {
                    loadLocalData();
                }



            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No internet connection", ToastLength.Long).Show();
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
            SupportActionBar.SetTitle(Resource.String.BlockedContacts);
            ////  SupportActionBar.SetIcon(Resource.Drawable.ic_Setting);
            base.OnResume();
        }


    }

}