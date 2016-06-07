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

            Button ragBtn = FindViewById<Button>(Resource.Id.gotoRagBtn);
            ragBtn.Click += delegate
            {
                //Go to rag application page                    
                Intent rag = new Intent(this.ApplicationContext, typeof(RAGActivity));
                rag.PutExtra("json", GetRagJson());
                StartActivity(rag);
            };          
        }

        private string GetRagJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/rag");
            request.ContentType = "application/json; charset=utf-8";

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