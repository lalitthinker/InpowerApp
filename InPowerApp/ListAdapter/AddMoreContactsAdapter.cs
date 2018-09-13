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
    public class AddMoreContactsAdapter : RecyclerView.Adapter
    {
        // OnBottomReachedListener onBottomReachedListener;
        // Event handler for item clicks:
        public event EventHandler<int> ListReload;

        // Underlying data set (a photo album):
        // Underlying data set (a photo album):
        public List<UserProfile> searchContacts;
        public List<UserProfile> originalContacts;
        Activity Contextt;

        //public void setOnBottomReachedListener(OnBottomReachedListener onBottomReachedListener)
        //{

        //    this.onBottomReachedListener = onBottomReachedListener;
        //}

        // Load the adapter with the data set (photo album) at construction time:
        public AddMoreContactsAdapter(List<UserProfile> contact, Activity Context)
        {
            originalContacts = contact.ToList();
            this.Contextt = Context;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.AddContactListItem, parent, false);

            AddMoreContactsListHolder vh = new AddMoreContactsListHolder(itemView, OnClick);



            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AddMoreContactsListHolder vh = holder as AddMoreContactsListHolder;
            var item = originalContacts[position];
            vh.txtSenderName.Text = item.FirstName+" "+item.LastName;

            if (item.ProfileImageUrl != null)
            {
                CommonHelper.SetImageOnUIImageView(vh.contactPic, item.ProfileImageUrl, Contextt,400,400);
            }

            UserProfile addContactModel = originalContacts[position];
            var addContactListEvent = new AddMoreContactListner(addContactModel, Contextt);
            vh.AddContactButton.SetOnClickListener(addContactListEvent);
            addContactListEvent.ListReload += RefreshList;

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

        public void add(UserProfile Contact)
        {
            originalContacts.Add(Contact);
        }

        public void remove(UserProfile Contact)
        {
            originalContacts.Remove(Contact);
        }
    }



    public class AddMoreContactsListHolder : RecyclerView.ViewHolder
    {
        public TextView txtSenderName { get; set; }
        public ImageView contactPic { get; set; }
        public Button AddContactButton { get; set; }
        public EventHandler<int> AddClick { get; set; }
        public Action<int> _listener;

        public AddMoreContactsListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            txtSenderName = itemView.FindViewById<TextView>(Resource.Id.txtSenderName);
            contactPic = itemView.FindViewById<ImageView>(Resource.Id.contactPic);
            AddContactButton = itemView.FindViewById<Button>(Resource.Id.imgMessagelogo);
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

    internal class AddMoreContactListner : Java.Lang.Object, View.IOnClickListener
    {
        private UserProfile Contact;
        private Activity context;
        public EventHandler<int> ListReload;


        public AddMoreContactListner(UserProfile Contact, Activity context)
        {
            this.Contact = Contact;
            this.context = context;
        }


        public void Dispose()
        {

        }

        public void OnClick(View v)
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this.context);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Add Contact");
            alert.SetMessage("Do you want to Add this Contact");
            alert.SetButton("OK", (c, ev) =>
            {
                Contact C = new Contact();
                C.contactId = Contact.UserId;
              //  C.
                SaveContact(C);
            });
            alert.SetButton2("CANCEL", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.Show();

        }

        public async void SaveContact(Contact _model)
        {
            var result = await new ContactsService().AddContactService(_model.contactId);
            if (result.Status == 1)
            {
                Toast.MakeText(this.context, "Contact successfully added", ToastLength.Long).Show();
                this.ListReload(this, (int)_model.contactId);
            }
            else
            {
                Toast.MakeText(this.context, "Contact not added", ToastLength.Long).Show();
                this.ListReload(this, 0);
            }
            this.ListReload(this, 0);
        }
    }


  
}