using Foundation;
using System;
using UIKit;
using InPowerIOS.Model;
//using InPowerIOS.Chats;
using System.Collections.Generic;
using InPowerIOS.Repositories;
using InPowerIOS.Chats;
using System.Threading.Tasks;

using PCL.Service;
using PCL.Model;
using InPowerIOS.Common;
using Newtonsoft.Json;
using InPowerIOS.SignalR;
using InPowerApp.Model;
using System.Linq;
namespace InPowerIOS.Chats
{
    public partial class ChatListViewController : UIViewController
    {
        public ChatListViewController (IntPtr handle) : base (handle)
        {
        }

        private readonly MainScreenTabBarController ParentController;
        private List<ChatConversation> Chats;
        private ChatListViewControllerSource chatsource;
        private UISearchController searchController;
        public int UserId;
        public UISearchBar searchBar;
        ChatSignalRServices _objChatSignalRService = new ChatSignalRServices();
        public ChatListViewController(MainScreenTabBarController ParentController) : base("ChatListViewController", null)
        {
            this.ParentController = ParentController;
        }

        public Contact selectedContact { get; set; }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            searchBar = CommonSearchView.Create();
            searchBar.TextChanged += (sender, e) =>  
            {
                searchChatUsers();
            };  
            searchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            tblChatList.TableHeaderView = searchBar;
           
            Title = "CHATS";

            UserId = CommonHelper.GetUserId();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
         
            Loadonresume();
           
           // tblChatList.Scrolled += TblChatList_Scrolled;

        }


        public override void ViewDidAppear(bool animated)
        {
            Loadonresume();
            base.ViewDidAppear(animated);
        }

        private async void Loadonresume()
        {
              if (InternetConnectivityModel.CheckConnection())
                {
                    loadSignalR();
                }

                InvokeOnMainThread(() =>
                {
                    LoadLocalChats();
                });

                if (InternetConnectivityModel.CheckConnection())
                {
                    LoaderAsyncfromserver();
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

        private static void LogUnhandledException(Exception exception)
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

        private async void loadSignalR()
        {
            try
            {
                await _objChatSignalRService.Connect();
                _objChatSignalRService.OnGetUpdateStatusRecieved += _objChatSignalRService_OnGetUpdateStatus;
                _objChatSignalRService.OnGroupMessageReceived += _objChatSignalRService_OnGroupMessageReceived;
                _objChatSignalRService.OnGetGroupMessageUpdateStatusRecieved += _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved;
                _objChatSignalRService.OnGetGroupCreateStatusRecieved += _objChatSignalRService_OnGetGroupCreateStatusRecieved;
                _objChatSignalRService.OnMessageReceived += _objChatSignalRService_OnMessageReceived;

                await _objChatSignalRService.ConnectUser("ChatList");

            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }

        private void _objChatSignalRService_OnGetGroupCreateStatusRecieved(object sender, GroupResponseViewModel e)
        {
            try
            {
                if (e.GroupId != 0)
                {
                    GroupRepository.SaveGroupCreated(e);
                    ChatModel cm = new ChatModel();
                    cm.ChatId = e.ChatModel.ChatId;
                    cm.GroupId = e.ChatModel.GroupId;
                    cm.Message = e.ChatModel.Message;
                    cm.LastActiveTime = e.ChatModel.LastActiveTime;
                    ChatConversation chatConversation = ChatConversationRepository.SaveGroupConverstionNewFromServer(cm);
                    if (chatConversation != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            MessageReceived(chatConversation, "NewChat");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }



        private void _objChatSignalRService_OnGetGroupMessageUpdateStatusRecieved(object sender, GroupMessageStatusViewModel e)
        {
            try
            {
                if (e.GroupMessageId != 0)
                {
                    var savedMessages = GroupRepository.UpdateGroupMessageStatus(e);
                    InvokeOnMainThread(() =>
                    {
                        LoadLocalChats();
                    });
                }
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }


        private async void _objChatSignalRService_OnGroupMessageReceived(object sender, GroupMessageResponseViewModel e)
        {
            try
            {
                if (e.MessageId != 0)
                {
                    var chatmessage = GroupRepository.CheckGroupChatMessage(e.MessageId, e.GroupId);
                    if (chatmessage == null)
                    {
                        GroupRepository.SaveGroupMessage(e, "ChatList");
                        var chatConversationResponse = ChatConversationRepository.GetConversationbyGroupId(e.GroupId);
                        if (chatConversationResponse.success)
                        {
                            if (e.Attachments.Count > 0)
                            {
                                e.Message = (string.IsNullOrEmpty(e.Message)) ? e.Attachments[e.Attachments.Count - 1].Type : e.Message;
                            }
                            chatConversationResponse.chatConversation = ChatConversationRepository.UpdateChatLastMessage(chatConversationResponse.chatConversation.id, e.Message, e.SenderProfileName);
                            if (chatConversationResponse.chatConversation != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                    MessageReceived(chatConversationResponse.chatConversation, "OldChat");
                            });
                        }
                    }
                       
                        GroupMessageStatusViewModel _model = new GroupMessageStatusViewModel();
                        _model.GroupMessageId = e.MessageId;
                        await _objChatSignalRService.SendGroupMessageUpdate(e.SenderId.ToString(), _model, "ChatList");
                    }
                }
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }
        private void _objChatSignalRService_OnGetUpdateStatus(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                    if (chatmessage != null)
                    {
                        var savedMessages = ChatMessageRepository.updateChatMessage(e);
                        var chatConversation = ChatConversationRepository.GetConversationIdbyChatId(savedMessages.ChatId);
                        if (chatConversation != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                MessageReceived(chatConversation, "OldChat");
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }

        private void MessageReceived(ChatConversation chatConversation, string ProcessType)
        {
            if (ProcessType == "OldChat")
            {
                chatsource.UpdateChat(chatConversation);
                tblChatList.ReloadData();
            }
            else
            {

                chatsource.AddChat(chatConversation);
                tblChatList.ReloadData();
            }
        }


        private async void _objChatSignalRService_OnMessageReceived(object sender, ChatMessageViewModel e)
        {
            try
            {
                if (e.ChatMessageId != 0)
                {
                    var chatmessage = ChatMessageRepository.CheckMessage(e.ChatMessageId);
                    if (chatmessage == null)
                    {
                        var savedMessages = ChatMessageRepository.SaveChatMessage(e, e.ChatId);
                        var chatConversation = ChatConversationRepository.GetConversationIdbyChatId(savedMessages.ChatId);
                        if (e.Attachments.Count > 0)
                        {
                            savedMessages.MessageText = (string.IsNullOrEmpty(savedMessages.MessageText)) ? e.Attachments[e.Attachments.Count - 1].Type : savedMessages.MessageText;
                        }
                        chatConversation = ChatConversationRepository.UpdateChatLastMessage(chatConversation.id, savedMessages.MessageText, "");
                        if (chatConversation != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                MessageReceived(chatConversation, "OldChat");
                            });
                        }

                        await _objChatSignalRService.SendUpdate(chatConversation.ContactId.ToString(), e, "ChatList");
                    }
                }
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }


        private async void LoaderAsyncfromserver()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var result = await new ChatService().getChatAll(new UserRequestViewModel { UserId = (int)CommonHelper.GetUserId() });

                    if (result.Status == 1)
                    {
                        var chatlist = JsonConvert.DeserializeObject<List<ChatModel>>(result.Response.ToString());
                        Console.Write(result.Status);
                        ChatConversationRepository.SaveChatConversationFromServer(chatlist);
                       
                        InvokeOnMainThread(() =>
                        {
                            LoadLocalChats();
                        });
                      
                    }
                });
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(e);
            }
        }


        private void LoadLocalChats()
        {
            Chats = ChatConversationRepository.GetAllChat().ToList();

            if (Chats.Count > 0)
            {
                tblChatList.TableFooterView = new UIView();
               
                chatsource = new ChatListViewControllerSource(Chats,this);
               
                tblChatList.Source = chatsource;
                tblChatList.RowHeight = 60;
                tblChatList.ReloadData();
            }
        }

        void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            searchBar.Text = "";
            searchChatUsers();
            View.EndEditing(false);
        }


        private void searchChatUsers()  
        {  
            //perform the search, and refresh the table with the results  
            chatsource.PerformSearch(searchBar.Text);  
            tblChatList.ReloadData(); 
        
        }  

        protected void DismissKeyboardOnBackgroundTap()
        { 

            // Add gesture recognizer to hide keyboard 
           // var tap = new UITapGestureRecognizer { CancelsTouchesInView = false }; tap.AddTarget (() => View.EndEditing (true)); View.AddGestureRecognizer (tap);
        }


    }
}