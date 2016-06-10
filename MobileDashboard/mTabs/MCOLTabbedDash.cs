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
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Android.Preferences;

namespace MobileDashboard
{
    [Activity(Label = "MCOL Dashboard")]
    public class MCOLTabbedDash : FragmentActivity
    {
        private bool isFirstRun = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Create your application here
            SetContentView(Resource.Layout.MCOLTabbedDash);

            if (isFirstRun)
            {
                //Figure out how to run it only once            
                //ShowAlert("Swipe right to view other MCOL data.");                
            }

            isFirstRun = false;

            //If expired
            if (McolStatsFragment.expired)
            {
                ShowAlert("Sorry, the data has expired.");
            }

            var adapter =
            new TabsPageAdapter(SupportFragmentManager, new McolSummaryFragment(), new McolStatsFragment(), new McolAlertsFragment(), new McolBatchFragment());

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.mcolViewPager);
            viewPager.Adapter = adapter;
        }       

        public void ShowAlert(string str)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(str);
            alert.SetPositiveButton("OK", (senderAlert, args) => {
                // write your own set of instructions
            });            
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }
    }
}
