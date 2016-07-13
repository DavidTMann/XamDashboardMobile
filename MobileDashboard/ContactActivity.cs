using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using MobileDashboard.JsonAdapters;

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

            //Get myteam json
            string json = GetMyTeamJson();
            //Deserialize json into c# object MyTeam
            var myTeamObj = JsonConvert.DeserializeObject<List<MyTeam>>(json);
            
            //MyTeam listview
            var myTeamListView = FindViewById<ListView>(Resource.Id.myTeamListView);

            //Make custom adapter for my team list view
            MyTeamAdapter teamAdapter = new MyTeamAdapter(this, myTeamObj);
            myTeamListView.Adapter = teamAdapter;
                                   

            //Back to menu button
            Button menuBtn = FindViewById<Button>(Resource.Id.summBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to menu                   
                Intent menu = new Intent(this, typeof(MenuActivity));
                StartActivity(menu);
            };
        }

        public string GetMyTeamJson()
        {
            var request = (HttpWebRequest)WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/user/myteam");
            request.ContentType = "application/json";
            request.Headers.Add(string.Format("x-auth: {0}", MainActivity.jwtToken));

            //Below needs to be commented out if i'm debugging on android device
            if (MainActivity._localProxy != null)
            {
                request.Proxy = new WebProxy("proxy.logica.com", 80);
            }

            string json = "";

            //Get response code
            var response = HttpWebResponseExt.GetResponseNoException(request);

            //Check response if OK read json and return, if 401 then set data expired to true

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();

                ////Remove \ and quotes wrapped round json
                //json = json.Replace(@"\", string.Empty);
                //json = json.Substring(1, json.Length - 2);

                return json;
            }
        }
    }
}