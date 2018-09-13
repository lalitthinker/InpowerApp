using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InPowerApp.Common;
using InPowerApp.Model;
using PCL.Common;
using Square.Picasso;
using PCL.Service;
using PCL.Model;
using InPowerApp.Repositories;
using InPowerApp.Fragments;
using Java.Lang;

namespace InPowerApp.ListAdapter
{
    public class WishListBookListAdapter : RecyclerView.Adapter, IFilterable
    {
        // Event handler for item clicks:
        public event EventHandler<int> ListReloadWishList;


        // Underlying data set (a photo album):
        public List<Books> searchBooks;
        public List<Books> originalBooks;
        public Filter Filter { get; private set; }

        Context context_wishList;

        // Load the adapter with the data set (photo album) at construction time:
        public WishListBookListAdapter(List<Books> Book, Context context)
        {
            originalBooks = Book.ToList();
            this.context_wishList = context;
            Filter = new WishListBookFilter(this);
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.BookListItemWishList, parent, false);

            WishListBookListHolder vh = new WishListBookListHolder(itemView, OnClick);


            return vh;
        }



        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            WishListBookListHolder vh = holder as WishListBookListHolder;
            var item = originalBooks[position];
            vh.BookTitle.Text = item.Title;
            vh.BookAuthor.Text = item.Author;
            if (item.BookPictureUrl != null)
            {
                CommonHelper.SetImageOnUIImageView(vh.BookImage, item.BookPictureUrl, context_wishList,400,400);
            }

            Books obitmobEvent = originalBooks[position];
            var ReadListEvent = new WishListButtonReadAdapterButtonClickListener(obitmobEvent, context_wishList);
            var RemoveBookEvent = new WishListButtonRemoveAdapterButtonClickListener(obitmobEvent, context_wishList);
            vh.ReadButton.SetOnClickListener(ReadListEvent);
            vh.RemoveButton.SetOnClickListener(RemoveBookEvent);


            ReadListEvent.ListReloadRead += RefreshList;
            RemoveBookEvent.ListReloadWishList += RefreshList2;


        }

        public void RefreshList(object o, int bookId)
        {
            if (bookId != 0)
            {
                this.ListReloadWishList(this, bookId);
            }
        }


        public void RefreshList2(object o, int bookId)
        {
            if (bookId != 0)
            {
                this.ListReloadWishList(this, bookId);
            }
        }


        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return originalBooks.Count; }
        }


        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            //if (list != null)
            //    ItemClick(this, position);
        }

        public void add(Books book)
        {
            originalBooks.Add(book);
        }

        public void removeBook(Books book)
        {
            originalBooks.Remove(book);
        }
    }

    public class WishListBookListHolder : RecyclerView.ViewHolder
    {
        public TextView BookTitle { get; set; }
        public TextView BookAuthor { get; set; }
        public ImageView BookImage { get; set; }
        public Button ReadButton { get; set; }
        public Button RemoveButton { get; set; }

        public EventHandler<int> ReadClick { get; set; }
        public EventHandler<int> WishListClick { get; set; }

        public Action<int> _listener;

        public WishListBookListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            BookTitle = itemView.FindViewById<TextView>(Resource.Id.BookTitle);
            BookAuthor = itemView.FindViewById<TextView>(Resource.Id.BookAuthor);
            BookImage = itemView.FindViewById<ImageView>(Resource.Id.BookImage);
            ReadButton = itemView.FindViewById<Button>(Resource.Id.btnReadWishList);
            RemoveButton = itemView.FindViewById<Button>(Resource.Id.btnRemoveWishList);
        }

        public void SetClickListener(Action<int> listener)
        {
            _listener = listener;
            // ItemView.Click += HandleClick;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (ItemView != null)
            {
                //  ItemView.Click -= HandleClick;
            }
            _listener = null;
        }

        void HandleClick(object sender, int e)
        {
            if (_listener != null)
            {
                _listener(base.AdapterPosition);
            }
        }
    }

    internal class WishListButtonRemoveAdapterButtonClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private Books obitmobEvent;
        private Context context_wishList;
        public EventHandler<int> ListReloadWishList;



        public WishListButtonRemoveAdapterButtonClickListener(Books obitmobEvent, Context context)
        {
            this.obitmobEvent = obitmobEvent;
            this.context_wishList = context;
        }


        public void Dispose()
        {

        }

        public void OnClick(View v)
        {
            BooksMapViewModel _model = new BooksMapViewModel();
            _model.BookId = obitmobEvent.BookId;
            _model.IsRead = 1;
            _model.UserId = CommonHelper.GetUserId();
            RemoveBook(_model);
        }

        public async void RemoveBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().RemoveBook(_model);
            if (result.Status == 1)
            {
                BookViewModel _viewModel = new BookViewModel();
                _viewModel.BookId = _model.BookId;
                _viewModel.BookStatus = BookStatus.Removed;
                var savedBooks = BookRepository.UpdateBook(_viewModel);
                if (savedBooks != null)
                {
                    Toast.MakeText(context_wishList, "Book successfully removed", ToastLength.Long).Show();
                    this.ListReloadWishList(context_wishList, (int)_model.BookId);
                }
                else
                {
                    Toast.MakeText(context_wishList, "Failed to remove book", ToastLength.Long).Show();
                    this.ListReloadWishList(context_wishList, 0);
                }
            }
            //  this.ListReloadWishList(this, 0);
        }


    }

    internal class WishListButtonReadAdapterButtonClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private Books obitmobEvent;
        private Context context_read;
        public EventHandler<int> ListReloadRead;


        public WishListButtonReadAdapterButtonClickListener(Books obitmobEvent, Context context)
        {
            this.obitmobEvent = obitmobEvent;
            this.context_read = context;
        }


        public void Dispose()
        {

        }

        public void OnClick(View v)
        {
            BooksMapViewModel _model = new BooksMapViewModel();
            _model.BookId = obitmobEvent.BookId;
            _model.IsRead = 1;
            _model.UserId = CommonHelper.GetUserId();
            SaveBook(_model);
        }

        public async void SaveBook(BooksMapViewModel _model)
        {
            var result = await new BookShelfService().UpdateBook(_model);
            if (result.Status == 1)
            {
                BookViewModel _viewModel = new BookViewModel();
                _viewModel.BookId = _model.BookId;
                _viewModel.BookStatus = BookStatus.Read;
                var savedBooks = BookRepository.UpdateBook(_viewModel);
                if (savedBooks != null)
                {
                    Toast.MakeText(this.context_read, "Book successfully added to read list", ToastLength.Long).Show();
                    this.ListReloadRead(this.context_read, (int)_model.BookId);
                }
                else
                {
                    Toast.MakeText(this.context_read, "Failed to add book to read list", ToastLength.Long).Show();
                    this.ListReloadRead("test", 0);
                }
            }
            //  ListReloadRead("test", 0);
        }
    }

    public class WishListBookFilter : Filter
    {
        private readonly WishListBookListAdapter _adapter;
        public WishListBookFilter(WishListBookListAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnObj = new FilterResults();
            var results = new List<Books>();
            if (_adapter.searchBooks == null)
                _adapter.searchBooks = _adapter.originalBooks;

            if (constraint == null) return returnObj;

            if (_adapter.searchBooks != null && _adapter.searchBooks.Any())
            {
                // Compare constraint to all names lowercased. 
                // It they are contained they are added to results.
                results.AddRange(
                  _adapter.searchBooks.Where(
                      book => ((book.Title != null) ? book.Title.ToLower().Contains(constraint.ToString()) : false) || ((book.Author != null) ? book.Author.ToLower().Contains(constraint.ToString()) : false)));
            }

            // Nasty piece of .NET to Java wrapping, be careful with this!
            returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            returnObj.Count = results.Count;

            constraint.Dispose();

            return returnObj;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
                _adapter.originalBooks = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<Books>()).ToList();

            _adapter.NotifyDataSetChanged();

            // Don't do this and see GREF counts rising
            constraint.Dispose();
            results.Dispose();
        }
    }


}