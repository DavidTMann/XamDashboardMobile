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

            //Grab username from MainActivity
            string userTxt = Intent.GetStringExtra("user") ?? "unknown";

            //User field
            TextView userLabel = FindViewById<TextView>(Resource.Id.userLabel);

            if (userTxt == "unknown")
            {
                userLabel.Text = "Welcome!";
            }
            else
            {
                userLabel.Text += userTxt + "!";
            }

            Button ragBtn = FindViewById<Button>(Resource.Id.gotoRagBtn);
            ragBtn.Click += delegate
            {
                //Go to rag application page                    
                Intent rag = new Intent(this.ApplicationContext, typeof(RAGActivity));
                rag.PutExtra("json", GetRagJson());
                StartActivity(rag);
            };

            Button mcolDashTabs = FindViewById<Button>(Resource.Id.mcolDashV2Btn);
            mcolDashTabs.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent mcolTabbedDash = new Intent(this.ApplicationContext, typeof(MCOLTabbedDash));
                StartActivity(mcolTabbedDash);
            };

            if (!McolAlertsFragment.AlreadyNotified)
            {
                //Where MCOL ALERTS Notifications are handled
                GetMcolAlerts();
                //Sends if alert lvl5
                SendLevel5Alert(userTxt);
            }           

        }

        private void GetMcolAlerts()
        {
            McolAlertsFragment alerts = new McolAlertsFragment();

            string json = alerts.GetMcolServAlertsJson();

            //Deserialize json and put it in list view
            var mcolAlerts = JsonConvert.DeserializeObject<List<McolAlerts>>(json);

            //Check to see if level 5 alerts in there
            foreach (var al in mcolAlerts)
            {
                if (al.serverAlerts.priority == "Level 5")
                {
                    McolAlertsFragment.Level5Notify = true;
                }
            }
        }

        private void SendLevel5Alert(string userTxt)
        {
            if (McolAlertsFragment.Level5Notify)
            {
                const int Level5NotificationId = 1000;

                //Create SMS intent and pass in username
                Intent smsIntent = new Intent(this, typeof(SMSActivity));
                smsIntent.PutExtra("user", userTxt);

                /// Construct a back stack for cross-task navigation:
                TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(SMSActivity)));
                stackBuilder.AddNextIntent(smsIntent);

                // Create the PendingIntent with the back stack:            
                PendingIntent resultPendingIntent =
                    stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

                //Big text style notification 
                string longMessage = "MCOL level 5 alerts are present. Click this notification to notify other team members.";

                Notification notif = new Notification.Builder(this)
                .SetContentTitle("Warning: Level 5 Alerts")
                .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                .SetContentIntent(resultPendingIntent)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetStyle(new Notification.BigTextStyle()
                    .BigText(longMessage))
                .Build();

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(Level5NotificationId, notif);

                //Once notified we can stop sending alert for this session
                McolAlertsFragment.AlreadyNotified = true;
            }
        }

        public string GetRagJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/rag");
            request.ContentType = "application/json; charset=utf-8";

            //Below needs to be commented out if i'm debugging on android device
            //request.Proxy = new WebProxy("proxy.logica.com", 80);

            string json;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();

                //Remove \ and quotes wrapped round json
                json = json.Replace(@"\", string.Empty);
                json = json.Substring(1, json.Length - 2);

                return json;
            }
        }
    }
}