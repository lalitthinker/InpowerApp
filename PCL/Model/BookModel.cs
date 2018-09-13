using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Model
{
    public class BooksMapViewModel
    {
        public long UserId { get; set; }
        public long BookId { get; set; }
        public int IsRead { get; set; }
        public int SkipRecords { get; set; }
        public int TakeRecords { get; set; }
        public string SearchText { get; set; }
    }
    public class BookViewModel
    {
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
        public BookStatus BookStatus { get; set; }
    }
    public enum BookStatus
    {
        Removed =0,
        Read = 1,
        WishList = 2
    }
}
