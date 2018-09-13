using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PCL.Model;

namespace InPowerApp.ListAdapter
{
    class InterestListAdapter : BaseAdapter
    {

        Context context;
        List<InterestResponseViewModel> ListInterest;

        public InterestListAdapter(Context context, List<InterestResponseViewModel> _List)
        {
            this.context = context;
            ListInterest = _List;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            InterestListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as InterestListAdapterViewHolder;

            if (view == null)
            {
                holder = new InterestListAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in



                view = inflater.Inflate(Resource.Layout.InterestListItem, parent, false);
                holder.InterestedName = view.FindViewById<TextView>(Resource.Id.lblInterestlistItemName);
                holder.chkOk = view.FindViewById<CheckBox>(Resource.Id.chkOk);
                view.Tag = holder;
                view.SetTag(Resource.Id.chkOk, holder.chkOk);
            }
            else
            {
                holder = (InterestListAdapterViewHolder)convertView.Tag;
            }

            holder.chkOk.Tag = position;
            holder.chkOk.Visibility = ViewStates.Visible;
            holder.chkOk.SetOnCheckedChangeListener(new CheckChangeListner(ListInterest, convertView, this));


            //fill in your items
            holder.InterestedName.Text = ListInterest[position].Name;
            holder.chkOk.Checked = ListInterest[position].isSelected();

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return ListInterest.Count;
            }
        }

    }

    class InterestListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use

        public TextView InterestedName { get; set; }
        public CheckBox chkOk { get; set; }
    }
}