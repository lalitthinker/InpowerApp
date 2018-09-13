using Android.Support.V7.Widget;
using System;

namespace InPowerApp.Activities
{
    public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, bool loadCalled);
        public event LoadMoreEventHandler LoadMoreEvent;

        private LinearLayoutManager LayoutManager;

        public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter().ItemCount;
            var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount)
            {
                LoadMoreEvent(this, true);
            }
        }
    }

    public class XamarinRecyclerUpViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, bool loadCalled);
        public event LoadMoreEventHandler LoadMoreEvent;

        private LinearLayoutManager LayoutManager;
        private static int firstVisibleInListview;

        public bool LoadList;


        public XamarinRecyclerUpViewOnScrollListener(LinearLayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
            firstVisibleInListview = LayoutManager.FindFirstVisibleItemPosition();
           
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            int currentFirstVisible = LayoutManager.FindFirstVisibleItemPosition();

            if (currentFirstVisible == 0)
            {
                LoadMoreEvent(this, true);
            }
        }
    }
}