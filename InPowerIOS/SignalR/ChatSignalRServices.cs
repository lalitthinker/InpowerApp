using InPowerApp.Model;
using Microsoft.AppCenter.Crashes;
using Microsoft.AspNet.SignalR.Client;
using PCL.Common;
using PCL.Model;
using System;

using System.Threading.Tasks;

namespace InPowerIOS.SignalR
{
    public class Singleton
    {
        private static Singleton instance;

        private Singleton() { }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
    public class ChatSignalRServices
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public event EventHandler<ChatMessageViewModel> OnMessageReceived;
        public event EventHandler<GroupMessageResponseViewModel> OnGroupMessageReceived;
        public event EventHandler<ChatMessageViewModel> OnGetUpdateStatusRecieved;
        public event EventHandler<GroupMessageStatusViewModel> OnGetGroupMessageUpdateStatusRecieved;
        public event EventHandler<GroupResponseViewModel> OnGetGroupCreateStatusRecieved;
        public event EventHandler<string> OnGetReload;
        public event EventHandler<string> OnGroupGetReload;

        public ChatSignalRServices()
        {
            try
            {
                _connection = new HubConnection(GlobalConstant.BaseUrlSignalR);
                _proxy = _connection.CreateHubProxy("ChatHubs");
            }
            catch (Exception ex)
            { Crashes.TrackError(ex);}
        }

        #region IChatServices implementation

        public async Task Connect()
        {
            try
            {
                await _connection.Start();
            }
            catch (Exception ex)
            {  Crashes.TrackError(ex);}
            #region Private Chat
            _proxy.On("sendPrivateMessage", (ChatMessageViewModel ChatMessageViewModel, string FU, string TU) => OnMessageReceived(this, new ChatMessageViewModel
            {
                Attachments = ChatMessageViewModel.Attachments,
                ContactId = ChatMessageViewModel.ContactId,
                ChatId = ChatMessageViewModel.ChatId,

                ChatMessageId = ChatMessageViewModel.ChatMessageId,
                Message = ChatMessageViewModel.Message,
                MessageTime = ChatMessageViewModel.MessageTime,
                IsRead = ChatMessageViewModel.IsRead,
                IsSend = ChatMessageViewModel.IsSend,
                IsRecieved = ChatMessageViewModel.IsRecieved,
                MobiledatabaseId = ChatMessageViewModel.MobiledatabaseId,
                FU = FU,
                TU = TU
            }));
            _proxy.On("GetUpdateStatus", (ChatMessageViewModel ChatMessageViewModel, string FU, string TU) => OnGetUpdateStatusRecieved(this, new ChatMessageViewModel
            {
                Attachments = ChatMessageViewModel.Attachments,
                ContactId = ChatMessageViewModel.ContactId,
                ChatId = ChatMessageViewModel.ChatId,

                ChatMessageId = ChatMessageViewModel.ChatMessageId,
                Message = ChatMessageViewModel.Message,
                MessageTime = ChatMessageViewModel.MessageTime,
                IsRead = ChatMessageViewModel.IsRead,
                IsSend = ChatMessageViewModel.IsSend,
                IsRecieved = ChatMessageViewModel.IsRecieved,
                MobiledatabaseId = ChatMessageViewModel.MobiledatabaseId,
                FU = FU,
                TU = TU
            }));
            _proxy.On("GetReload", (string reload) => OnGetReload(this, "Reload"));
            #endregion

            #region Group Chat

            _proxy.On("GetGroupMessage", (GroupMessageResponseViewModel GroupMessageViewModel, string FU, string TU) => OnGroupMessageReceived(this, new GroupMessageResponseViewModel
            {
                Attachments = GroupMessageViewModel.Attachments,
                GroupId = GroupMessageViewModel.GroupId,
                IsSend = GroupMessageViewModel.IsSend,

                MessageId = GroupMessageViewModel.MessageId,
                Message = GroupMessageViewModel.Message,
                MessageTime = GroupMessageViewModel.MessageTime,
                MobileDatabaseId = GroupMessageViewModel.MobileDatabaseId,
                SenderId = GroupMessageViewModel.SenderId,
                SenderProfileName = GroupMessageViewModel.SenderProfileName,
                SenderProfilePicUrl = GroupMessageViewModel.SenderProfilePicUrl,
                FU = FU,
                TU = TU
            }));

            _proxy.On("GetGroupMessageUpdateStatus", (GroupMessageStatusViewModel GroupMessageStatusViewModel, string FU, string TU) => OnGetGroupMessageUpdateStatusRecieved(this, new GroupMessageStatusViewModel
            {
                GroupMessageId = GroupMessageStatusViewModel.GroupMessageId,
                IsRead = GroupMessageStatusViewModel.IsRead,
                IsRecieved = GroupMessageStatusViewModel.IsRecieved,
                IsSend = GroupMessageStatusViewModel.IsSend,
                ReceiverId = GroupMessageStatusViewModel.ReceiverId,
                SenderId = GroupMessageStatusViewModel.SenderId,
                FU = FU,
                TU = TU
            }));

            _proxy.On("GetGroupCreateMessage", (GroupResponseViewModel GroupResponseViewModel, string FU, string TU) => OnGetGroupCreateStatusRecieved(this, new GroupResponseViewModel
            {
                Description = GroupResponseViewModel.Description,
                GroupId = GroupResponseViewModel.GroupId,
                GroupMessageId = GroupResponseViewModel.GroupMessageId,
                InterestId = GroupResponseViewModel.InterestId,
                MemberCount = GroupResponseViewModel.MemberCount,
                Members = GroupResponseViewModel.Members,
                Message = GroupResponseViewModel.Message,
                MessageTime = GroupResponseViewModel.MessageTime,
                Name = GroupResponseViewModel.Name,
                PictureUrl = GroupResponseViewModel.PictureUrl,
                UserId = GroupResponseViewModel.UserId,
                Visibility = GroupResponseViewModel.Visibility,
                ChatModel = GroupResponseViewModel.ChatModel

            }));
            _proxy.On("GetReloadGroup", (string reload) => OnGroupGetReload(this, "ReloadGroup"));
            #endregion
        }
        public class UserDetail
        {
            public string ConnectionId { get; set; }
            public int UserID { get; set; }

        }

        public async Task SendGroup(GroupMessageResponseViewModel message, long groupid)
        {
            try
            {
                _proxy.Invoke("SendGroupMessage", message, groupid);
            }
            catch (Exception ex)
            { Crashes.TrackError(ex);}
        }
        public async Task Send(string toUserId, ChatMessageViewModel message)
        {
            try
            {
                _proxy.Invoke("SendPrivateMessage", toUserId, message);
            }
            catch (Exception ex)
            { Crashes.TrackError(ex);}
        }

        public async Task GroupCreate(GroupResponseViewModel message)
        {
            try
            {
                _proxy.Invoke("GroupCreate", message);
            }
            catch (Exception ex)
            { Crashes.TrackError(ex);}
        }
        public async Task Reload(string toUserId)
        {
            try
            {
                _proxy.Invoke("Reload", toUserId);
            }
            catch (Exception ex)
            { }
        }
        public async Task SendUpdate(string toUserId, ChatMessageViewModel message, string activity)
        {
            try
            {
                _proxy.Invoke("UpdateMessageStatus", toUserId, message, activity);
            }
            catch (Exception ex)
            { Crashes.TrackError(ex);}
        }

        public async Task SendGroupMessageUpdate(string toUserId, GroupMessageStatusViewModel message, string activity)
        {
            try
            {
                _proxy.Invoke("UpdateGroupMessageStatus", toUserId, message, activity);
            }
            catch (Exception ex)
            { Crashes.TrackError(ex);}
        }
        public async Task ConnectUserGroup(long groupId, string ChatType)
        {
            _proxy.Invoke("JoinRoom", Common.CommonHelper.GetUserId(), groupId, ChatType);
        }
        public async Task ConnectUser(string ChatType)
        {
            _proxy.Invoke("ConnectUser", Common.CommonHelper.GetUserId(), ChatType);
        }
        public async Task disconnectUser()
        {
            try
            {
                _connection.Stop();
            }
            catch(Exception ex)
            { Crashes.TrackError(ex); }
        }
        public async Task JoinRoom(string roomName)
        {
            _proxy.Invoke("JoinRoom", roomName);
        }

        #endregion

    }
}

