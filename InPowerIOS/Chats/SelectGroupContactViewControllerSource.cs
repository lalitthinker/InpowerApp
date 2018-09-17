using System;
using System.Collections.Generic;
using Foundation;
using PCL.Model;
using UIKit;

namespace InPowerIOS.Chats
{
    public class SelectGroupContactViewControllerSource: UITableViewSource
    {
        public List<GroupMemberViewModel> searchContacts;
        public List<ContacSelectListViewModel> originalContact;

        public SelectGroupContactViewControllerSource(List<ContacSelectListViewModel> items)
        {
            this.originalContact = items;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SelectGroupContactListTableViewCell") as SelectGroupContactListTableViewCell;
            cell.UpdateCell(originalContact[indexPath.Row], indexPath.Row);
            if (this.originalContact[indexPath.Row].isSelected())
            {
                cell.Accessory = UITableViewCellAccessory.Checkmark;
            }
            else
            {
                cell.Accessory = UITableViewCellAccessory.None;
            }
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return originalContact != null ? originalContact.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SelectGroupContactListTableViewCell") as SelectGroupContactListTableViewCell;

            cell.Selected = !this.originalContact[indexPath.Row].isSelected();
            this.originalContact[indexPath.Row].ConatactCheck =  !this.originalContact[indexPath.Row].isSelected();
            tableView.ReloadData();
        }
    }
}
