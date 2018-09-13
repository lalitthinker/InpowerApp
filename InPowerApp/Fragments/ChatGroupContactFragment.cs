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
using InPowerApp.ListAdapter;
using InPowerApp.Model;

namespace InPowerApp.Fragments
{
    public class ChatGroupContactFragment : Android.Support.V4.App.Fragment
    {
        ViewPager viewPager;
        TabLayout tabLayoutForContacts;
        private int TabIndex;

        public ChatGroupContactFragment(int tabIndex)
        {
            this.TabIndex = tabIndex;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ChatGroupContact_Fragments, container, false);

            viewPager = view.FindViewById<ViewPager>(Resource.Id.pager);
            tabLayoutForContacts = view.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            //Since we are a fragment in a fragment you need to pass down the child fragment manager!


            FragmentPagerAdapter adapter = new CustomPagerAdapterForContacts(ChildFragmentManager);
            viewPager.Adapter = adapter;
            viewPager.PageSelected += ViewPager_PageSelected;
            tabLayoutForContacts.SetupWithViewPager(viewPager);
            tabLayoutForContacts.TabSelected += TabLayoutForContacts_TabSelected;
            TabLayout.Tab tab = tabLayoutForContacts.GetTabAt(TabIndex);
            tab.Select();
            return view;
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {

        }
        //int PreviousTab = 0;static int newtab = 0;
        private void TabLayoutForContacts_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            //    PreviousTab = newtab;
            //    var tab = e.Tab;
            //    newtab = e.Tab.Position;

            //    switch (e.Tab.Position)
            //    {
            //        case 0:
            //            SelectedTabForChat.TabName = "Chat";
            //            break;
            //        case 1:
            //            SelectedTabForChat.TabName = "Group";
            //            break;
            //        case 2:
            //            SelectedTabForChat.TabName = "Contact";
            //            break;
            //    }
            //    if (PreviousTab != newtab)
            //    {
            //        //Android.Support.V4.App.Fragment fragment = null;
            //        //var ft = this.Activity.SupportFragmentManager.BeginTransaction();
            //        //fragment = new ChatGroupContactFragment(e.Tab.Position);
            //        //if (fragment != null)
            //        //{
            //        //    ft.Replace(Resource.Id.content_frame, fragment);
            //        //    ft.Commit();
            //        //}
            //    }
            //    // viewPager.SetCurrentItem(e.Tab.Position, true);
        }
    }
}