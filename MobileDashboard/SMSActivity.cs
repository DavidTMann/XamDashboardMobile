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
            
            //SMS intent to go to messaging app, mimic menu, nasty hack
            SetContentView(Resource.Layout.Menu);

            //Grab username from MainActivity
            string userTxt = Intent.GetStringExtra("user") ?? "unknown";

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

            var smsUri = Android.Net.Uri.Parse("smsto:07468415831, 07468708232, 07876257970");
            var smsIntent = new Intent(Intent.ActionSendto, smsUri);
            smsIntent.PutExtra("sms_body", "MCOL Level 3 Alert");
            StartActivity(smsIntent);

        }
    }
}