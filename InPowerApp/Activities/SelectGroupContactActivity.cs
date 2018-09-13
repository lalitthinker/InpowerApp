using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.ListAdapter;
using InPowerApp.Model;
using InPowerApp.Repositories;
using InPowerApp.SignalR;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PCL;
using PCL.Common;
using PCL.Model;
using PCL.Service;
using Plugin.Connectivity;
using static Android.Views.View;

namespace InPowerApp.Activities
{
    [Activity(Label = "Welcome to InPower")]
    public class SelectGroupContactActivity : AppCompatActivity, IOnLayoutChangeListener
    {
        public GroupRequestViewModel groupmodel { get; set; }
        bool loadList = true;
        bool searchList = false;
        RecyclerView mRecyclerView;
        GroupContactAdapter mAdapter;
        public List<ContacSelectListViewModel> ContactList;
        PaginationModel paginationModel = new PaginationModel();
        private Android.Support.V7.Widget.SearchView _searchView;
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
                SetContentView(Resource.Layout.MyContactslayout);
                groupmodel = JsonConvert.DeserializeObject<GroupRequestViewModel>(Intent.GetStringExtra("GroupObject"));
                var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetTitle(Resource.String.AddContacts);
                paginationModel.Status = 2;
                mRecyclerView = FindViewById<RecyclerView>(Resource.Id.lvGroupContactList);
                if (mRecyclerView != null)
                {
                    mRecyclerView.HasFixedSize = true;
                    var layoutManager = new LinearLayoutManager(this);
                    mRecyclerView.SetLayoutManager(layoutManager);
                }
                paginationModel.SkipRecords = 0;

                loadadapter();
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }
        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new System.Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new System.Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as System.Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(System.Exception exception)
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

        public void loadadapter()
        {
            try
            {
                ContactList = new List<ContacSelectListViewModel>();
                var contactdata = ContactRepository.GetContactsbyType("mycontacts");
                foreach (var item in contactdata)
                {
                    ContacSelectListViewModel _objContactViewModel = new ContacSelectListViewModel();
                    _objContactViewModel.ConatactName = item.name;
                    _objContactViewModel.ContactId = item.contactId;
                    _objContactViewModel.ProfileImageUrl = item.contactPicUrl;
                    _objContactViewModel.ConatactCheck = false;
                    ContactList.Add(_objContactViewModel);
                }

                if (ContactList != null)
                {
                    mAdapter = new GroupContactAdapter(ContactList, this);
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.NotifyDataSetChanged();
                }
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        private void ListReload(object sender, int bookId)
        {
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            try
            {
                MenuInflater.Inflate(Resource.Menu.action_contactsListMenu, menu);
                if (menu != null)
                {
                    menu.FindItem(Resource.Id.action_menuOKOK).SetVisible(true);

                    var searchItems = menu.FindItem(Resource.Id.action_ContactSearch);

                    searchItems.SetVisible(true);

                    //MenuItemCompat.SetOnActionExpandListener(searchItems, new SearchViewExpandListener((IFilter)mAdapter));

                    var searchItem = MenuItemCompat.GetActionView(searchItems);
                    _searchView = searchItem.JavaCast<Android.Support.V7.Widget.SearchView>();
                    _searchView.SubmitButtonEnabled = true;

                    _searchView.QueryTextChange += (s, e) => mAdapter.Filter.InvokeFilter(e.NewText);



                }
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_ContactSearch)
            {
                //this.Finish();
            }

            else if (item.ItemId == Resource.Id.action_menuOKOK)
            {
                PostGroupData();
            }
            return base.OnOptionsItemSelected(item);
        }

        private async void PostGroupData()
        {
            try
            {
                if (groupmodel != null && ContactList.Count > 0)
                {
                    List<GroupMemberViewModel> ListGroupMemberViewModel = new List<GroupMemberViewModel>();
                    for (int i = 0; i < ContactList.Count; i++)
                    {
                        if (ContactList[i].isSelected())
                        {
                            var _objGroupMemberViewModel = new GroupMemberViewModel
                            {
                                MemberId = ContactList[i].ContactId

                            };
                            ListGroupMemberViewModel.Add(_objGroupMemberViewModel);

                        }
                    }
                    var _objGroupRequestModel = new GroupRequestViewModel
                    {
                        Name = groupmodel.Name,
                        Description = groupmodel.Description,
                        InterestId = groupmodel.InterestId,
                        MemberCount = ListGroupMemberViewModel.Count(),
                        Visibility = 1,
                        IsPrivate = groupmodel.IsPrivate,
                        GroupType = groupmodel.GroupType,
                        Members = ListGroupMemberViewModel,
                        UserId = CommonHelper.GetUserId(),
                        PictureUrl = groupmodel.PictureUrl
                    };

                    var result = await new GroupChatService().CreateGroup(_objGroupRequestModel);
                    if (result.Status == 1)
                    {
                        var modelGroup = JsonConvert.DeserializeObject<GroupResponseViewModel>(result.Response.ToString());
                        await _objChatSignalRService.GroupCreate(modelGroup);

                        GroupRepository.SaveGroupCreated(modelGroup);
                        foreach (var member in modelGroup.Members)
                        {
                            GroupRepository.SaveorUpdateGroupMember(member, modelGroup.GroupId);
                        }

                        StartActivity(typeof(MainActivity));
                    }

                }
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnResume()
        {
            try
            {
                SupportActionBar.SetTitle(Resource.String.MyContacts);
                loadSignalR();
                base.OnResume();
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {

        }
        private async void loadSignalR()
        {
            try
            {
                await _objChatSignalRService.Connect();
                await _objChatSignalRService.ConnectUser("GroupCreate");

            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


    }
}