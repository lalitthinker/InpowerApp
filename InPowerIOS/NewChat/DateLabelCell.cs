using Foundation;
using System;
using UIKit;
using static InPowerIOS.Chats.ChatViewContarollerSource;

namespace InPowerIOS
{
    public partial class DateLabelCell : UITableViewCell
    {
        public DateLabelCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(DateItem item)
        {
           
            var datetimedata = Convert.ToDateTime(item.getDate()).Date;

            if (datetimedata.Date == DateTime.UtcNow.Date)
                        {
                lblDate.SetTitle("Today",UIControlState.Normal);
                        }
            else if (datetimedata.Date == DateTime.UtcNow.Date.AddDays(-1))
                        {
                lblDate.SetTitle("Yesterday", UIControlState.Normal);
                        }
                        else
                        {
                lblDate.SetTitle(datetimedata.ToString("MMM dd, yyyy"), UIControlState.Normal);
                        }
           // lblDate.TouchUpInside += LblDate_TouchUpInside;

        }

      
partial void LblDate_TouchUpInside(UIButton sender)
        {
            
        }
    }
}