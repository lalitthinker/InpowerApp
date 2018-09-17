using System;
using InPowerIOS.Models;

namespace InPowerIOS.NewChat
{
    public class Message
    {
        public MessageType Type { get; set; }
        public string Text { get; set; }
        public string MessageDateTime { get; set; }
    }
}
