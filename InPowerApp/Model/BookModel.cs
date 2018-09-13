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
    
    public class Books
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public long BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Edition { get; set; }
        public string PublicationDate { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string BookPictureUrl { get; set; }
        public string BookUrl { get; set; }
        public int BookStatus { get; set; }
        public long UserId { get; set; }
    }

}