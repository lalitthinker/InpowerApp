using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.ListAdapter;
using InPowerApp.Model;

namespace InPowerApp.Activities
{
    [Activity(Label = "PhoneContactsActivity")]
    public class PhoneContactsActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.SearchView _searchView;
        List<PhoneContactModel> PhoneContactList;
        private PhoneContactAdapter _adapter;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PhoneContactLayout);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            mRecyclerView = (RecyclerView)FindViewById(Resource.Id.PhoneContactList);
            mLayoutManager = new LinearLayoutManager(mRecyclerView.Context, LinearLayoutManager.Vertical, false);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportActionBar.SetHomeButtonEnabled(true);
            FillContacts();

        }

        protected async override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.InviteViaSms);
            base.OnResume();
            await Task.Run(() =>
            {
                this.RunOnUiThread(() =>
                {
                    FillContacts();
                });

            });

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
                FragmentManager.PopBackStack();
            }
            else
            {
                this.Finish();
                base.OnBackPressed();
            }
        }


        public void FillContacts()
        {

            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;

            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.Contacts.InterfaceConsts.PhotoId,
                ContactsContract.CommonDataKinds.Phone.Number,
                ContactsContract.Contacts.InterfaceConsts.PhotoThumbnailUri
            };

            var loader = new CursorLoader(this, uri, projection, null, null, null);
            var cursor = (ICursor)loader.LoadInBackground();

            PhoneContactList = new List<PhoneContactModel>();

            if (cursor.MoveToFirst())
            {
                do
                {
                    PhoneContactList.Add(new PhoneContactModel
                    {
                        contactId = cursor.GetLong(cursor.GetColumnIndex(projection[0])),
                        name = cursor.GetString(cursor.GetColumnIndex(projection[1])),
                        photoId = cursor.GetString(cursor.GetColumnIndex(projection[2])),
                        number = cursor.GetString(cursor.GetColumnIndex(projection[3])),
                        photo = cursor.GetString(cursor.GetColumnIndex(projection[4]))
                    });
                } while (cursor.MoveToNext());
            }

            if (PhoneContactList != null)
            {
                _adapter = new PhoneContactAdapter(this, PhoneContactList);
                //_adapter.ItemClick += _adapter_ItemClick;
                mRecyclerView.SetAdapter(_adapter);
                _adapter.NotifyDataSetChanged();
            }

        }




        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menuSearch, menu);
            if (menu != null)
            {
                menu.FindItem(Resource.Id.action_menuSearchSearch).SetVisible(true);
                var item = menu.FindItem(Resource.Id.action_menuSearchSearch);
                MenuItemCompat.SetOnActionExpandListener(item, new SearchViewExpandListener(_adapter));

                var searchItem = MenuItemCompat.GetActionView(item);
                _searchView = searchItem.JavaCast<Android.Support.V7.Widget.SearchView>();
                _searchView.QueryTextChange += (sender, e) => _adapter.Filter.InvokeFilter(e.NewText);
            }
            return base.OnCreateOptionsMenu(menu);
        }


        private class SearchViewExpandListener
#pragma warning disable CS0618 // Type or member is obsolete
                   : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
#pragma warning restore CS0618 // Type or member is obsolete
        {
            private readonly IFilterable _adapter;

            public SearchViewExpandListener(IFilterable adapter)
            {
                _adapter = adapter;
            }

            public bool OnMenuItemActionCollapse(IMenuItem item)
            {
                _adapter.Filter.InvokeFilter("");
                return true;
            }

            public bool OnMenuItemActionExpand(IMenuItem item)
            {
                return true;
            }




        }
    }
}