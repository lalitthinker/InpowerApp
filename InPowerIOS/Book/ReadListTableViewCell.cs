using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
using InPowerIOS.Common;
using SDWebImage;
using PCL.Model;
using PCL.Service;
using InPowerIOS.Repositories;

namespace InPowerIOS.Book
{
    public partial class ReadListTableViewCell : UITableViewCell
    {
        Books book = new Books();
        public event EventHandler<long> ReloadList;

        public ReadListTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public void UpdateCell(Books books)
        {

            if (books != null)
            {
                book = books;
               // CommonHelper.SetCircularImage(ReadBookImage);
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

        partial void BtnRemove_TouchUpInside(UIButton sender)
        {
            BooksMapViewModel _model = new BooksMapViewModel();
            _model.BookId = book.BookId;
            _model.IsRead = 1;
            _model.UserId = CommonHelper.GetUserId();
            RemoveBook(_model);
        }


        public async void RemoveBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().RemoveBook(_model);
            if (result.Status == 1)
            {
                BookViewModel _viewModel = new BookViewModel();
                _viewModel.BookId = _model.BookId;
                _viewModel.BookStatus = BookStatus.Removed;
                var savedBooks = BookRepository.UpdateBook(_viewModel);
                if (savedBooks != null)
                {
                    CustomToast.Show("Book successfully removed", true);
                    this.ReloadList(this, (int)_model.BookId);
                }
                else
                {
                        CustomToast.Show("Failed to remove book", false);
                        this.ReloadList(this, 0);
                }
            }
            //this.ListReload(this, 0);
        }
    }
}