using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using InPowerApp.Activities;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PCL.Model;
using PCL.Service;
using Plugin.Connectivity;

namespace InPowerApp.Fragments
{

    public class ContactsFragment : Android.Support.V4.App.Fragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            // Create your fragment here
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }


        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }


        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", exception.Message);
                Crashes.TrackError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ContactListAdapter mAdapter;
        List<Contact> Contacts;
        private Android.Support.V7.Widget.SearchView _searchView;


        //  ListView PhoneContactListView;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Contacts_Fragment, container, false);

            try
            {

                mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.lvContactList);
                if (mRecyclerView != null)
                {
                    var layoutManager = new LinearLayoutManager(this.Context);
                    mRecyclerView.SetLayoutManager(layoutManager);
                }

                FloatingActionButton fabAddContact = view.FindViewById<FloatingActionButton>(Resource.Id.fabAddContact);
                fabAddContact.Click += FabAddContact_Click;
                HasOptionsMenu = true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }



            return view;
        }

        private void FabAddContact_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this.Activity, typeof(AddMoreContactsActivity));
                intent.AddFlags(ActivityFlags.SingleTop);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        //private void PhoneContactListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    var Contact = Contacts.ElementAt(e.Position);

        //    var ContactView = new ContactViewModel { ContactId = Contact.contactId };
        //    var intent = new Intent(Context, typeof(PrivateMessageActivity));
        //    intent.PutExtra("ContactObject", JsonConvert.SerializeObject(ContactView));
        //    StartActivity(intent);

        //}


        //public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        //{
        //    inflater.Inflate(Resource.Menu.action_contacts, menu);
        //    base.OnCreateOptionsMenu(menu, inflater);




        //    var action_ContactsSearch = menu.FindItem(Resource.Id.action_ContactsSearch);
        //    action_ContactsSearch.SetVisible(true);
        //}

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            try
            {
                // menu.Clear();
                inflater.Inflate(Resource.Menu.action_contacts, menu);
                base.OnCreateOptionsMenu(menu, inflater);


                var searchItems = menu.FindItem(Resource.Id.action_ContactsSearch);

                searchItems.SetVisible(true);

                //MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener((IFilter)mAdapter));

                var searchItem = MenuItemCompat.GetActionView(searchItems);
                _searchView = searchItem.JavaCast<Android.Support.V7.Widget.SearchView>();
                _searchView.QueryTextChange += (s, e) => mAdapter.Filter.InvokeFilter(e.NewText);

                _searchView.QueryTextSubmit += (s, e) =>
                {
                    // Handle enter/search button on keyboard here
                    Toast.MakeText(this.Context, "Searched for: " + e.Query, ToastLength.Short).Show();
                    e.Handled = true;
                };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {


                if (item.ItemId == Resource.Id.action_ContactsSearch)
                {

                }

            }

            return base.OnOptionsItemSelected(item);
        }
        public async void GetMyContactsFromServer()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var result = await new ContactsService().GetAllMyContact();

                    if (result.Status == 1)
                    {
                        var chatlist = JsonConvert.DeserializeObject<List<UserProfile>>(result.Response.ToString());
                        ContactRepository.SaveMyContactsFromServer(chatlist, "mycontacts");
                        Activity.RunOnUiThread(() =>
                        {
                            loadadapter();
                        });
                    }

                });

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        //private void loadadapter()
        //{
        //    Contacts = new List<Contact>();
        //    Contacts = ContactRepository.GetContactsbyType("mycontacts");
        //    if (Contacts != null)
        //    {
        //        PhoneContactListView.Adapter = new ContactListAdapter(this.Context, Contacts);
        //    }
        //}

        public void loadadapter()
        {
            try
            {
                Contacts = new List<Contact>();
                Contacts = ContactRepository.GetContactsbyType("mycontacts");
                Contacts.Insert(0, null);
                if (Contacts != null)
                {
                    mAdapter = new ContactListAdapter(Contacts, this.Context);
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.ItemClick += PhoneContactListView_ItemClick;
                    mAdapter.AddNewGroupItemClick += MAdapter_AddNewGroupItemClick;
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void MAdapter_AddNewGroupItemClick(object sender, int e)
        {
            try
            {
                Intent intent = new Intent(this.Activity, typeof(CreateGroupActivity));
                intent.AddFlags(ActivityFlags.SingleTop);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void PhoneContactListView_ItemClick(object sender, int position)
        {
            try
            {
                var ChatCon = Contacts.ElementAt(position);
                if (ChatCon != null)
                {

                    var ContactUser = ContactRepository.GetContactbyUserId(Convert.ToInt64(ChatCon.contactId));
                    var ContactView = new ContactViewModel { ContactId = (long)ChatCon.contactId, ProfileImageUrl = ContactUser.contactPicUrl };
                    if (ContactView != null)
                    {
                        var intent = new Intent(Context, typeof(PrivateMessageActivity));

                        intent.AddFlags(ActivityFlags.SingleTop);
                        intent.PutExtra("ContactObject", JsonConvert.SerializeObject(ContactView));

                        StartActivity(intent);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async override void OnResume()
        {
            base.OnResume();
            await Task.Run(async () =>
            {
                Loadonresume();
            });
        }

       


        private async void Loadonresume()
        {
           
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    loadadapter();
                });

                if (InternetConnectivityModel.CheckConnection(this.Context))
                {
                    GetMyContactsFromServer();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}