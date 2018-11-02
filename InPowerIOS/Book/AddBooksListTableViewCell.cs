using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using SDWebImage;
using InPowerIOS.Common;
using PCL.Model;
using PCL.Service;
using InPowerIOS.Repositories;
using System.Collections.Generic;
using InPowerApp.Model;
using GlobalToast;

namespace InPowerIOS.Book
{
    public partial class AddBooksListTableViewCell : UITableViewCell
    {
        BookViewModel book = new BookViewModel();
        public event EventHandler<long> ReloadList;

        public AddBooksListTableViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(BookViewModel books)
        {
            if (books != null)
            {
                book = books;
              //  CommonHelper.SetCircularImage(ReadBookImage);
                lblBookTitle.Text = books.Title;
                lblAutherName.Text = books.Author;
                if (!string.IsNullOrEmpty(books.BookPictureUrl))
                {
                    ReadBookImage.SetImage(new NSUrl(books.BookPictureUrl), UIImage.FromBundle("default_book.png"));
                }
                else
                {
                    ReadBookImage.Image = new UIImage("default_book.png");
                }
            }
        }

        public async void SaveReadBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().PostBook(_model);
            if (result.Status == 1)
            {
                List<BookViewModel> _listBook = new List<BookViewModel>();
                _listBook.Add(book);
                // BookViewModel _viewModel = new BookViewModel();
                //_viewModel.BookId = _model.BookId;
                //_viewModel.BookStatus = BookStatus.WishList;
                var savedBooks = BookRepository.SaveBookList(_listBook, BookStatus.Read);
                if (savedBooks != null)
                {
                    CustomToast.Show("Book successfully added to read list", true);
                    this.ReloadList(this, (int)_model.BookId);
                }
                else
                {
                    CustomToast.Show("Failed to add book to read list", false);
                  
                    this.ReloadList(this, 0);
                }
            }
            //this.ListReload(this, 0); 
        }


        partial void BtnRead_TouchUpInside(UIButton sender)
        {
            BooksMapViewModel _model = new BooksMapViewModel();
            _model.BookId = book.BookId;
            _model.IsRead = 1;
            _model.UserId = CommonHelper.GetUserId();
            SaveReadBook(_model);
        }


        partial void BtnWishList_TouchUpInside(UIButton sender)
        {
            if (InternetConnectivityModel.CheckConnection(true))
            {
                BooksMapViewModel _model = new BooksMapViewModel();
                _model.BookId = book.BookId;
                _model.IsRead = 0;
                _model.UserId = CommonHelper.GetUserId();
                SaveWishListBook(_model);
            }
        }

     


        public async void SaveWishListBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().PostBook(_model);
            if (result.Status == 1)
            {
                List<BookViewModel> _listBook = new List<BookViewModel>();
                _listBook.Add(book);
                // BookViewModel _viewModel = new BookViewModel();
                //_viewModel.BookId = _model.BookId;
                //_viewModel.BookStatus = BookStatus.WishList;
                var savedBooks = BookRepository.SaveBookList(_listBook, BookStatus.WishList);
                if (savedBooks != null)
                {
                    CustomToast.Show("Book successfully added to wish list", true);
                    this.ReloadList(this, (int)_model.BookId);
                }
                else
                {
                    CustomToast.Show("Failed to add book to wish list", false);
                    this.ReloadList(this, 0);
                }
            }
        }
    }
}