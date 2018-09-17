using Foundation;
using System;
using UIKit;
using PCL.Model;

namespace InPowerIOS.Registration
{
    public partial class SuggestInterestTableViewCell : UITableViewCell
    {
        public SuggestInterestTableViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateCell(InterestResponseViewModel interestResponseViewModel, int position)
        {
            if (interestResponseViewModel != null)
            {
                lblInterest.Text = interestResponseViewModel.Name;
                //chkOk.Checked = interestResponseViewModel[position].isSelected();
              
            }
        }
    }
}