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
    [Activity(Label = "Service Contacts")]
    public class ContactActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Contact);

            //Default contact img's
            var defImg1 = FindViewById<ImageView>(Resource.Id.DefaultImg1);
            defImg1.SetImageResource(Resource.Drawable.DefaultContact1);

            var defImg2 = FindViewById<ImageView>(Resource.Id.DefaultImg2);
            defImg2.SetImageResource(Resource.Drawable.DefaultContact2);

            var defImg3 = FindViewById<ImageView>(Resource.Id.DefaultImg3);
            defImg3.SetImageResource(Resource.Drawable.DefaultContact3);

            //SMs & Email buttons contact 1 QAS
            //SMS
            var smsBtn1 = FindViewById<Button>(Resource.Id.contactSmsBtn1);
            smsBtn1.Click += delegate
            {
                //Qas's number
                var smsUri = Android.Net.Uri.Parse("smsto:07468708232");
                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.PutExtra("sms_body", "Hello, ");
                StartActivity(smsIntent);
            };

            //EMAIL
            var emailBtn1 = FindViewById<Button>(Resource.Id.contactEmailBtn1);
            emailBtn1.Click += delegate
            {
                //Qas's email
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "q.malik@cgi.com" });

                email.PutExtra(Android.Content.Intent.ExtraSubject, "Dashboard Mobile - ");
                email.PutExtra(Android.Content.Intent.ExtraText, "Hello, " + System.Environment.NewLine);
                email.SetType("message/rfc822");
                StartActivity(email);
            };

            //SMs & Email buttons contact 2 DAVE
            //SMS
            var smsBtn2 = FindViewById<Button>(Resource.Id.contactSmsBtn2);
            smsBtn2.Click += delegate
            {
                //Qas's number
                var smsUri = Android.Net.Uri.Parse("smsto:07468708238");
                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.PutExtra("sms_body", "Hello, ");
                StartActivity(smsIntent);
            };

            //EMAIL
            var emailBtn2 = FindViewById<Button>(Resource.Id.contactEmailBtn2);
            emailBtn2.Click += delegate
            {
                //Qas's email
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "d.mann@cgi.com" });

                email.PutExtra(Android.Content.Intent.ExtraSubject, "Dashboard Mobile - ");
                email.PutExtra(Android.Content.Intent.ExtraText, "Hello, " + System.Environment.NewLine);
                email.SetType("message/rfc822");
                StartActivity(email);
            };

            //SMs & Email buttons contact 3 WARREN
            //SMS
            var smsBtn3 = FindViewById<Button>(Resource.Id.contactSmsBtn3);
            smsBtn3.Click += delegate
            {
                //Qas's number
                var smsUri = Android.Net.Uri.Parse("smsto:07876257970");
                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.PutExtra("sms_body", "Hello, ");
                StartActivity(smsIntent);
            };

            //EMAIL
            var emailBtn3 = FindViewById<Button>(Resource.Id.contactEmailBtn3);
            emailBtn3.Click += delegate
            {
                //Qas's email
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "warren.ayling@cgi.com" });

                email.PutExtra(Android.Content.Intent.ExtraSubject, "Dashboard Mobile - ");
                email.PutExtra(Android.Content.Intent.ExtraText, "Hello, " + System.Environment.NewLine);
                email.SetType("message/rfc822");
                StartActivity(email);
            };

            //Back to menu button
            Button menuBtn = FindViewById<Button>(Resource.Id.summBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to menu                   
                Intent menu = new Intent(this, typeof(MenuActivity));
                StartActivity(menu);
            };
        }
    }
}