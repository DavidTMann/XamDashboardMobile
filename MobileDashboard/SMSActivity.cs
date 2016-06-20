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

namespace MobileDashboard
{
    [Activity(Label = "Menu")]
    public class SMSActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //SMS intent to go to messaging app, mimic menu, nasty hack
            SetContentView(Resource.Layout.Menu);

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
                
                StartActivity(rag);
            };

            Button mcolDashTabs = FindViewById<Button>(Resource.Id.mcolDashV2Btn);
            mcolDashTabs.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent mcolTabbedDash = new Intent(this.ApplicationContext, typeof(MCOLTabbedDash));
                StartActivity(mcolTabbedDash);
            };

            var smsUri = Android.Net.Uri.Parse("smsto:07468415831, 07468708238");
            var smsIntent = new Intent(Intent.ActionSendto, smsUri);
            smsIntent.PutExtra("sms_body", "MCOL Level 5 Alert");
            StartActivity(smsIntent);

        }
    }
}