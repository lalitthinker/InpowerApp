using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;


namespace InPowerIOS.SideBarMenu
{
    public class MenuListItem
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
    }
    public class MenuListSource : UITableViewSource
    {
        private List<MenuListItem> Items = new List<MenuListItem>();

        public event EventHandler RowSelectedEvent;
        public long eventId { get; set; }
        public MenuListItem selectedItem = new MenuListItem();
        public MenuListSource(List<MenuListItem> items)
        {
            this.Items = items;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Items.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (RowSelectedEvent != null)
            {
                this.selectedItem = Items[indexPath.Row];
                RowSelectedEvent(this, EventArgs.Empty);
            }
            tableView.DeselectRow(indexPath, true);
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("MenuListItemCell") as MenuListItemCell;
            cell.UpdateCell(Items[indexPath.Row].Name
                            ,
                            Items[indexPath.Row].ImageUrl
                            , Items[indexPath.Row].ImageUrl, false
                           );
            return cell;
        }


    }
}
