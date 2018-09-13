using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
    public class GroupRequestViewModel
    {
        public long GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int InterestId { get; set; }
        public long MemberCount { get; set; }
        public string PictureUrl { get; set; }
        public int Visibility { get; set; }

        public bool IsHidden { get; set; }
        public bool IsPrivate { get; set; }
        public string Message { get; set; }
        public long MobileDatabaseId { get; set; }
        public long GroupMessageId { get; set; }
        public int BookId { get; set; }
        public GroupType GroupType { get; set; }
        public long UserId { get; set; }
        public List<GroupMemberViewModel> Members { get; set; }
    }
    public class GroupMemberViewModel
    {
        public long GroupId { get; set; }
        public long MemberId { get; set; }
        public string MemberName { get; set; }

    }
    public enum GroupType
    {
        Genral = 1,
        BookClub,
        MasterMind
    }
    public class GroupResponseViewModel
    {
        public long GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long InterestId { get; set; }
        public long MemberCount { get; set; }
        public string PictureUrl { get; set; }
        public long UserId { get; set; }
        public int Visibility { get; set; }
        public string Message { get; set; }
        public long GroupMessageId { get; set; }
        public DateTime MessageTime { get; set; }
        public List<GroupMemberViewModel> Members { get; set; }
        public ChatForGroupViewModel ChatModel { get; set; }
    }

    public class ChatForGroupViewModel
    {
        public long? ContactId { get; set; }
        public long? GroupId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime? LastActiveTime { get; set; }
        public long UserId { get; set; }
        public bool IsGroup { get; set; }
    }

    public class ContactProfileViewModel
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public string ProfileImageUrl { get; set; }
        public string AboutMe { get; set; }
        public bool isActive { get; set; }
        public bool isShoutout { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string AndroidToken { get; set; }
        public string IOSToken { get; set; }

    }
    public class GroupViewModel
    {
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


    public class GroupMessageRequestViewModel
    {
        public long GroupId { get; set; }
        public long? MessageId { get; set; }
        public string Message { get; set; }
        public long SenderId { get; set; }

        public DateTime MessageTime { get; set; }
        public bool IsSend { get; set; }
        public long MobileDatabaseId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
    }
    public class GroupMessageResponseViewModel
    {
        public long GroupId { get; set; }
        public long MessageId { get; set; }
        public string Message { get; set; }
        public long SenderId { get; set; }
        public string SenderProfileName { get; set; }
        public string SenderProfilePicUrl { get; set; }
        public DateTime MessageTime { get; set; }
        public bool IsSend { get; set; }
        public long MobileDatabaseId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<GroupMessageStatusViewModel> GroupMessageStatuses { get; set; }
        public string FU { get; set; }
        public string TU { get; set; }
    }

    public class GroupMessageStatusViewModel
    {
        public long GroupMessageId { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public bool IsRead { get; set; }
        public bool IsRecieved { get; set; }
        public bool IsSend { get; set; }
        public string FU { get; set; }
        public string TU { get; set; }
    }
}
