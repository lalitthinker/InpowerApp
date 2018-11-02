using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InPowerIOS.Common;
using InPowerIOS.Model;
using Microsoft.AppCenter.Crashes;
using PCL.Model;
using SQLite;

namespace InPowerIOS.Repositories
{
    public class BookRepository
    {
        public static List<Books> SaveBookList(List<BookViewModel> lstBooks,BookStatus bookStatus)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            Books bookModel = null;

            var lstBook = new List<Books>();
            try
            {
                foreach (var book in lstBooks)
                {
                    try
                    {
                        bookModel = CheckBook(book.BookId);
                        if (bookModel == null)
                        {
                            bookModel = new Books();
                            bookModel.BookId = book.BookId;
                            bookModel.BookPictureUrl = book.BookPictureUrl;
                            bookModel.BookUrl = book.BookUrl;
                            bookModel.Author = book.Author;
                            bookModel.Description = book.Description;
                            bookModel.Edition = book.Edition;
                            bookModel.ISBN = book.ISBN;
                            bookModel.PublicationDate = book.PublicationDate;
                            bookModel.Publisher = book.Publisher;
                            bookModel.Title = book.Title;
                            bookModel.UserId = CommonHelper.GetUserId();
                            bookModel.BookStatus = (int)bookStatus;
                            db.Insert(bookModel);
                            lstBook.Add(bookModel);
                        }
                        else
                        {
                           
                        }
                        db.Commit();
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        Console.WriteLine(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Console.WriteLine(ex.Message, ex);
                lstBook = new List<Books>();
            }
            db.Close();
            return lstBook;
        }



        public static Books UpdateBook(BookViewModel lstBooks)
        {
            var db = new SQLiteConnection(CommonConstant.DBPath);
            Books bookModel = null;

            try
            {
                bookModel = CheckBook(lstBooks.BookId);
                if (bookModel != null)
                {
                    bookModel.BookStatus = (int)lstBooks.BookStatus;//0 = Removed , 1=Read,2=WishList

                    db.Update(bookModel);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return bookModel;
        }

        public static Books CheckBook(long bookId)
        {
            Books book = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                book =
                    db.Query<Books>("select * from 'Books' where BookId=" + bookId + " and UserId=" + CommonHelper.GetUserId())
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
            db.Close();
            return book;
        }

        public static List<Books> GetBooks(BookStatus Status, List<Books> _booklist)
        {
            List<Books> lstbooks = null;
            var db = new SQLiteConnection(CommonConstant.DBPath);
            try
            {
                db.Trace = true;
                int _bookListCount = _booklist != null ? _booklist.Count : 0;

                var l = db.Query<Books>("select * from 'Books' where BookStatus=" + (int)Status + " and UserId=" + CommonHelper.GetUserId())
                       .OrderBy(m => m.BookId);

                // var l = db.Query<Books>("select * from 'Books'")
                // .OrderBy(m => m.BookId).Skip(_bookListCount).Take(CommonConstant.MessagePageSize);
                //.Skip(pageIndex * CommonConstant.MessagePageSize)
                //.Take(CommonConstant.MessagePageSize);

                lstbooks = l.ToList();
                //if (_booklist != null)
                //{
                //    _booklist.Union(lstbooks);

                //    lstbooks = _booklist;
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                lstbooks = new List<Books>();
            }
            db.Close();
            return lstbooks;
        }
    }
}