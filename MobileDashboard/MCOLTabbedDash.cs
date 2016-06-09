using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using MobileDashboard.mTabs;
using System;

namespace MobileDashboard
{
    [Activity(Label = "MCOL Dashboard")]
    public class MCOLTabbedDash : FragmentActivity
    {
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Create your application here
            SetContentView(Resource.Layout.MCOLTabbedDash);
            
            var adapter =
            new TabsPageAdapter(SupportFragmentManager, new McolStatsFragment(), new McolAlertsFragment());

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.mcolViewPager);
            viewPager.Adapter = adapter;

        }       
    }
}
