using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
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
using Android.Graphics;

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
            
            var adapter =
            new TabsPageAdapter(SupportFragmentManager, new McolSummaryFragment(), new McolStatsFragment(), new McolAlertsFragment(), new McolBatchFragment());

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.mcolViewPager);
            viewPager.Adapter = adapter;


            //Checks if data is expired
            CheckIfDataIsExpired();
            
        }

        public void SendLevel5Alert()
        {  
            if (McolAlertsFragment.Level5Notify)
            {
                // Setup an intent for SecondActivity:
                Intent secondIntent = new Intent(this, typeof(SMSActivity));

                // Pass some information to SecondActivity:
                secondIntent.PutExtra("message", "Greetings from MainActivity!");

                // Create a task stack builder to manage the back stack:
                TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);

                // Add all parents of SecondActivity to the stack: 
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(SMSActivity)));

                // Push the intent that starts SecondActivity onto the stack:
                stackBuilder.AddNextIntent(secondIntent);

                // Obtain the PendingIntent for launching the task constructed by
                // stackbuilder. The pending intent can be used only once (one shot):
                const int pendingIntentId = 0;
                PendingIntent pendingIntent =
                stackBuilder.GetPendingIntent(pendingIntentId, 1073741824);

                // Instantiate the builder and set notification elements, including 
                // the pending intent:
                Notification.Builder builder = new Notification.Builder(this)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("Sample Notification")
                .SetContentText("Hello World! This is my second action notification!")
                .SetSmallIcon(Resource.Drawable.Icon);

                // Build the notification:
                Notification notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }
        }

        private void CheckIfDataIsExpired()
        {
            int DataExpiredNotificationId = 1001;
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
                notificationManager.Notify(DataExpiredNotificationId, builder.Build());
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
