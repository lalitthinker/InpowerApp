using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PCL.Model;
using SQLite;

namespace InPowerApp.Model
{
  
    public class ChatConversation
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public long ChatId { get; set; }
        public string Tags { get; set; }
        public long OwnerId { get; set; }
        public long? ContactId { get; set; }
        public int Visibility { get; set; }
        public bool isHidden { get; set; }
        public string LastMessage { get; set; }
        public string SenderName { get; set; }
        public long SmallestMessageId { get; set; }
        public int pendingChatMessage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public long UserId { get; set; }
        public bool IsBlock { get; set; }
        //group fields
        public bool IsGroup { get; set; }
        public long? GroupId { get; set; }
        //public string GroupName { get; set; }
        //public string MemberCount { get; set; }
        //public string GroupDescription { get; set; }
        //public long InterestId { get; set; }
        //public string GroupPictureUrl { get; set; }

    }


    public class ChatModel
    {
        public long? ContactId { get; set; }
        public long? GroupId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public bool IsRead { get; set; }
        public UserProfile ContactProfile { get; set; }
        public GroupModel GroupDetails { get; set; }
        public DateTime? LastActiveTime { get; set; }
        public string email { get; set; }
        public long UserId { get; set; }
        public bool IsGroup { get; set; }
        public List<GroupMemberViewModel> GroupMember { get; set; }
    }
    public class UserRegister
    {

        
        public long UserId { get; set; }

     
        public string Email { get; set; }

        
        public string Password { get; set; }


    }
    public class ChatAttachment
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public string type { get; set; }
        public string url { get; set; }
        public bool isDownloaded { get; set; }
        public string downloadPath { get; set; }
        public long chatMessageId { get; set; }
        public long contactId { get; set; }
        public long UserId { get; set; }
        public int IsGroup { get; set; }
    }
}