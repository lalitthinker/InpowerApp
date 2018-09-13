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

namespace InPowerApp.ListAdapter
{
    public class NavDrawerListAdapter : BaseAdapter<NavDrawerItem>
    {
        Activity context;
        List<NavDrawerItem> list;
        int SrNo;

        public NavDrawerListAdapter(Activity _context, List<NavDrawerItem> _list)
                : base()
        {
            this.context = _context;
            this.list = _list;

        }

        public override int Count
        {
            get { return list.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override NavDrawerItem this[int index]
        {
            get { SrNo = 1; return list[index]; }
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            // re-use an existing view, if one is available
            // otherwise create a new one
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.drawer_list_item, parent, false);

            NavDrawerItem item = this[position];
            if (item != null)
            {
                ImageView imgIcon = (ImageView)view.FindViewById(Resource.Id.icon);
                TextView txtTitle = (TextView)view.FindViewById(Resource.Id.title);
                TextView txtCount = (TextView)view.FindViewById(Resource.Id.textinput_counter);


                imgIcon.SetImageResource(item.getIcon());
                txtTitle.Text = item.getTitle();
                //txtCount.Text = (position + 1).ToString();

                if (item.getCounterVisibility())
                {
                    txtCount.Text = item.getCount();
                }
                else
                {
                    txtCount.Visibility = ViewStates.Gone;
                }
            }
            return view;

        }
    }
}