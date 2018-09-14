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
    public class AddBooksToWishListActivity : AppCompatActivity, IOnLayoutChangeListener
    {
        bool loadList = true;
        bool searchList = false;
        RecyclerView mRecyclerView;
        AddBooksToWishListAdapter mAdapter;
        public List<BookViewModel> BookList;
        PaginationModel paginationModel = new PaginationModel();
        private Android.Support.V7.Widget.SearchView _searchView;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddBooksToWishListlayout);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetTitle(Resource.String.AddBooksToWishList);
            paginationModel.Status = 2;
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.BookList);
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
                            Toast.MakeText(this, "Loading More Books...", ToastLength.Long).Show();
                            if (InternetConnectivityModel.CheckConnection(this))
                            {
                                if (String.IsNullOrEmpty(paginationModel.SearchText))
                                    LoadMoreBooks();
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
                LoadServerBooks();
            }
        }
        private async void LoadServerBooksFromSearch()
        {
            try
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                var result = await new BookShelfService().GetAllBooks(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    BookList = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    loadBookAdapter(BookList);
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }
        private async void LoadServerBooks(string SearchText = null)
        {
            try
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                paginationModel.SearchText = SearchText;
                var result = await new BookShelfService().GetAllBooks(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    BookList = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    loadBookAdapter(BookList);
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }
        private async void LoadMoreBooks()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;
                var result = await new BookShelfService().GetAllBooks(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    BookList = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    try
                    {
                        if (BookList != null && BookList.Count > 0)
                        {
                            foreach (var item in BookList)
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
                    Toast.MakeText(this, "No More Books", ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }

        public void loadBookAdapter(List<BookViewModel> BookList)
        {
            try
            {
                //  BookList = BookRepository.GetBooks(BookStatus.Removed, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    mAdapter = new AddBooksToWishListAdapter(BookList, this);
                    mAdapter.ListReloadWishList += ListReload;
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }


        public void RealodloadBookAdapter(int bookId)
        {
            try
            {
                BookViewModel bookitem = BookList.Where(a => a.BookId == bookId).FirstOrDefault();
                int indexOfRemoved = BookList.IndexOf(bookitem);
                if (BookList != null && BookList.Count > 0)
                {
                    mAdapter.removeBook(bookitem);
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
                RealodloadBookAdapter(bookId);
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_bookListMenu, menu);
            if (menu != null)
            {
               

                var searchItems = menu.FindItem(Resource.Id.action_BookSearch);

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
                        LoadServerBooks();
                        // mAdapter.Filter.InvokeFilter(e.NewText);
                    }
                };

                _searchView.QueryTextSubmit += (s, e) =>
                {
                    LoadServerBooks(e.Query);
                };

                _searchView.Close += (s, e) =>
                {
                    paginationModel.SearchText = null;
                    LoadServerBooks();
                };


                MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener(mAdapter));
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
                return true;
            }

            public bool OnMenuItemActionExpand(IMenuItem item)
            {
                return true;
            }
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
              //  this.Finish();
                var intent = new Intent(this, typeof(MainActivity));
              //  intent.AddFlags(ActivityFlags.SingleTop);

                StartActivity(intent);


            }
            return base.OnOptionsItemSelected(item);


        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {

        }

       

    }
}