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
using SQLite;

namespace InPowerApp.Model
{
    
    public class Contact
    {
        

        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public string name { get; set; }
        public long? number { get; set; }
        public string source { get; set; }
        public string screenName { get; set; }
        public long contactId { get; set; }
        public string socialContactId { get; set; }
        public string contactPicUrl { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Aboutme { get; set; }
        public string email { get; set; }
        public long UserId { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}