using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Model;
using PCL.Common;
using Square.Picasso;
using PCL.Service;
using PCL.Model;
using Java.Lang;

namespace InPowerApp.ListAdapter
{
    //public interface OnBottomReachedListener
    //{

    //    void onBottomReached(int position);


    //}
    public class GroupContactAdapter : RecyclerView.Adapter
    {
        // OnBottomReachedListener onBottomReachedListener;
        // Event handler for item clicks:
        public event EventHandler<int> ListReload;
        public Filter Filter { get; private set; }
        // Underlying data set (a photo album):
        // Underlying data set (a photo album):
        public List<ContacSelectListViewModel> searchContacts;
        public List<ContacSelectListViewModel> originalContacts;
        Activity Contextt;

        //public void setOnBottomReachedListener(OnBottomReachedListener onBottomReachedListener)
        //{

        //    this.onBottomReachedListener = onBottomReachedListener;
        //}

        // Load the adapter with the data set (photo album) at construction time:
        public GroupContactAdapter(List<ContacSelectListViewModel> contact, Activity Context)
        {
            originalContacts = contact.ToList();
            this.Contextt = Context;
            Filter = new SelectGroupContactFilter(this);
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.SelectGroupContact, parent, false);

            GroupContactListHolder vh = new GroupContactListHolder(itemView, OnClick);



            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GroupContactListHolder vh = holder as GroupContactListHolder;
            var item = originalContacts[position];
            //vh.txtSenderName.Text = item.FirstName+" "+item.LastName;

            //if (item.ProfileImageUrl != null)
            //{
            //    CommonHelper.SetImageOnUIImageView(vh.contactPic, item.ProfileImageUrl, Contextt,400,400);
            //}

            vh.ChkSelected.Tag = position;
            vh.ChkSelected.Visibility = ViewStates.Visible;
            vh.ChkSelected.SetOnCheckedChangeListener(new CheckChangeGroupListner(originalContacts, this));


            //fill in your items
            vh.ContactId = originalContacts[position].ContactId;
            vh.ConatctName.Text = originalContacts[position].ConatactName;
            if (originalContacts[position].ProfileImageUrl != null)
            {
                CommonHelper.SetImageOnUIImageView(vh.ContactImage, originalContacts[position].ProfileImageUrl, this.Contextt, 400, 400);
            }
            vh.ConatctName.Text = originalContacts[position].ConatactName;
            vh.ChkSelected.Checked = originalContacts[position].isSelected();

        }

        public void RefreshList(object o, int contactId)
        {
            if (contactId != 0)
            {
                this.ListReload(this, contactId);
            }
        }



        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return originalContacts.Count; }
        }


        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            //if (ItemClick != null)
            //    ItemClick(this, position);
        }

        public void add(ContacSelectListViewModel Contact)
        {
            originalContacts.Add(Contact);
        }

        public void remove(ContacSelectListViewModel Contact)
        {
            originalContacts.Remove(Contact);
        }
    }



    public class GroupContactListHolder : RecyclerView.ViewHolder
    {
        public long ContactId { get; set; }
        public ImageView ContactImage { get; set; }
        public TextView ConatctName { get; set; }
        public CheckBox ChkSelected { get; set; }
        public Action<int> _listener;

        public GroupContactListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            //holder.ContactImage = view.FindViewById<ImageView>(Resource.Id.contactPic);
            //holder.ConatctName = view.FindViewById<TextView>(Resource.Id.ContactName);
            //holder.ChkSelected = view.FindViewById<CheckBox>(Resource.Id.ChkSelected);
            //view.Tag = holder;
            //view.SetTag(Resource.Id.chkOk, holder.ChkSelected);
            ContactImage = itemView.FindViewById<ImageView>(Resource.Id.contactPic);
            ConatctName = itemView.FindViewById<TextView>(Resource.Id.ContactName);
            ChkSelected = itemView.FindViewById<CheckBox>(Resource.Id.ChkSelected);
        }



        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            // ItemView.Click += HandleClick;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (ItemView != null)
            {
                //  ItemView.Click -= HandleClick;
            }
            _listener = null;
        }

        void HandleClick(object sender, int e)
        {
            if (_listener != null)
            {
                _listener(base.AdapterPosition);
            }
        }
    }

    internal class CheckChangeGroupListner : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
    {
        private List<ContacSelectListViewModel> contactList;
     
        private GroupContactAdapter groupContactAdapter;

        public CheckChangeGroupListner(List<ContacSelectListViewModel> contactList, GroupContactAdapter groupContactAdapter)
        {
            this.contactList = contactList;
          
            this.groupContactAdapter = groupContactAdapter;
        }
        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            int getPosition = (int)buttonView.Tag;
            contactList[getPosition].setSelected(buttonView.Checked);
        }
    }

    public class SelectGroupContactFilter : Filter
    {
        private readonly GroupContactAdapter _adapter;
        public SelectGroupContactFilter(GroupContactAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnObj = new FilterResults();
            var results = new List<ContacSelectListViewModel>();
            if (_adapter.searchContacts == null)
                _adapter.searchContacts = _adapter.originalContacts;

            if (constraint == null) return returnObj;

            if (_adapter.searchContacts != null && _adapter.originalContacts.Any())
            {
                // Compare constraint to all names lowercased. 
                // It they are contained they are added to results.
                results.AddRange(
                    _adapter.searchContacts.Where(
                        contact => ((contact.ConatactName != null) ? contact.ConatactName.ToLower().Contains(constraint.ToString()) : false)));
            }

            // Nasty piece of .NET to Java wrapping, be careful with this!
            returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            returnObj.Count = results.Count;

            constraint.Dispose();

            return returnObj;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
                _adapter.originalContacts = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<ContacSelectListViewModel>()).ToList();

            _adapter.NotifyDataSetChanged();

            // Don't do this and see GREF counts rising
            constraint.Dispose();
            results.Dispose();
        }
    }

}