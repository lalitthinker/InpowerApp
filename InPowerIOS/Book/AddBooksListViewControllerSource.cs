using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using PCL.Model;
using UIKit;
using System.Linq;

namespace InPowerIOS.Book
{
    public class AddBooksListViewControllerSource: UITableViewSource
    {
        public AddBooksListViewControllerSource()
        {
        }
        public bool FullyLoaded;
        public List<BookViewModel> searchBooks;
        public List<BookViewModel> originalBooks;
        public event EventHandler<long> ReloadList;
        public event EventHandler<long> ItemRemoved;


        public AddBooksListViewControllerSource(List<BookViewModel> bookList)
        {
            this.originalBooks = bookList;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return originalBooks.Count;
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
            //if (indexPath.Row == originalBooks.Count)
            //{
            //    var loadingCell = tableView.DequeueReusableCell("LoadFooterCell", indexPath) as LoadFooterCell;
            //    loadingCell.Loading = true;
            //    return loadingCell;
            //}
            var cell = tableView.DequeueReusableCell("AddBooksListTableViewCell") as AddBooksListTableViewCell;
            cell.UpdateCell(originalBooks[indexPath.Row]);
            cell.ReloadList += Cell_ReloadList;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            // if showing last row of last section, load more
            if (indexPath.Section == tableView.NumberOfSections() - 1 && indexPath.Row == originalBooks.Count - 1 && !FullyLoaded)
            {
                long bookid = originalBooks[indexPath.Row].BookId;
                this.ReloadList(this, bookid);
            }
        }

        void Cell_ReloadList(object sender, long e)
        {
            this.ItemRemoved(this, e);
        }


        public void AddMoreBookList(List<BookViewModel> bookList)  
        {
            originalBooks.AddRange(bookList);
        } 

        public void RemoveBook(long BookId)
        {
            var bookItem = originalBooks.Where(a => a.BookId == BookId).FirstOrDefault();
            originalBooks.Remove(bookItem);
        }
    }
}
