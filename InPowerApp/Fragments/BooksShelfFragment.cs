using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using InPowerApp.Activities;
using InPowerApp.ListAdapter;

namespace InPowerApp.Fragments
{
    public class BooksShelfFragment : Android.Support.V4.App.Fragment
    {
        ViewPager viewPager;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.BooksShelf_Fragments, container, false);

            viewPager = view.FindViewById<ViewPager>(Resource.Id.pager);
            var tabLayoutForContacts = view.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            //Since we are a fragment in a fragment you need to pass down the child fragment manager!

            FragmentPagerAdapter adapter = new CustomPagerAdapterForBooksShelf(ChildFragmentManager);
            viewPager.Adapter = adapter;
            tabLayoutForContacts.SetupWithViewPager(viewPager);


            return view;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
               
            }

            return base.OnOptionsItemSelected(item);
        }
       
    }
}