using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InPowerApp.Model
{
    public class PhoneContactModel
    {
        public string mobileContact { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string photo { get; set; }
        public string photoId { get; set; }
        public long contactId { get; set; }
        private bool selected { get; set; }
    }
}