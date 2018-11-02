using Foundation;
using System;
using UIKit;
using PCL.Model;
using InPowerIOS.Repositories;
using System.Collections.Generic;
using InPowerIOS.Model;
using System.Linq;
using InPowerApp.Model;
using Newtonsoft.Json;
using PCL.Service;
using InPowerIOS.Common;

namespace InPowerIOS.Book
{
    public partial class BookShelfReadListViewController : UIViewController
    {
        public BookShelfReadListViewController (IntPtr handle) : base (handle)
        {
        }

        bool loadList = true;
        public UISearchBar searchBar;
        PaginationModel paginationModel = new PaginationModel();
        public List<Books> BookList;
        private BookShelfReadListViewControllerSource BookShelfReadListsource;

        public override void ViewDidAppear(bool animated)
        {
            loadBookAdapter();
            paginationModel.Status = 1;
            base.ViewDidAppear(animated);
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            Title = "READ";

            searchBar = CommonSearchView.Create();
            searchBar.TextChanged += (sender, e) =>
            {
                searchBooks();
            };
            searchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            tblBookReadList.TableHeaderView = searchBar;

            paginationModel.Status = 1;
            if (InternetConnectivityModel.CheckConnection())             {                 LoadServerBooks();             }             else             {                 paginationModel.SkipRecords = 0;                 paginationModel.TakeRecords = 30;                 loadBookAdapter();             }
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
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(), false);
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
                            BookShelfReadListsource.AddMoreBookList(BookList);
                            tblBookReadList.ReloadData();
                            loadList = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomToast.Show(ex.ToString(), false);
                    }
                }
                else
                {
                    CustomToast.Show(Message :"No More Books", Default: true);
                }
            }
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(), false);
            }
        }


        private void LoadMoreBooksOffline()
        {
            try
            {
                loadList = false;
                paginationModel.SkipRecords += 30;

                BookList = BookRepository.GetBooks(BookStatus.Read, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    BookShelfReadListsource.AddMoreBookList(BookList);
                    tblBookReadList.ReloadData();
                    loadList = true;
                }
            }
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(), false);
            }


        }


        public void loadBookAdapter()
        {
            try
            {
                BookList = BookRepository.GetBooks(BookStatus.Read, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    tblBookReadList.TableFooterView = new UIView();
                    BookShelfReadListsource = new BookShelfReadListViewControllerSource(BookList);
                    tblBookReadList.Source = BookShelfReadListsource;
                    BookShelfReadListsource.ReloadList += BookShelfWishListsource_ReloadList;
                    BookShelfReadListsource.ItemRemoved += BookShelfWishListsource_ItemRemoved;
                    tblBookReadList.RowHeight = 115;
                    tblBookReadList.ReloadData();
                }
            }
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(),false);
            }
        }

        void BookShelfWishListsource_ReloadList(object sender, long e)
        {
            if (loadList && !(BookList.Count < 30))
            {
                CustomToast.Show(Message: "Loading More Books", Default: true);
                if (InternetConnectivityModel.CheckConnection())
                {
                    LoadMoreBooks();
                }
                else
                {
                    LoadMoreBooksOffline();
                }
            }

            //if (loadList == true)
            //{
            //    CustomToast.Show(Message: "Loading More Books", Default: true);
            //    LoadMoreBooks();
            //}
        }

        void BookShelfWishListsource_ItemRemoved(object sender, long e)
        {
            BookShelfReadListsource.RemoveBook(e);
            tblBookReadList.ReloadData();
            searchBooks();
            //View.EndEditing(false);
        }

        void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            searchBar.Text = "";
            searchBooks();
            View.EndEditing(false);
        }

        private void searchBooks()  
        {  
            //perform the search, and refresh the table with the results  
            BookShelfReadListsource.PerformSearch(searchBar.Text);  
            tblBookReadList.ReloadData(); 
        
        }  
    }
}