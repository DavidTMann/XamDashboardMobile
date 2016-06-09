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
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace MobileDashboard.mTabs
{
    class TabsPageAdapter : FragmentPagerAdapter
    {
        private Fragment[] fragments;

        public TabsPageAdapter(FragmentManager fm, params Fragment[] fragments) : base(fm)
        {
            this.fragments = fragments;
        }

        public override int Count
        {
            get
            {
                return fragments.Length;
            }
        }

        public override Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            var title = String.Format("Tab {0}", position + 1);
            return new Java.Lang.String(title);
        }
    }
}