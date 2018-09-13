using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
    public class ChatMessageViewModel
    {
        public long ContactId { get; set; }
        public long ChatId { get; set; }
        public long ChatMessageId { get; set; }
        public string Message { get; set; }
        public DateTime MessageTime { get; set; }
        public bool IsRead { get; set; }
        public bool IsRecieved { get; set; }
        public long MobiledatabaseId { get; set; }
        public string FU { get; set; }
        public string TU { get; set; }
        public bool  IsSend { get; set; }
        
        public List<AttachmentViewModel> Attachments { get; set; }
    }
    public class AttachmentViewModel
    {
        public string Type { get; set; }
        public string Url { get; set; }
    }
  

}
