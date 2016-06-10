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

namespace MobileDashboard
{
    class McolStatsFragment : Android.Support.V4.App.Fragment
    {
        DataExpiry da = new DataExpiry();
        public static bool expired = false;

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

            //Add expiry date to mcolStats
            //Currently adds 10 minutes, change to 1 hour after testing UTC are an hour behind
            mcolStats[0].ExpiryDate = DataExpiry.currentTime.AddMinutes(62);

            ListView mcolListView = rootView.FindViewById<ListView>(Resource.Id.mcolListView);

            //Checks expiry date
            CheckExpiryStatsData( mcolStats, mcolListView, rootView);

            mcolListView.Adapter = new McolStatsAdapter(this.Activity, mcolStats);

            return rootView;
        }

        //Run this method, if expired makes data null
        private void CheckExpiryStatsData(List<McolStats> mcolStats, ListView mcolListView, View rootView)
        {
            if (da.IsExpired(mcolStats[0].ExpiryDate))
            {
                expired = true;
                mcolStats = null;
                mcolListView.Visibility = ViewStates.Gone;

                TextView statsExpiredTxt = rootView.FindViewById<TextView>(Resource.Id.statsExpiredTxt);
                statsExpiredTxt.Visibility = ViewStates.Visible;
            }
        }

        private string GetMcolStatsJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/mcol/stats");
            request.ContentType = "application/json; charset=utf-8";
            request.Proxy = new WebProxy("proxy.logica.com", 80);

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
