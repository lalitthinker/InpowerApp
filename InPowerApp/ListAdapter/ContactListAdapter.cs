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
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using PCL.Common;
using Square.Picasso;

namespace InPowerApp.ListAdapter
{
    public class ContactListAdapter : RecyclerView.Adapter, IFilterable
    {
        public List<Contact> searchContact;
        public List<Contact> originalContact;
        public Filter Filter { get; private set; }
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> AddNewGroupItemClick;
        Context context;

        // Load the adapter with the data set (photo album) at construction time:
        public ContactListAdapter(List<Contact> Contact, Context context)
        {
            originalContact = Contact.ToList();
            searchContact = Contact.ToList();
            this.context = context;
            Filter = new ContactFilter(this);
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //// Inflate the CardView for the photo:
            //View itemView = LayoutInflater.From(parent.Context).
            //            Inflate(Resource.Layout.ContactListItem, parent, false);



            //ContactListHolder vh = new ContactListHolder(itemView, OnClick);




            //return vh;

            RecyclerView.ViewHolder viewHolder = null;
            LayoutInflater inflater = LayoutInflater.From(parent.Context);


            switch (viewType)
            {

                case 0:
                    View v1 = inflater.Inflate(Resource.Layout.AddNewGroupListItem, parent, false);
                    viewHolder = new AddNewGroupListItemViewHolder(v1, OnClickFirst);
                    break;

                case 1:

                    View v2 = inflater.Inflate(Resource.Layout.ContactListItem, parent, false);
                    viewHolder = new ContactListHolder(v2, OnClick);
                    break;
            }
            return viewHolder;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (position)
            {
                case 0:
                    AddNewGroupListItemViewHolder vhAddNewGroup = holder as AddNewGroupListItemViewHolder;
                    break;
                default:
                
                    ContactListHolder vhContact = holder as ContactListHolder;
                    var item = searchContact[position];
                    vhContact.txtSenderName.Text = item.name;

                    if (item.contactPicUrl != null)
                    {
                        CommonHelper.SetImageOnUIImageView(vhContact.imgMessagelogo, item.contactPicUrl, context, 400, 400);
                    }
                    break;
            }

          
        }

        public override int GetItemViewType(int position)
        {
            if (position==0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public int getItemCount()
        {
            return searchContact != null ? searchContact.Count : 0;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return searchContact.Count; }
        }


        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        void OnClickFirst(int position)
        {
            if (AddNewGroupItemClick != null)
                AddNewGroupItemClick(this, position);
        }

        public void add(Contact Contact)
        {
            originalContact.Add(Contact);
        }

        public void removeBook(Contact Contact)
        {
            originalContact.Remove(Contact);
        }
    }



    public class ContactListHolder : RecyclerView.ViewHolder
    {
        public TextView txtSenderName { get; set; }
        public ImageView imgMessagelogo { get; set; }

        public Action<int> _listener;

        public ContactListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            txtSenderName = itemView.FindViewById<TextView>(Resource.Id.txtSenderName);
            imgMessagelogo = itemView.FindViewById<ImageView>(Resource.Id.imgMessagelogo);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }

        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            ItemView.Click += HandleClick;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            { 
            if (ItemView != null)
            {
                ItemView.Click -= HandleClick;

            }
            _listener = null;
            }
            catch(System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        void HandleClick(object sender, EventArgs e)
        {
            try
            {
                if (_listener != null)
                {
                    _listener(base.AdapterPosition);
                }
            }catch(System.Exception ex)
            {


            }
        }
    }

    public class AddNewGroupListItemViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtSenderName { get; set; }
        public ImageView imgMessagelogo { get; set; }

        public Action<int> _listener;

        public AddNewGroupListItemViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            //txtSenderName = itemView.FindViewById<TextView>(Resource.Id.txtSenderName);
            //imgMessagelogo = itemView.FindViewById<ImageView>(Resource.Id.imgMessagelogo);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }

        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            ItemView.Click += HandleClick;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                if (ItemView != null)
            {
                ItemView.Click -= HandleClick;
            }
            _listener = null;
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        void HandleClick(object sender, EventArgs e)
        {
            try
            {
                if (_listener != null)
                {
                    _listener(base.AdapterPosition);
                }
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }


    public class ContactFilter : Filter
    {
        Context context;
        private readonly ContactListAdapter _adapter;
        public ContactFilter(ContactListAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {

            var returnObj = new FilterResults();
            var results = new List<Contact>();
          

            if (constraint == null)
            {
                return returnObj;
            }

           if (_adapter.originalContact != null && _adapter.originalContact.Any())
            {
                if (_adapter.originalContact.Count != 0)
                {
                    results.Insert(0, null);

                    results.AddRange(
                      _adapter.originalContact.Where(
                          contact => (contact!=null)? (((contact.name != null) ? contact.name.ToLower().Contains(constraint.ToString()) : false) || ((contact.screenName != null) ? contact.screenName.ToLower().Contains(constraint.ToString()) : false)):false));
                    
                }

            }
            else
            {

               Toast.MakeText(context, "No contact", ToastLength.Long).Show();

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
               _adapter.searchContact = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<Contact>()).ToList();

            _adapter.NotifyDataSetChanged();

            // Don't do this and see GREF counts rising
            constraint.Dispose();
            results.Dispose();
        }
    }
}