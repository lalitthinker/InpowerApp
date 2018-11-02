using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;

namespace InPowerIOS.Model
{
   public  class ChatMessage
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public bool isDeleted { get; set; }
        public bool isHidden { get; set; }
        public long ChatMessageId { get; set; }
        public long ContactId { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageTime { get; set; }
        public bool IsRead { get; set; }
        public bool IsRecieved { get; set; }
        public bool IsSend { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
    }
}