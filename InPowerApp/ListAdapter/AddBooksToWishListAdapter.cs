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
using Java.Lang;

namespace InPowerApp.ListAdapter
{
    //public interface OnBottomReachedListener
    //{

    //    void onBottomReached(int position);


    //}
    public class AddBooksToWishListAdapter : RecyclerView.Adapter, IFilterable
    {
        // OnBottomReachedListener onBottomReachedListener;
        // Event handler for item clicks:
        public event EventHandler<int> ListReloadWishList;

        // Underlying data set (a photo album):
        // Underlying data set (a photo album):
        public List<BookViewModel> searchBooks;
        public List<BookViewModel> originalBooks;
        public Filter Filter { get; private set; }

        Activity Contextt;

        //public void setOnBottomReachedListener(OnBottomReachedListener onBottomReachedListener)
        //{

        //    this.onBottomReachedListener = onBottomReachedListener;
        //}

        // Load the adapter with the data set (photo album) at construction time:
        public AddBooksToWishListAdapter(List<BookViewModel> Book, Activity Context)
        {
            originalBooks = Book.ToList();
            this.Contextt = Context;
            Filter = new SearchBookFilter(this);
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.BookListItem, parent, false);



            BoolListHolder vh = new BoolListHolder(itemView, OnClick);



            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BoolListHolder vh = holder as BoolListHolder;
            var item = originalBooks[position];
            vh.BookTitle.Text = item.Title;
            vh.BookAuthor.Text = item.Author;
            if (item.BookPictureUrl != null)
            {
                CommonHelper.SetImageOnUIImageView(vh.BookImage, item.BookPictureUrl, Contextt,400,400);
            }
            BookViewModel obitmobEvent = originalBooks[position];
            var ReadListEvent = new ReadListnerWithActivity(obitmobEvent, Contextt);
            var WishListEvent = new btnWishListAdapterButtonClickListener(obitmobEvent, Contextt);
            vh.WishListButton.SetOnClickListener(WishListEvent);
            vh.ReadButton.SetOnClickListener(ReadListEvent);

            WishListEvent.ListReloadWishListListner += RefreshList;
            ReadListEvent.ListReload += RefreshList;


        }

        public void RefreshList(object o, int bookId)
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
            //if (ItemClick != null)
            //    ItemClick(this, position);
        }

        public void add(BookViewModel book)
        {
            originalBooks.Add(book);
        }

        public void removeBook(BookViewModel book)
        {
            originalBooks.Remove(book);
        }
    }



    public class BoolListHolder : RecyclerView.ViewHolder
    {
        public TextView BookTitle { get; set; }
        public TextView BookAuthor { get; set; }
        public ImageView BookImage { get; set; }
        public Button ReadButton { get; set; }
        public Button WishListButton { get; set; }

        public EventHandler<int> ReadClick { get; set; }
        public EventHandler<int> WishListClick { get; set; }

        public Action<int> _listener;

        public BoolListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            BookTitle = itemView.FindViewById<TextView>(Resource.Id.BookTitle);
            BookAuthor = itemView.FindViewById<TextView>(Resource.Id.BookAuthor);
            BookImage = itemView.FindViewById<ImageView>(Resource.Id.BookImage);
            ReadButton = itemView.FindViewById<Button>(Resource.Id.btnRead);
            WishListButton = itemView.FindViewById<Button>(Resource.Id.btnWishList);

            // itemView.Click += (sender, e) => listener(base.LayoutPosition);

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

    public class SearchBookFilter : Filter
    {
        private readonly AddBooksToWishListAdapter _adapter;
        public SearchBookFilter(AddBooksToWishListAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnObj = new FilterResults();
            var results = new List<BookViewModel>();
            if (_adapter.searchBooks == null)
                _adapter.searchBooks = _adapter.originalBooks;

            if (constraint == null) return returnObj;

            if (_adapter.searchBooks != null && _adapter.searchBooks.Any())
            {
                // Compare constraint to all names lowercased. 
                // It they are contained they are added to results.
                results.AddRange(
                    _adapter.searchBooks.Where(
                        book => book.Title.ToLower().Contains(constraint.ToString()) || book.Author.ToLower().Contains(constraint.ToString())));
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
                    .Select(r => r.ToNetObject<BookViewModel>()).ToList();

            _adapter.NotifyDataSetChanged();

            // Don't do this and see GREF counts rising
            constraint.Dispose();
            results.Dispose();
        }
    }
}