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