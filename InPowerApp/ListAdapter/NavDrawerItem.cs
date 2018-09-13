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

namespace InPowerApp.ListAdapter
{
    public class NavDrawerItem
    {
        private String title;
        private int icon;
        private String count = "0";
        // boolean to set visiblity of the counter
        private Boolean isCounterVisible = false;

        public NavDrawerItem()
        {
        }

        public NavDrawerItem(String title, int icon)
        {
            this.title = title;
            this.icon = icon;
        }

        public NavDrawerItem(String title, int icon, Boolean isCounterVisible,
                String count)
        {
            this.title = title;
            this.icon = icon;
            this.isCounterVisible = isCounterVisible;
            this.count = count;
        }

        public String getTitle()
        {
            return this.title;
        }

        public int getIcon()
        {
            return this.icon;
        }

        public String getCount()
        {
            return this.count;
        }

        public Boolean getCounterVisibility()
        {
            return this.isCounterVisible;
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public void setIcon(int icon)
        {
            this.icon = icon;
        }

        public void setCount(String count)
        {
            this.count = count;
        }

        public void setCounterVisibility(Boolean isCounterVisible)
        {
            this.isCounterVisible = isCounterVisible;
        }
    }
}