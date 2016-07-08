using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Content;
using Android.App;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using MobileDashboard.JsonAdapters;
using MobileDashboard.SharedClass;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace MobileDashboard
{
    class McolStatsFragment : Android.Support.V4.App.Fragment
    {
        DataExpiry dt = new DataExpiry();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolStatsFrag, container, false);

            string json = GetMcolStatsJson();

            Button menuBtn = rootView.FindViewById<Button>(Resource.Id.statsBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent menu = new Intent(this.Activity, typeof(MenuActivity));
                StartActivity(menu);
            };

            //Deserialize json and put it in list view
            var mcolStats = JsonConvert.DeserializeObject<List<McolStats>>(json);
                        
            ListView mcolListView = rootView.FindViewById<ListView>(Resource.Id.mcolListView);
            
            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            ExpireStatsData(mcolStats, mcolListView);

            mcolListView.Adapter = new McolStatsAdapter(this.Activity, mcolStats);

            return rootView;
        }

        //Run this method, if expired makes data null
        private void ExpireStatsData(List<McolStats> mcolObj, ListView mcolListView)
        {
            if (DataExpiry.dataExpired)
            {
                mcolObj = null;
                mcolListView.Visibility = ViewStates.Gone;                
            }
        }

        private string GetMcolStatsJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/mcol/stats");
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Set("x-auth", MainActivity.jwtToken);

            //Comment out if debugging on android device
            if (MainActivity._localProxy != null)
            {
                request.Proxy = new WebProxy("proxy.logica.com", 80);
            }

            string json;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();

                //Praise the lord for this line below
                json = JToken.Parse(json).ToString();

                ////Remove \ and quotes wrapped round json
                //json = json.Replace(@"\", string.Empty);
                //json = json.Substring(1, json.Length - 2);

                return json;
            }
        }        
    }
}
