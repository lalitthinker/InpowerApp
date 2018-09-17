using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using UIKit;
using System.Linq;

namespace InPowerIOS.Book
{
    public class BookShelfWishListViewControllerSource: UITableViewSource
    {
        public BookShelfWishListViewControllerSource()
        {
        }
        public List<Books> searchBooks;
        public List<Books> originalBooks;
        public event EventHandler<long> ReloadList;
        public event EventHandler<long> ItemRemoved;
     
        public BookShelfWishListViewControllerSource(List<Books> bookList)
        {
            this.originalBooks = bookList;
            this.searchBooks = bookList;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return searchBooks.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //if (RowSelectedEvent != null)
            //{
            //    this.selectedItem = Items[indexPath.Row];
            //    RowSelectedEvent(this, EventArgs.Empty);
            //}
            //tableView.DeselectRow(indexPath, true);
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("BookWishListTableViewCell") as BookWishListTableViewCell;
            cell.UpdateCell(searchBooks[indexPath.Row]);
            cell.ReloadList += Cell_ReloadList;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            // if showing last row of last section, load more
            if (indexPath.Section == tableView.NumberOfSections() - 1 && indexPath.Row == originalBooks.Count - 1)
            {
                long bookid = originalBooks[indexPath.Row].BookId;
                this.ReloadList(this, bookid);
            }
        }

        void Cell_ReloadList(object sender, long e)
        {
            this.ItemRemoved(this, e);
        }

        public void AddMoreBookList(List<Books> bookList)
        {
            originalBooks.AddRange(bookList);
        }

        public void RemoveBook(long BookId)
        {
            var bookItem = originalBooks.Where(a => a.BookId == BookId).FirstOrDefault();
            originalBooks.Remove(bookItem);
        }

        public void PerformSearch(string searchText)  
        {  
            searchText = searchText.ToLower();  
            this.searchBooks = originalBooks.Where(
                book => ((book.Title != null) ? book.Title.ToLower().Contains(searchText) : false) || ((book.Author != null) ? book.Author.ToLower().Contains(searchText) : false)).ToList();
        } 
    }
}
