using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Newtonsoft.Json;
using PCL.Common;
using PCL.Model;
using PCL.Service;

using static Android.Views.View;

namespace InPowerApp.Activities
{
    [Activity(Label = "Welcome to InPower")]
    public class AddMoreContactsActivity : AppCompatActivity, IOnLayoutChangeListener
    {
       
        bool loadList = true;
        bool searchList = false;
        RecyclerView mRecyclerView;
        AddMoreContactsAdapter mAdapter;
        public List<UserProfile> ContactList;
        PaginationModel paginationModel = new PaginationModel();
        private Android.Support.V7.Widget.SearchView _searchView;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddContactslayout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            SupportActionBar.SetTitle(Resource.String.AddContacts);
            paginationModel.Status = 2;
          
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.ContactList);
            if (mRecyclerView != null)
            {
                mRecyclerView.HasFixedSize = true;
                var layoutManager = new LinearLayoutManager(this);
                var onScrollListener = new XamarinRecyclerViewOnScrollListener(layoutManager);
                onScrollListener.LoadMoreEvent += (object sender, bool loadCalled) =>
                {
                    if (loadCalled)
                        if (loadList)
                        {
                            Toast.MakeText(this, "Loading More Contacts...", ToastLength.Long).Show();
                            if (InternetConnectivityModel.CheckConnection(this))
                            {
                                if (String.IsNullOrEmpty(paginationModel.SearchText))
                                    LoadMoreContacts();
                            }
                            else
                            {
                                // LoadMoreBooksOffline();
                            }
                        }
                };
                mRecyclerView.AddOnScrollListener(onScrollListener);
                mRecyclerView.SetLayoutManager(layoutManager);
            }
            paginationModel.SkipRecords = 0;
            if (InternetConnectivityModel.CheckConnection(this, true))
            {
                LoadServerContacts();
            }
        }

       

        private async void LoadServerContactsFromSearch()
        {
            try
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                var result = await new ContactsService().GetContactsAll(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    ContactList = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                    loadContactAdapter(ContactList);
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }
        private async void LoadServerContacts(string SearchText = null)
        {
            try
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                paginationModel.SearchText = SearchText;
                var result = await new ContactsService().GetContactsAll(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    ContactList = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                    loadContactAdapter(ContactList);
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }
        private async void LoadMoreContacts()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;
                var result = await new ContactsService().GetContactsAll(paginationModel);
                if (result.Status == 1)
                {
                    ContactList = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                    try
                    {
                        if (ContactList != null && ContactList.Count > 0)
                        {
                            foreach (var item in ContactList)
                            {
                                mAdapter.add(item);
                            }
                            mAdapter.NotifyItemRangeInserted(paginationModel.SkipRecords, paginationModel.TakeRecords);
                            loadList = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "No More Contacs", ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }

        public void loadContactAdapter(List<UserProfile> ContactList)
        {
            try
            {
                //  ContactList = BookRepository.GetBooks(BookStatus.Removed, ContactList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (ContactList != null && ContactList.Count > 0)
                {
                    mAdapter = new AddMoreContactsAdapter(ContactList, this);
                    mAdapter.ListReload += ListReload;
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }


        public void RealodloadContactAdapter(int contactId)
        {
            try
            {
                UserProfile contactItem = ContactList.Where(a => a.UserId == contactId).FirstOrDefault();
                int indexOfRemoved = ContactList.IndexOf(contactItem);
                if (ContactList != null && ContactList.Count > 0)
                {
                    mAdapter.remove(contactItem);
                    mAdapter.NotifyItemRemoved(indexOfRemoved);
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private void ListReload(object sender, int bookId)
        {
            if (bookId != 0)
                RealodloadContactAdapter(bookId);
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_contactsListMenu, menu);
            if (menu != null)
            {
                menu.FindItem(Resource.Id.action_menuOKOK).SetVisible(true);

                var searchItems = menu.FindItem(Resource.Id.action_ContactSearch);

                searchItems.SetVisible(true);

                //MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener((IFilter)mAdapter));

                var searchItem = MenuItemCompat.GetActionView(searchItems);
                _searchView = searchItem.JavaCast<Android.Support.V7.Widget.SearchView>();
                _searchView.SubmitButtonEnabled = true;

                _searchView.QueryTextChange += (s, e) =>
                {
                    if (String.IsNullOrEmpty(e.NewText))
                    {
                        paginationModel.SearchText = null;
                        LoadServerContacts();
                       // mAdapter.Filter.InvokeFilter(e.NewText);
                    }
                };

                _searchView.QueryTextSubmit += (s, e) =>
                {
                    LoadServerContacts(e.Query);
                };

                _searchView.Close += (s, e) =>
                {
                    paginationModel.SearchText = null;
                    LoadServerContacts();
                };


               
            }
            return base.OnCreateOptionsMenu(menu);
        }

      

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_menuOKOK)
            {
                this.Finish();
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.SingleTop);

                StartActivity(intent);
            }
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {

        }
    }
}