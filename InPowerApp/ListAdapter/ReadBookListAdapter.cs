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
using Java.Lang;

namespace InPowerApp.ListAdapter
{
    public class ReadBookListAdapter : RecyclerView.Adapter, IFilterable
    {
        // Event handler for item clicks:
        public event EventHandler<int> ListReload;


        // Underlying data set (a photo album):
        public List<Books> searchBooks;
        public List<Books> originalBooks;
        public Filter Filter { get; private set; }

        Context context;

        // Load the adapter with the data set (photo album) at construction time:
        public ReadBookListAdapter(List<Books> Book,Context context)
        {
            originalBooks = Book.ToList();
            this.context = context;
            Filter = new ReadBookFilter(this);
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.BookListItemRead, parent, false);

          

            ReadBoolListHolder vh = new ReadBoolListHolder(itemView, OnClick);
            

          
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ReadBoolListHolder vh = holder as ReadBoolListHolder;
            var item = originalBooks[position];
            vh.ReadBookTitle.Text = item.Title;
            vh.ReadBookAuthor.Text = item.Author;
            if (item.BookPictureUrl != null)
            {
                CommonHelper.SetImageOnUIImageView(vh.ReadBookImage, item.BookPictureUrl, context,600,600);
            }

           Books obitmobEvent = originalBooks[position];
            var RemoveBookEvent = new ReadBookRemoveAdapterButtonClickListener(obitmobEvent, context);
       
            vh.RemoveButton.SetOnClickListener(RemoveBookEvent);

            RemoveBookEvent.ListReload += RefreshList;
           
        }

        public void RefreshList(object o, int bookId)
        {
            if (bookId != 0)
            {
                this.ListReload(this, bookId);
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
            //if (ListReload != null)
            //    ListReload(this, position);
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



    public class ReadBoolListHolder : RecyclerView.ViewHolder
    {
        public TextView ReadBookTitle { get; set; }
        public TextView ReadBookAuthor { get; set; }
        public ImageView ReadBookImage { get; set; }
        public Button RemoveButton { get; set; }

        public Action<int> _listener;

        public ReadBoolListHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            ReadBookTitle = itemView.FindViewById<TextView>(Resource.Id.ReadBookTitle);
            ReadBookAuthor = itemView.FindViewById<TextView>(Resource.Id.ReadBookAuthor);
            ReadBookImage = itemView.FindViewById<ImageView>(Resource.Id.ReadBookImage);
            RemoveButton = itemView.FindViewById<Button>(Resource.Id.btnRemoveRead);
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

    internal class ReadBookRemoveAdapterButtonClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private Books obitmobEvent;
        private Context context;
        public EventHandler<int> ListReload;

        public ReadBookRemoveAdapterButtonClickListener(Books obitmobEvent, Context context)
        {
            this.obitmobEvent = obitmobEvent;
            this.context = context;
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
                    Toast.MakeText(context, "Book successfully removed", ToastLength.Long).Show();
                    this.ListReload(this, (int)_model.BookId);
                }
                else
                {
                    Toast.MakeText(context, "Failed to remove book", ToastLength.Long).Show();
                    this.ListReload(this, 0);
                }
            }
            this.ListReload(this, 0);
        }

    }

    public class ReadBookFilter : Filter
    {
        private readonly ReadBookListAdapter _adapter;
        public ReadBookFilter(ReadBookListAdapter adapter)
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
                        book => ((book.Title!=null)? book.Title.ToLower().Contains(constraint.ToString()):false) || ((book.Author != null) ? book.Author.ToLower().Contains(constraint.ToString()) : false)));
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