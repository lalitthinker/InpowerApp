using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using PCL.Model;

namespace InPowerApp.ListAdapter
{
    internal class CheckChangeListner : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
    {
        private List<InterestResponseViewModel> listInterest;
        private View convertView;
        private InterestListAdapter interestListAdapter;

        public CheckChangeListner(List<InterestResponseViewModel> listInterest, View convertView, InterestListAdapter interestListAdapter)
        {
            this.listInterest = listInterest;
            this.convertView = convertView;
            this.interestListAdapter = interestListAdapter;
        }

     
        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            int getPosition = (int)buttonView.Tag;
            listInterest[getPosition].setSelected(buttonView.Checked);
        }
    }

}