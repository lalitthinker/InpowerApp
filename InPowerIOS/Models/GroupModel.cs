using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using SQLite;

namespace InPowerIOS.Model
{
    public class GroupModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public long InterestId { get; set; }
        public long MemberCount { get; set; }
        public string GroupPictureUrl { get; set; }

        public long OwnerId { get; set; }

        public int Visibility { get; set; }
        public bool isHidden { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public long UserId { get; set; }
    }
    public class GroupMessage
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public long GroupMessageId { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime MessageTime { get; set; }
        public int? Visibility { get; set; }

        public bool isExpired { get; set; }

        public long MessageId { get; set; }
        public bool IsRead { get; set; }

        public string MessageText { get; set; }
        public long GroupId { get; set; }
        public string senderName { get; set; }
        public string senderPicUrl { get; set; }
        public long SenderUserId { get; set; }
        public long UserId { get; set; }
    }
    public class GroupAttachment
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public long? groupId { get; set; }
        public long groupMessageId { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public bool isDownloaded { get; set; }
        public string downloadPath { get; set; }
        public long UserId { get; set; }
    }

    [Preserve(AllMembers = true)]
    public class GroupMember
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public long GroupMemberId { get; set; }

        public int VisibilityLevel { get; set; }
        public string MemberName { get; set; }
        public long GroupId { get; set; }
        public string PictureUrl { get; set; }
        public long UserId { get; set; }
    }

    public class GroupMessageStatus
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public long GroupMessageId { get; set; }
        public long ReceiverId { get; set; }
        public bool IsRead { get; set; }
        public bool IsRecieved { get; set; }
        public bool IsSend { get; set; }
        public long UserId { get; set; }
    }
}