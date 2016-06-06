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
    [Activity(Label = "MenuActivity")]
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
        }
    }
}