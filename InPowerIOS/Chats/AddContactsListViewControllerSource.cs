using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using UIKit;
using System.Linq;

namespace InPowerIOS.Chats
{
    public class AddContactsListViewControllerSource : UITableViewSource
    {
        public List<UserProfile> searchContacts;
        public List<UserProfile> originalContacts;
        public event EventHandler<long> ReloadList;
        public event EventHandler<long> ItemRemoved;
        public AddContactsListViewControllerSource(List<UserProfile> contactList)
        {
            this.originalContacts = contactList;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return originalContacts.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("AddContactsTableViewCell") as AddContactsTableViewCell;
            cell.UpdateCell(originalContacts[indexPath.Row]);
            cell.ReloadList += Cell_ReloadList;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            // if showing last row of last section, load more
            if (indexPath.Section == tableView.NumberOfSections() - 1 && indexPath.Row == originalContacts.Count - 1)
            {
                long userid = originalContacts[indexPath.Row].UserId;
                this.ReloadList(this, userid);
            }
        }


        void Cell_ReloadList(object sender, long e)
        {
            this.ItemRemoved(this, e);
        }


        public void AddMoreContactList(List<UserProfile> contactList)
        {
            originalContacts.AddRange(contactList);
        }

        public void RemoveBook(long UserId)
        {
            var bookItem = originalContacts.Where(a => a.UserId == UserId).FirstOrDefault();
            originalContacts.Remove(bookItem);
        }
    }
}
