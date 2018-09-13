using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace InPowerApp.ListAdapter
{
    class ImageAdapter : PagerAdapter
    {
        private Context context;
        private int[] thumbIds = {
        Resource.Drawable.welcometoinpoweraap,
        Resource.Drawable.welcometoinpoweraap1,
        Resource.Drawable.welcometoinpoweraap2,
            Resource.Drawable.welcometoinpoweraap3
    };
        public ImageAdapter(Context c)
        {
            this.context = c;
        }
        public override int Count
        {
            get
            {
                return thumbIds.Length;
            }
        }
        //public override Java.Lang.Object GetItem(int position)
        //{
        //    return null;
        //}
        //public override long GetItemId(int position)
        //{
        //    return 0;
        //}
        //// create a new ImageView for each item referenced by the Adapter  
        //public override View GetView(int position, View convertView, ViewGroup parent)
        //{
        //    ImageView i = new ImageView(context);
        //    i.SetImageResource(thumbIds[position]);
        //    i.SetScaleType(ImageView.ScaleType.FitXy);
        //    return i;
        //}
        // references to our images  


        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == ((ImageView)@object);
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            ImageView i = new ImageView(context);
            i.SetScaleType(ImageView.ScaleType.CenterCrop);
            i.SetImageResource(thumbIds[position]);
            ((ViewPager)container).AddView(i, 0);
            return i;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object @object)
        {
            ((ViewPager)container).RemoveView((ImageView)@object);
        }
    }

}