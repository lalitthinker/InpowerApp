using System;
using System.Collections.Generic;
using Foundation;
using InPowerIOS.Model;
using UIKit;

namespace InPowerIOS.Setting
{
    public class BlockedContactsViewControllerSource: UITableViewSource
    {
        public BlockedContactsViewControllerSource()
        {
        }

        public List<ChatConversation> originalContacts;
        public BlockedContactsViewControllerSource(List<ChatConversation> originalContacts)
        {
            this.originalContacts = originalContacts;
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
            var cell = tableView.DequeueReusableCell("BlockedContactsTableViewCell") as BlockedContactsTableViewCell;
            cell.UpdateCell(originalContacts[indexPath.Row]);
            return cell;
        }
    }
}
