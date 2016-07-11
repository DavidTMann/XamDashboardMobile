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
        public static bool dataExpired = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Create your application here
            SetContentView(Resource.Layout.MCOLTabbedDash);
            
            var adapter =
            new TabsPageAdapter(SupportFragmentManager, new McolSummaryFragment(), new McolStatsFragment(), new McolAlertsFragment());

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.mcolViewPager);
            viewPager.Adapter = adapter;
            
            //Checks if data is expired
            CheckIfDataIsExpired();
        }        

        public void CheckIfDataIsExpired()
        {
            int DataExpiredNotificationId = 1001;

            //Quick API call to test if data has expired, if 401 then expired
            McolStatsFragment stats = new McolStatsFragment();
            stats.GetMcolStatsJson();

            //If expired
            if (dataExpired)
            {
                //Alert dialog below
                //ShowAlert("Sorry, the data has expired. Please log back in.", false);

                //Introduce new DialogFragment
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialog_LogIn loginPrompt = new dialog_LogIn();
                loginPrompt.Cancelable = false;
                loginPrompt.Show(transaction, "dialog fragment");

                //Notification to say data has expired
                // Construct a back stack for cross-task navigation:
                Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);

                // Build the notification:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                    .SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
                    .SetContentTitle("Data has expired.")      // Set its title
                    .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                    .SetSmallIcon(Resource.Drawable.Icon)  //  this icon
                    .SetContentText("Data has expired, please log back in."); // The message to display.                  

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(DataExpiredNotificationId, builder.Build());
            }
        }

        public void ShowAlert(string str, bool yesAndNo)
        {
            if (yesAndNo)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle(str);
                alert.SetPositiveButton("Yes", (sender, args) =>
                {
                    //Go to RAG Score                   
                    Intent ragActivity = new Intent(this.ApplicationContext, typeof(RAGActivity));
                    StartActivity(ragActivity);
                });
                alert.SetNegativeButton("No", (sender, args) =>
                {
                    // User pressed no do nothing
                });

                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
            else if (!yesAndNo)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle(str);
                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    //Go back to log in page
                    Intent logInPage = new Intent(this.ApplicationContext, typeof(MainActivity));
                    StartActivity(logInPage);
                });
                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }            
        }
    }
}
