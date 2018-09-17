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
    public partial class AddBooksListViewController : UIViewController
    {
      
        public AddBooksListViewController(IntPtr handle) : base(handle)
        {
            
        }

        bool loadList = true;
        public UISearchBar searchBar;
        PaginationModel paginationModel = new PaginationModel();
        public List<BookViewModel> BookList;
        private AddBooksListViewControllerSource BookShelfWishListsource;


        public override void ViewDidAppear(bool animated)
        {
            paginationModel.Status = 2;
            base.ViewDidAppear(animated);
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

           
            Title = "ADD BOOKS";

            searchBar = CommonSearchView.Create();
      
            searchBar.SearchButtonClicked+= SearchBar_SearchButtonClicked;
            searchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            tblAddBooksList.TableHeaderView = searchBar;

            paginationModel.Status = 2;
            if (InternetConnectivityModel.CheckConnection())
            {
                LoadServerBooks();
            }
            else
            {
                paginationModel.SkipRecords = 0;
                paginationModel.TakeRecords = 30;
            }
           
        }

        //private void searchBooks()
        //{
            
        //    LoadServerBooks(searchBar.Text);
        //    tblAddBooksList.ReloadData(); 
        
       
        //}

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
                    var booklist = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    loadBookAdapter(booklist);
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
                    BookList = JsonConvert.DeserializeObject<List<BookViewModel>>(result.Response.ToString());
                    try
                    {
                        
                        if (BookList != null && BookList.Count > 0)
                        {
                            BookShelfWishListsource.AddMoreBookList(BookList);
                            tblAddBooksList.ReloadData();
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


        public void loadBookAdapter(List<BookViewModel> BookList)
        {
            try
            {
                //BookList = BookRepository.GetBooks(BookStatus.WishList, BookList).Skip(paginationModel.SkipRecords).Take(paginationModel.TakeRecords).ToList();
                if (BookList != null && BookList.Count > 0)
                {
                    tblAddBooksList.TableFooterView = new UIView();
                    BookShelfWishListsource = new AddBooksListViewControllerSource(BookList);
                    tblAddBooksList.Source = BookShelfWishListsource;
                    BookShelfWishListsource.ReloadList+= BookShelfWishListsource_ReloadList;
                    BookShelfWishListsource.ItemRemoved += BookShelfWishListsource_ItemRemoved;
                    tblAddBooksList.RowHeight = 115;
                    tblAddBooksList.ReloadData();
                }
            }
            catch (Exception ex)
            {
                CustomToast.Show(ex.ToString(), false);
            }
        }

        void BookShelfWishListsource_ReloadList(object sender, long e)
        {
            if (loadList == true)
            {
                if (String.IsNullOrEmpty(paginationModel.SearchText))
                {
                    CustomToast.Show(Message: "Loading More Books", Default: true);

                    LoadMoreBooks();
                }
            }
        }

        void BookShelfWishListsource_ItemRemoved(object sender, long e)
        {
            BookShelfWishListsource.RemoveBook(e);
            tblAddBooksList.ReloadData();
        }

       
        void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            LoadServerBooks();
            View.EndEditing(false);
        }

        void SearchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            LoadServerBooks(searchBar.Text);
            View.EndEditing(false);
        }

    }
}