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

namespace MobileDashboard
{
    class McolStatsFragment : Android.Support.V4.App.Fragment
    {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolStatsFrag, container, false);

            string json = GetMcolStatsJson();

            //Deserialize json and put it in list view
            var mcol = JsonConvert.DeserializeObject<List<McolStats>>(json);

            ListView mcolListView = rootView.FindViewById<ListView>(Resource.Id.mcolListView);
            mcolListView.Adapter = new McolStatsAdapter(this.Activity, mcol);
            
            return rootView;
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
