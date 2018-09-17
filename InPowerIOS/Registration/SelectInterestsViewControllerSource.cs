using System;
using System.Collections.Generic;
using Foundation;
using PCL.Model;
using UIKit;

namespace InPowerIOS.Registration
{
    public class SelectInterestsViewControllerSource: UITableViewSource
    {
        
        public List<InterestResponseViewModel> originalInterest;

        public SelectInterestsViewControllerSource(List<InterestResponseViewModel> items)
        {
            this.originalInterest = items;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SuggestInterestTableViewCell") as SuggestInterestTableViewCell;
            cell.UpdateCell(originalInterest[indexPath.Row], indexPath.Row);

            if (this.originalInterest[indexPath.Row].isSelected())
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
            return originalInterest != null ? originalInterest.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SuggestInterestTableViewCell") as SuggestInterestTableViewCell;

            cell.Selected = !this.originalInterest[indexPath.Row].isSelected();
            this.originalInterest[indexPath.Row].InterestCheck = !this.originalInterest[indexPath.Row].isSelected();
            tableView.ReloadData();
        }
    }
}
