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
        private static bool isFirstRun = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Create your application here
            SetContentView(Resource.Layout.MCOLTabbedDash);

            if (isFirstRun)
            {
                //Figure out how to run it only once            
                ShowAlert("Swipe right to view other MCOL data.");
                isFirstRun = false;
            }

            //Checks if data is expired
            CheckIfDataIsExpired();

            //Checks if level 5 alert 
            CheckIfLevel5Alert();

            var adapter =
            new TabsPageAdapter(SupportFragmentManager, new McolSummaryFragment(), new McolStatsFragment(), new McolAlertsFragment(), new McolBatchFragment());

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.mcolViewPager);
            viewPager.Adapter = adapter;

        }

        private void CheckIfLevel5Alert()
        {
            if (McolAlertsFragment.Level5Notify)
            {
                //Notification to say data has expired
                // Construct a back stack for cross-task navigation:
                Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);

                // Build the notification:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                    .SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
                    .SetContentTitle("Warning : Level 5 Alerts.")      // Set its title
                    .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                    .SetSmallIcon(Resource.Drawable.Icon)  //  this icon
                    .SetContentText("MCOL level 5 alerts are present. Click this notification to notify other team members."); // The message to display.                  

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(1, builder.Build());
            }
        }

        private void CheckIfDataIsExpired()
        {
            //If expired
            if (McolStatsFragment.expired)
            {
                ShowAlert("Sorry, the data has expired.");

                //Notification to say data has expired
                // Construct a back stack for cross-task navigation:
                Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);

                // Build the notification:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                    .SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
                    .SetContentTitle("Data has expired.")      // Set its title
                    .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                    .SetSmallIcon(Resource.Drawable.Icon)  //  this icon
                    .SetContentText("Data has expired and cannot be seen."); // The message to display.                  

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(1, builder.Build());
            }
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
