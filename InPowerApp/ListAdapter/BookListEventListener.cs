using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Model;
using InPowerApp.Repositories;
using PCL.Model;
using PCL.Service;

namespace InPowerApp.ListAdapter
{

    internal class ReadListnerWithActivity : Java.Lang.Object, View.IOnClickListener
    {
        private BookViewModel obitmobEvent;
        private Activity context_read;
        public EventHandler<int> ListReload;


        public ReadListnerWithActivity(BookViewModel obitmobEvent, Activity context)
        {
            this.obitmobEvent = obitmobEvent;
            this.context_read = context;
        }


        public void Dispose()
        {

        }

        public void OnClick(View v)
        {
            if (InternetConnectivityModel.CheckConnection(context_read,true))
            {
                BooksMapViewModel _model = new BooksMapViewModel();
                _model.BookId = obitmobEvent.BookId;
                _model.IsRead = 1;
                _model.UserId = CommonHelper.GetUserId();
                SaveBook(_model);
            }
        }

        public async void SaveBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().PostBook(_model);
            if (result.Status == 1)
            {
                List<BookViewModel> _listBook = new List<BookViewModel>();
                _listBook.Add(obitmobEvent);
                // BookViewModel _viewModel = new BookViewModel();
                //_viewModel.BookId = _model.BookId;
                //_viewModel.BookStatus = BookStatus.WishList;
                var savedBooks = BookRepository.SaveBookList(_listBook, BookStatus.Read);
                if (savedBooks != null)
                {
                    Toast.MakeText(this.context_read, "Book successfully added to read list", ToastLength.Long).Show();
                    this.ListReload(this, (int)_model.BookId);
                }
                else
                {
                    Toast.MakeText(this.context_read, "Failed to add book to read list", ToastLength.Long).Show();
                   this.ListReload(this, 0);
                }
            }
            this.ListReload(this, 0);
        }
    }
   
    
    internal class btnWishListAdapterButtonClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private BookViewModel obitmobEvent;
        private Activity context;
        public EventHandler<int> ListReloadWishListListner;


        public btnWishListAdapterButtonClickListener(BookViewModel obitmobEvent, Activity context)
        {
            this.obitmobEvent = obitmobEvent;
            this.context = context;
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.Handle == IntPtr.Zero)
                return;

        }


        public void OnClick(View v)
        {
            if (InternetConnectivityModel.CheckConnection(context, true))
            {
                BooksMapViewModel _model = new BooksMapViewModel();
                _model.BookId = obitmobEvent.BookId;
                _model.IsRead = 0;
                _model.UserId = CommonHelper.GetUserId();
                SaveBook(_model);
            }
        }

        public async void SaveBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().PostBook(_model);
            if (result.Status == 1)
            {
                List<BookViewModel> _listBook = new List<BookViewModel>();
                _listBook.Add(obitmobEvent);
               // BookViewModel _viewModel = new BookViewModel();
                //_viewModel.BookId = _model.BookId;
                //_viewModel.BookStatus = BookStatus.WishList;
                var savedBooks = BookRepository.SaveBookList(_listBook,BookStatus.WishList);
                if (savedBooks != null)
                {
                    Toast.MakeText(context, "Book added to wish list", ToastLength.Long).Show();
                  this.ListReloadWishListListner(this, (int) _model.BookId);
                }
                else
                {
                    Toast.MakeText(context, "Failed to add to wish list", ToastLength.Long).Show();
                   this.ListReloadWishListListner(this, 0);
                }
            }
           this.ListReloadWishListListner(this, 0);
        }

     
    }
}