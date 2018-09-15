using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using InPowerApp.Activities;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Newtonsoft.Json;
using PCL.Model;
using PCL.Service;
using Plugin.Connectivity;

namespace InPowerApp.Fragments
{
    public class ReadFragment : Android.Support.V4.App.Fragment
    {
        bool loadList = true;
        PaginationModel paginationModel = new PaginationModel();
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ReadBookListAdapter mAdapter;
        public List<Books> BookList;
        private Android.Support.V7.Widget.SearchView _searchView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Read_Fragment, container, false);
            HasOptionsMenu = true;
            paginationModel.Status = 1; // 0 = WishList ,  1 = Read , 2 = All
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ReadBookList);
            if (mRecyclerView != null)
            {
                mRecyclerView.HasFixedSize = true;
                var layoutManager = new LinearLayoutManager(this.Context);
                var onScrollListener = new XamarinRecyclerViewOnScrollListener(layoutManager);
                onScrollListener.LoadMoreEvent += (object sender, bool loadCalled) =>
                {
                    if (loadCalled)
                        if (loadList && !(BookList.Count < 30))
                        {
                            Toast.MakeText(this.Context, "Loading More Books...", ToastLength.Long).Show();
                            if (InternetConnectivityModel.CheckConnection(this.Context))
                            {
                                LoadMoreBooks();
                            }
                            else
                            {
                                LoadMoreBooksOffline();
                            }
                        }
                };
                mRecyclerView.AddOnScrollListener(onScrollListener);
                mRecyclerView.SetLayoutManager(layoutManager);
            }
            paginationModel.SkipRecords = 0;
            mRecyclerView.Click += MRecyclerView_Click;
            FloatingActionButton fabAddBook = view.FindViewById<FloatingActionButton>(Resource.Id.fabAddBook);
            fabAddBook.Click += FabAddBook_Click;
            if (InternetConnectivityModel.CheckConnection(this.Context))
            {
                LoadServerBooks();
            }
            else
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                loadBookAdapter();
            }
            return view;
        }

        private void MRecyclerView_Click(object sender, EventArgs e)
        {
            CommonHelper.Hidekeyboard(Activity, Activity.Window);
        }

        private void FabAddBook_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(AddBooksToWishListActivity));
            StartActivity(intent);
        }

        private async void LoadServerBooks()
        {
            try
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                var result = await new BookShelfService().GetAllBooks(paginationModel);// 0 = WishList ,  1 = Read , 2 = All
                if (result.Status == 1)
                {
                    var booklist = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    var savedBooks = BookRepository.SaveBookList(booklist, BookStatus.Read);
                    loadBookAdapter();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this.Context, e.ToString(), ToastLength.Long).Show();
            }
        }

        public void loadBookAdapter()
        {
            try
            {
                BookList = BookRepository.GetBooks(BookStatus.Read, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    mAdapter = new ReadBookListAdapter(BookList, this.Context);
                    mAdapter.ListReload += ListReload;
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {

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
                    var booklist = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    var savedBooks = BookRepository.SaveBookList(booklist, BookStatus.Read);
                    try
                    {
                        BookList = BookRepository.GetBooks(BookStatus.Read, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
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
                        Toast.MakeText(this.Context, ex.ToString(), ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this.Context, "No More Books", ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this.Context, e.ToString(), ToastLength.Long).Show();
            }
        }

        private async void LoadMoreBooksOffline()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;

                BookList = BookRepository.GetBooks(BookStatus.Read, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
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
                Toast.MakeText(this.Context, ex.ToString(), ToastLength.Long).Show();
            }


        }

        public void RealodloadBookAdapter(int bookId)
        {
            try
            {
                Books bookitem = BookList.Where(a => a.BookId == bookId).FirstOrDefault();
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
                Toast.MakeText(this.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private void ListReload(object sender, int bookId)
        {
            if (bookId != 0)
                RealodloadBookAdapter(bookId);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.action_addBooks, menu);
            base.OnCreateOptionsMenu(menu, inflater);

            if (BookList.Count > 0)
            {

                var searchItems = menu.FindItem(Resource.Id.action_BookSearch);

                searchItems.SetVisible(true);

                //MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener((IFilter)mAdapter));

                var searchItem = MenuItemCompat.GetActionView(searchItems);
                _searchView = searchItem.JavaCast<Android.Support.V7.Widget.SearchView>();
                _searchView.QueryTextChange += (s, e) => mAdapter.Filter.InvokeFilter(e.NewText);

                _searchView.QueryTextSubmit += (s, e) =>
                {
                    // Handle enter/search button on keyboard here
                    Toast.MakeText(this.Context, "Searched for: " + e.Query, ToastLength.Short).Show();
                    e.Handled = true;
                };


              //  MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener(mAdapter));
            }

        }



//        private class SearchViewExpandListener
//#pragma warning disable CS0618 // Type or member is obsolete
//           : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
//#pragma warning restore CS0618 // Type or member is obsolete
//        {
//            private readonly IFilterable _adapter;

//            public SearchViewExpandListener(IFilterable adapter)
//            {
//                _adapter = adapter;
//            }

//            public bool OnMenuItemActionCollapse(IMenuItem item)
//            {
//                _adapter.Filter.InvokeFilter("");
//                return true;
//            }

//            public bool OnMenuItemActionExpand(IMenuItem item)
//            {
//                return true;
//            }
//        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public override void OnResume()
        {
            loadBookAdapter();
            base.OnResume();
        }
    }
}