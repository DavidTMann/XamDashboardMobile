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
using System.Net;
using System.IO;
using Newtonsoft.Json;
using MobileDashboard.JsonAdapters;

namespace MobileDashboard
{
    [Activity(Label = "Menu")]
    public class MenuActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {          
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Menu);

            //User field
            TextView userLabel = FindViewById<TextView>(Resource.Id.userLabel);

            if (MainActivity.userName == null)
            {
                userLabel.Text = "Welcome!";
            }
            else
            {
                userLabel.Text += MainActivity.userName + "!";
            }

            //Log out button
            Button logOutBtn = FindViewById<Button>(Resource.Id.LogOutBtn);
            logOutBtn.Click += delegate
            {
                //Go to log in page                    
                Intent logIn = new Intent(this.ApplicationContext, typeof(MainActivity));

                StartActivity(logIn);
            };

            //Menu buttons
            Button ragBtn = FindViewById<Button>(Resource.Id.gotoRagBtn);
            ragBtn.Click += delegate
            {
                //Go to rag application page                    
                Intent rag = new Intent(this.ApplicationContext, typeof(RAGActivity));
                
                StartActivity(rag);
            };

            Button mcolDashTabs = FindViewById<Button>(Resource.Id.mcolDashV2Btn);
            mcolDashTabs.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent mcolTabbedDash = new Intent(this.ApplicationContext, typeof(MCOLTabbedDash));
                StartActivity(mcolTabbedDash);
            };

            Button contactsBtn = FindViewById<Button>(Resource.Id.ContactsBtn);
            contactsBtn.Click += delegate
            {
                //Go to contacts page                    
                Intent contacts = new Intent(this.ApplicationContext, typeof(ContactActivity));
                StartActivity(contacts);
            };
            
            if (!McolAlertsFragment.AlreadyNotified)
            {
                //Where MCOL ALERTS Notifications are handled
                GetMcolAlerts();
                //Sends if alert lvl5
                SendLevel3Alert();
            }
        }

        private void GetMcolAlerts()
        {
            McolAlertsFragment alerts = new McolAlertsFragment();

            string json = alerts.GetMcolServAlertsJson();

            //Deserialize json and put it in list view
            var mcolAlerts = JsonConvert.DeserializeObject<List<McolAlerts>>(json);

            //THIS ADDS EXPIRY DATE TO MCOL ALERT; THIS DECIDES WHEN ALL DATA WILL EXPIRE.
            //UTC TIME IS 1 HR BEHIND, CURRENTLY EXPIRES AFTER 5 MINUTES
            //DataExpiry.expiryDate = DataExpiry.currentTime.AddMinutes(5);

            //Check to see if level 5 alerts in there
            foreach (var al in mcolAlerts)
            {
                if (al.serverAlerts.priority == "Level 3")
                {
                    McolAlertsFragment.Level3Notify = true;
                }
            }
        }

        private void SendLevel3Alert()
        {
            if (McolAlertsFragment.Level3Notify)
            {
                const int Level3NotificationId = 1000;

                //Create SMS intent and pass in username
                Intent smsIntent = new Intent(this, typeof(SMSActivity));

                /// Construct a back stack for cross-task navigation:
                TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(SMSActivity)));
                stackBuilder.AddNextIntent(smsIntent);

                // Create the PendingIntent with the back stack:            
                PendingIntent resultPendingIntent =
                    stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

                //Big text style notification 
                string longMessage = "MCOL level 3 alerts are present. Click this notification to notify other team members.";

                Notification notif = new Notification.Builder(this)
                .SetContentTitle("Warning: Level 3 Alerts")
                .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                .SetContentIntent(resultPendingIntent)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetStyle(new Notification.BigTextStyle()
                    .BigText(longMessage))
                .Build();

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(Level3NotificationId, notif);

                //Once notified we can stop sending alert for this session
                McolAlertsFragment.AlreadyNotified = true;
            }
        }        
    }
}