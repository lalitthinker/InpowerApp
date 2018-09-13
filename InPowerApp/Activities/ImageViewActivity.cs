using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using InPowerApp.ListAdapter;

namespace InPowerApp.Activities
{
    [Activity(Label = "InPowerApp")]
    public class ImageViewActivity : AppCompatActivity, ViewPager.IOnPageChangeListener 
    {
        private const float V = 35;

        //private Context context;
        private TextView[] mDotTextView;
        private LinearLayout mDotRelativeLayout;
        private int mCurrentPage;
        ViewPager mSlideViewPager;
        Button btnSkipinstructionsViewPager, btnBackinstructionsViewPager, btnNextinstructionsViewPager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ImageGallerylayout);

            mDotRelativeLayout = FindViewById<LinearLayout>(Resource.Id.DotRelativeLayout);
            btnSkipinstructionsViewPager = FindViewById<Button>(Resource.Id.btnSkipinstructionsViewPager);
            btnBackinstructionsViewPager = FindViewById<Button>(Resource.Id.btnBackinstructionsViewPager);
            btnNextinstructionsViewPager = FindViewById<Button>(Resource.Id.btnNextinstructionsViewPager);
            mSlideViewPager = FindViewById<ViewPager>(Resource.Id.instructionsViewPager);
            ImageAdapter adapter = new ImageAdapter(this);
            mSlideViewPager.Adapter = adapter;

            SetDot(Position: 0);
            btnSkipinstructionsViewPager.Click += BtnSkipinstructionsViewPager_Click;
            btnBackinstructionsViewPager.Click += BtnBackinstructionsViewPager_Click;
            btnNextinstructionsViewPager.Click += BtnNextinstructionsViewPager_Click;
            mSlideViewPager.AddOnPageChangeListener(this);
        }

        private void BtnNextinstructionsViewPager_Click(object sender, EventArgs e)
        {
            if (btnNextinstructionsViewPager.Text == "FINISH")
            {
                this.Finish();
                StartActivity(typeof(LoginForm));
            }

            mSlideViewPager.SetCurrentItem(mCurrentPage + 1, true);
        }

        private void BtnBackinstructionsViewPager_Click(object sender, EventArgs e)
        {
            mSlideViewPager.SetCurrentItem(mCurrentPage - 1, true);
        }

        private void BtnSkipinstructionsViewPager_Click(object sender, EventArgs e)
        {
            this.Finish();
            StartActivity(typeof(LoginForm));
        }
        

        public void SetDot(int Position)
        {
            mDotTextView = new TextView[4];
            mDotRelativeLayout.RemoveAllViews();
            for (int i = 0; i < mDotTextView.Length; i++)
            {
                mDotTextView[i] = new TextView(context: this);
                mDotTextView[i].SetText(Resource.String.imageViewstring);
                mDotTextView[i].SetTextSize(Android.Util.ComplexUnitType.Dip, V);
                mDotTextView[i].SetTextColor(color: Android.Graphics.Color.White);
                mDotRelativeLayout.AddView(mDotTextView[i]);
            }



            if (mDotTextView.Length > 0)
            {
                mDotTextView[Position].SetTextColor(color: Android.Graphics.Color.Yellow);
            }

        }
        
        public void OnPageScrollStateChanged(int state)
        {
           
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
           
        }

        public void OnPageSelected(int position)
        {
            SetDot(Position: position);
            mCurrentPage = position;

            if (position == 0)
            {
                btnNextinstructionsViewPager.Enabled = true;
                btnBackinstructionsViewPager.Enabled = false;
                btnBackinstructionsViewPager.Visibility = ViewStates.Invisible;
                btnNextinstructionsViewPager.Text = "NEXT";
                btnBackinstructionsViewPager.Text = "";
            }


            else if (position == mDotTextView.Length - 1)
            {
                btnNextinstructionsViewPager.Enabled = true;
                btnBackinstructionsViewPager.Enabled = true;
                btnBackinstructionsViewPager.Visibility = ViewStates.Visible;
                btnNextinstructionsViewPager.Text = "FINISH";
                btnBackinstructionsViewPager.Text = "BACK";
            }
            else 
            {
                btnNextinstructionsViewPager.Enabled = true;
                btnBackinstructionsViewPager.Enabled = true;
                btnBackinstructionsViewPager.Visibility = ViewStates.Visible;
                btnNextinstructionsViewPager.Text = "NEXT";
                btnBackinstructionsViewPager.Text = "BACK";
            }
        }

    
    }
}