using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using InPowerApp.Fragments;
using Java.Lang;

namespace InPowerApp.ListAdapter
{
    public class CustomPagerAdapterForBooksShelf : FragmentPagerAdapter
    {
        private static readonly string[] Content = new[] { "READ", "WISH LIST" };

        public CustomPagerAdapterForBooksShelf(Android.Support.V4.App.FragmentManager p0)
                : base(p0)
        { }

        public override int Count
        {
            get { return Content.Length; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new ReadFragment();
            }

            return new WishListFragment();
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0) { return new Java.Lang.String(Content[p0 % Content.Length].ToUpper()); }
    }

}