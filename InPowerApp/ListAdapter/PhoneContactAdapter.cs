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
using InPowerApp.Model;
using Java.Lang;
using Refractored.Controls;

namespace InPowerApp.ListAdapter
{
    class PhoneContactAdapter : RecyclerView.Adapter, IFilterable
    {

        Activity context;
        public Filter Filter { get; private set; }
        private List<PhoneContactModel> _Originalitems;
        private List<PhoneContactModel> _items;
        PhoneContactModel _PhoneContactModel = new PhoneContactModel();
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        int SrNo;
        public PhoneContactAdapter(Activity activity, List<PhoneContactModel> _chat)
        {
            this.context = activity;
            _items = _chat.ToList();
            Filter = new PhoneContactFilter(this);


            alphaIndex = new Dictionary<string, int>();
            for (int i = 0; i < _items.Count; i++)
            {
                var key = _items[i].name.ToString().Substring(0, 1);
                if (!alphaIndex.ContainsKey(key))
                    alphaIndex.Add(key, i);
            }

            sections = new string[alphaIndex.Keys.Count];
            alphaIndex.Keys.CopyTo(sections, 0);
            sectionsObjects = new Java.Lang.Object[sections.Length];
            for (int i = 0; i < sections.Length; i++)
            {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }

        }
        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            var itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.PhoneContactsListItem, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            ChatAdaptereViewHolder vh = new ChatAdaptereViewHolder(itemView, OnClick);
            return vh;
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ChatAdaptereViewHolder vh = holder as ChatAdaptereViewHolder;
            _PhoneContactModel = new PhoneContactModel();
            _PhoneContactModel = _items[position];

            vh.ContactPersonName.Text = _items[position].name.ToString();
            vh.ContactNumber.Text = _items[position].number;

            if (_items[position].photoId == null)
            {
                vh.imgContactPersonlogo.SetImageResource(Resource.Drawable.default_profile);
            }
            //else
            //{
            //    var contactUri = ContentUris.WithAppendedId(
            //        ContactsContract.Contacts.ContentUri, _items[position].contactId);
            //    //var contactPhotoUri = Android.Net.Uri.WithAppendedPath(contactUri, Contacts.Photos.ContentDirectory);
            //    vh.imgContactPersonlogo.SetImageURI(Android.Net.Uri.WithAppendedPath(contactUri, Contacts.Photos.ContentDirectory));
            //    ////////viewHolder.imgContactPersonlogo.SetImageDrawable(ImageManager.Get(parent.Context, _items[position].photo));
            //}



            vh.InvitePhoneContact.Tag = _items[position].number;
            vh.InvitePhoneContact.SetOnClickListener(new InvitePhoneContactButtonClickListener(_items, context));

            //vh.imgMessagelogo.Tag = _items[position].number;
            //vh.imgMessagelogo.SetOnClickListener(new imgMessagelogoButtonClickListener(_items, context));


        }


        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return _items.Count; }
        }

        public class ChatAdaptereViewHolder : RecyclerView.ViewHolder
        {
            public CircleImageView imgContactPersonlogo { get; set; }

            public TextView ContactPersonName { get; set; }
            public TextView ContactNumber { get; set; }
            public ImageView imgMessagelogo { get; set; }
            public Button InvitePhoneContact { get; set; }


            public Action<int> _listener;

            public ChatAdaptereViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                imgContactPersonlogo = itemView.FindViewById<CircleImageView>(Resource.Id.imgContactPersonlogo);
                ContactPersonName = itemView.FindViewById<TextView>(Resource.Id.lblContactPersonName);
                ContactNumber = itemView.FindViewById<TextView>(Resource.Id.lblContactNumber);
                imgMessagelogo = itemView.FindViewById<ImageView>(Resource.Id.imgMessagelogo);
                InvitePhoneContact = itemView.FindViewById<Button>(Resource.Id.btnInvitePhoneContact);

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
                if (ItemView != null)
                {
                    ItemView.Click -= HandleClick;
                }
                _listener = null;
            }

            void HandleClick(object sender, EventArgs e)
            {
                if (_listener != null)
                {
                    _listener(base.AdapterPosition);
                }
            }
        }

        public void Initialize(View view)
        {
        }

        string[] sections;
        Java.Lang.Object[] sectionsObjects;
        private Dictionary<string, int> alphaIndex;

        public int GetPositionForSection(int sectionIndex)
        {
            return alphaIndex[sections[sectionIndex]];
        }

        public int GetSectionForPosition(int position)
        {
            int prevSection = 0;

            for (int i = 0; i < sections.Length; i++)
            {
                if (GetPositionForSection(i) > position)
                {
                    break;
                }

                prevSection = i;
            }

            return prevSection;
        }

        public System.Object[] GetSections()
        {
            return sectionsObjects;
        }

        private class PhoneContactFilter : Filter
        {
            private readonly PhoneContactAdapter _adapter;
            public PhoneContactFilter(PhoneContactAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<PhoneContactModel>();
                if (_adapter._Originalitems == null)
                    _adapter._Originalitems = _adapter._items;

                if (constraint == null) return returnObj;

                if (_adapter._Originalitems != null && _adapter._Originalitems.Any())
                {

                    results.AddRange(
                        _adapter._Originalitems.Where(
                            PhoneContact => PhoneContact.name.ToLower().Contains(constraint.ToString()) || PhoneContact.number.ToLower().Contains(constraint.ToString())));
                }
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter._items = values.ToArray < Java.Lang.Object>()
                        .Select(r => r.ToNetObject<PhoneContactModel>()).ToList();

                _adapter.NotifyDataSetChanged();

                // Don't do this and see GREF counts rising
                constraint.Dispose();
                results.Dispose();
            }
        }

        public class InvitePhoneContactButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private List<PhoneContactModel> _items;
            private Activity activity;

            public InvitePhoneContactButtonClickListener(List<PhoneContactModel> items, Activity activity)
            {
                _items = items;
                this.activity = activity;
            }

            public async void OnClick(View v)
            {
                string OperationMethodName = null, SendInvitationMessage = null, SendInvitationMessageContactNumber = null;
                SendInvitationMessageContactNumber = (string)v.Tag;
                OperationMethodName = "SendInvitationViaSMS";
                SendInvitationMessage = "Hey! I just installed InPower, with  messaging & all of my favorite Book Intrest on one app. Download it now at https://play.google.com/store/apps/details?id=thethiinker.inPower.app";

                if (SendInvitationMessageContactNumber != null && SendInvitationMessageContactNumber != "")
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(activity);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Sending SMS");
                    alert.SetMessage("Sending SMS may charge you, are you sure want to send SMS");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        var smsUri = Android.Net.Uri.Parse("smsto:" + SendInvitationMessageContactNumber);
                        var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                        smsIntent.PutExtra("sms_body", SendInvitationMessage);
                        activity.StartActivity(smsIntent);
                    });
                    alert.SetButton2("CANCEL", (c, ev) => {
                        alert.Dismiss();

                    });
                    alert.Show();

                  
                   
                    return;
                }
            }
        }
    }
}
