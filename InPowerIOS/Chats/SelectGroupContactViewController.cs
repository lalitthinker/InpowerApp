using Foundation;
using System;
using UIKit;
using InPowerIOS.SignalR;
using InPowerIOS.Common;
using System.Threading.Tasks;
using InPowerIOS.Repositories;
using Newtonsoft.Json;
using PCL.Model;
using System.Collections.Generic;
using PCL;
using InPowerIOS.SideBarMenu;
using BigTed;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public partial class SelectGroupContactViewController : UIViewController
    {
        public GroupRequestViewModel groupmodel { get; set; }
        public List<ContacSelectListViewModel> ContactList;
        public int UserId;

        PaginationModel paginationModel = new PaginationModel();
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        SelectGroupContactViewControllerSource selectGroupContactViewControllerSource;
        public SelectGroupContactViewController (IntPtr handle) : base (handle)
        {
            this.groupmodel = groupmodel;
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            NavigationItem.SetRightBarButtonItems(
               new[]
               {
                new UIBarButtonItem(CommonHelper.GetCorrectIcon()
                        , UIBarButtonItemStyle.Plain
                        ,
                        (esender, args) =>
                        {
                    PostGroupData();

                })
            }, true);
                

            Title = "My Contacts";
            UserId = CommonHelper.GetUserId();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            paginationModel.Status = 2;
            paginationModel.SkipRecords = 0;

            loadadapter();
        }


        public override void ViewDidAppear(bool animated)
        {
            loadSignalR();
            base.ViewDidAppear(animated);
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

                if (ContactList.Count > 0)
                {
                    tblSelectGroupContacts.TableFooterView = new UIView();

                    selectGroupContactViewControllerSource = new SelectGroupContactViewControllerSource(ContactList);

                    tblSelectGroupContacts.Source = selectGroupContactViewControllerSource;
                    tblSelectGroupContacts.RowHeight = 50;
                    tblSelectGroupContacts.ReloadData();
                }

            }
            catch (System.Exception ex)
            {
                
                Crashes.TrackError(ex);
            }

        }

        private async Task PostGroupData()
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
                                MemberId = ContactList[i].ContactId,
                            MemberName=ContactList[i].ConatactName

                            };
                            ListGroupMemberViewModel.Add(_objGroupMemberViewModel);

                        }
                    }
                    var _objGroupRequestModel = new GroupRequestViewModel
                    {
                        Name = groupmodel.Name,
                        Description = groupmodel.Description,
                        InterestId = groupmodel.InterestId,
                        MemberCount = ListGroupMemberViewModel.Count,
                        Visibility = 1,
                        IsPrivate = groupmodel.IsPrivate,
                        GroupType = groupmodel.GroupType,
                        Members = ListGroupMemberViewModel,
                        UserId = CommonHelper.GetUserId(),
                        PictureUrl = groupmodel.PictureUrl
                    };

                    BTProgressHUD.Show("Create Group", maskType: ProgressHUD.MaskType.Black);
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

                        BTProgressHUD.Dismiss();
                        //this.DismissViewController(true, null);
                        UIStoryboard storyboard = this.Storyboard;
                        RootViewController viewController = (RootViewController)
                            storyboard.InstantiateViewController("RootViewController");
                        this.PresentViewController(viewController, true, null);
                    }

                    BTProgressHUD.Dismiss();

                }
            }
            catch (System.Exception ex)
            {
                BTProgressHUD.Dismiss();
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
                //// Log to Android Device Logging.
                //Android.Util.Log.Error("Crash Report", exception.Message);
                //Crashes.TrackError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
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