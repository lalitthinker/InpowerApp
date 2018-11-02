using Foundation;
using System;
using UIKit;
using PCL.Model;
using InPowerIOS.Model;
using System.Collections.Generic;
using InPowerIOS.Repositories;
using System.Linq;
using InPowerApp.Model;
using Newtonsoft.Json;
using PCL.Service;
using GlobalToast;
using InPowerIOS.Common;

namespace InPowerIOS.Book
{
    public partial class BookShelfWishListViewController : UIViewController
    {
        public BookShelfWishListViewController(IntPtr handle) : base(handle)
        {
        }

        bool loadList = true;
        public UISearchBar searchBar;
        PaginationModel paginationModel = new PaginationModel();
        public List<Books> BookList;
        private BookShelfWishListViewControllerSource BookShelfWishListsource;


        public override void ViewDidAppear(bool animated)
        {
            loadBookAdapter();
            paginationModel.Status = 0;
            base.ViewDidAppear(animated);
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            searchBar = CommonSearchView.Create();
            searchBar.TextChanged += (sender, e) =>
            {
                searchBooks();
            };
            searchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            tblBookWishList.TableHeaderView = searchBar;
            Title = "WISH LIST";
            paginationModel.Status = 0;
            if (InternetConnectivityModel.CheckConnection())
            {
                LoadServerBooks();
            }
            else
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
                loadBookAdapter();
            }
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
                    var savedBooks = BookRepository.SaveBookList(booklist, BookStatus.WishList);
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
                    var savedBooks = BookRepository.SaveBookList(booklist, BookStatus.WishList);
                    try
                    {
                        BookList = BookRepository.GetBooks(BookStatus.WishList, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                        if (BookList != null && BookList.Count > 0)
                        {
                            BookShelfWishListsource.AddMoreBookList(BookList);
                            tblBookWishList.ReloadData();
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
                    CustomToast.Show(Message: "No More Books", Default: true);
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

                BookList = BookRepository.GetBooks(BookStatus.WishList, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    BookShelfWishListsource.AddMoreBookList(BookList);
                    tblBookWishList.ReloadData();
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
                BookList = BookRepository.GetBooks(BookStatus.WishList, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    tblBookWishList.TableFooterView = new UIView();
                    BookShelfWishListsource = new BookShelfWishListViewControllerSource(BookList);
                    tblBookWishList.Source = BookShelfWishListsource;
                    BookShelfWishListsource.ReloadList += BookShelfWishListsource_ReloadList;
                    BookShelfWishListsource.ItemRemoved += BookShelfWishListsource_ItemRemoved;
                    tblBookWishList.RowHeight = 115;
                    tblBookWishList.ReloadData();
                }
            }
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(), false);
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
        }

        void BookShelfWishListsource_ItemRemoved(object sender, long e)
        {
            BookShelfWishListsource.RemoveBook(e);
            tblBookWishList.ReloadData();
            searchBooks();
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
            BookShelfWishListsource.PerformSearch(searchBar.Text);  
            tblBookWishList.ReloadData(); 
        
        }  
    }
}