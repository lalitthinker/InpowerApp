using Foundation;
using System;
using UIKit;
using PCL.Model;
using InPowerIOS.Repositories;
using InPowerApp.Model;
using System.Threading.Tasks;
using InPowerIOS.SignalR;
using PCL.Service;
using InPowerIOS.Common;
using Newtonsoft.Json;
using InPowerIOS.Model;
using System.Collections.Generic;
using CoreGraphics;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Chats
{
    public partial class ChatViewContaroller : UIViewController
    {
        public ChatConversation chatConversation { get; set; }
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        PaginationModel paginationModel = new PaginationModel();
        public ContactViewModel contactViewModel { get; set; }
        long contactId;
        string ContactName = "";
        bool loadList = true;
        Dictionary<DateTime, List<ChatMessage>> ListChatsCon;

        private ChatViewContarollerSource chatViewContarollerSource;
        public ChatViewContaroller (IntPtr handle) : base (handle)
        {
            this.contactViewModel = contactViewModel;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            getUserDetails();

            var g = new UITapGestureRecognizer(() => View.EndEditing(false));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);

        }


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            Task.Run(() =>
            {
               Loadonresume();
            });

        }


        private void Loadonresume()
        {
            try
            {
                ContactName = ContactRepository.GetContactbyUserId(Convert.ToInt64(contactViewModel.ContactId)).name;
                contactId = contactViewModel.ContactId;
                if (InternetConnectivityModel.CheckConnection())
                {
                    loadSignalR();
                }

                InvokeOnMainThread(() =>
                {
                    paginationModel.SkipRecords = 0;
                    paginationModel.TakeRecords = 30;
                    LoadLocalLatestMessages();
                });

                if (InternetConnectivityModel.CheckConnection())
                {
                    LoadServerMessagesUpto(DateTime.UtcNow);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void LoadLocalLatestMessages()
        {
            try
            {
                loadList = false;

                if (chatConversation != null)
                {
                    ListChatsCon = ChatMessageRepository.GetChatMessagesForPageIndex(paginationModel, chatConversation.ChatId);

                    if (ListChatsCon != null && ListChatsCon.Count > 0)
                    {

                        tblChat.TableFooterView = new UIView();

                        chatViewContarollerSource = new ChatViewContarollerSource(ListChatsCon,this);

                        tblChat.Source = chatViewContarollerSource;
                        tblChat.RowHeight = 40;
                        tblChat.ReloadData();
                        ScrollToBottom(true);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }



        private async void LoadServerMessagesUpto(DateTime date)
        {
            var unixTimeStamp = CommonHelper.DateTimeToUnixTimestamp(date.AddSeconds(-1));
            try
            {
                //BTProgressHUD.Show("Loading messages..", maskType: ProgressHUD.MaskType.Black);
                await Task.Run(async () =>
                {
                    var result = await new ChatService().GetChatMessagesUptoId(contactViewModel.ContactId, unixTimeStamp);
                    if (result.Status == 1)
                    {
                        var chatMessagelist = JsonConvert.DeserializeObject<List<ChatMessageViewModel>>(result.Response.ToString());

                        var savedMessages = ChatMessageRepository.SaveChatMessages(chatMessagelist, chatConversation.ChatId);

                        InvokeOnMainThread(() =>
                        {
                            LoadLocalLatestMessages();
                        });

                    }

                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        public class PostChatResponseViewModel
        {
            public ChatModel Chat { get; set; }
            public ChatMessageViewModel ChatMessage { get; set; }
        }


        public void getUserDetails()
        {
            if (contactViewModel != null)
            {
                ContactName = ContactRepository.GetContactbyUserId(contactViewModel.ContactId).name;
                contactId = contactViewModel.ContactId;
                Title = ContactName;
               

                var titleView = new UILabel(new CGRect(0, 0, 100, 60));
                titleView.Text = ContactName;
                titleView.TextColor = UIColor.White;
                var ShowUserProfileViewController = new UITapGestureRecognizer(() =>
                {
                    var viewController = (UserProfileViewController)Storyboard.InstantiateViewController("UserProfileViewController");
                    viewController.contactViewModel = contactViewModel;
                    NavigationController.PushViewController(viewController, true);
                });
                titleView.UserInteractionEnabled = true;
                titleView.AddGestureRecognizer(ShowUserProfileViewController);
                NavigationItem.TitleView = titleView;

            }

            //if (contactViewModel.ProfileImageUrl != null && contactViewModel.ProfileImageUrl != "")
            //{

            //    Picasso.With(this)
            //   .Load(ContactObject.ProfileImageUrl)
            //  .Resize(100, 100)
            //  .CenterCrop().Placeholder(Resource.Drawable.default_profile)
            //  .Into(IV_Userlogo);
            //}
            //else
            //{
            //    IV_Userlogo.SetBackgroundResource(Resource.Drawable.default_profile);
            //}

        }

        private async void loadSignalR()
        {
            try
            {
                await _objChatSignalRService.Connect();
                _objChatSignalRService.OnMessageReceived += _objChatSignalRService_OnMessageReceived;
                _objChatSignalRService.OnGetUpdateStatusRecieved += _objChatSignalRService_OnGetUpdateStatus;
                _objChatSignalRService.OnGetReload += _objChatSignalRService_OnGetReload;
                await _objChatSignalRService.ConnectUser("PrivateChat");
                if (contactViewModel != null)
                {
                    await _objChatSignalRService.Reload(contactViewModel.ContactId.ToString());
                }



            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }


        }


        private void _objChatSignalRService_OnGetReload(object sender, string e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    LoadServerMessagesUpto(DateTime.Now);

                });
            }
            catch (Exception eX)
            {
                //Crashes.TrackError(eX);
            }

        }


        private Object thisLock = new Object();
        private async void _objChatSignalRService_OnMessageReceived(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    lock (thisLock)
                    {
                        var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                        if (chatmessage == null)
                        {
                            if (contactViewModel.ChatId == e.ChatId)
                            {
                                e.IsRead = true;
                            }
                            var savedMessages = ChatMessageRepository.SaveChatMessage(e, e.ChatId);
                            InvokeOnMainThread(() =>
                            {
                                LoadLocalLatestMessages();
                            });

                            UpdateChatMessage(chatConversation.ContactId.ToString(), e, "Private");
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        public async void UpdateChatMessage(string contactid, ChatMessageViewModel model, string privatemsg)
        {
            try
            {
                await _objChatSignalRService.SendUpdate(chatConversation.ContactId.ToString(), model, privatemsg);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }



        private void _objChatSignalRService_OnGetUpdateStatus(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    lock (thisLock)
                    {
                        var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                        if (chatmessage != null)
                        {
                            var savedMessages = ChatMessageRepository.updateChatMessage(e);

                            InvokeOnMainThread(() =>
                            {
                                LoadLocalLatestMessages();

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
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
                //Android.Util.Log.Error("Crash Report", exception.Message);
                //Crashes.TrackError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        void ScrollToBottom(bool animated)
        {
            if (tblChat.NumberOfSections() == 0)
                return;

            var items = (int)tblChat.NumberOfRowsInSection(0);
            if (items == 0)
                return;

            var finalRow = (int)NMath.Max(0, tblChat.NumberOfRowsInSection(0) - 1);
            NSIndexPath finalIndexPath = NSIndexPath.FromRowSection(finalRow, 0);
            tblChat.ScrollToRow(finalIndexPath, UITableViewScrollPosition.Top, animated);
        }

    }
}