using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using MobileDashboard.JsonAdapters;
using System.Collections.Generic;

namespace MobileDashboard
{
    class McolAlertsFragment : Fragment
    {
        public static bool Level3Notify = false;
        public static bool AlreadyNotified = false;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolAlertsFrag, container, false);
            
            Button menuBtn = rootView.FindViewById<Button>(Resource.Id.alertsBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent menu = new Intent(this.Activity, typeof(MenuActivity));
                StartActivity(menu);
            };

            string json = GetMcolServAlertsJson();

            var mcolAlerts = JsonConvert.DeserializeObject<List<McolAlerts>>(json);

            ListView mcolAlertsListView = rootView.FindViewById<ListView>(Resource.Id.mcolAlertsListView);
            
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            ExpireAlertsData(mcolAlerts, mcolAlertsListView);

            mcolAlertsListView.Adapter = new McolAlertsAdapter(this.Activity, mcolAlerts);
            
            return rootView;
        }

        //Run this method, if expired makes data null
        private void ExpireAlertsData(List<McolAlerts> mcolAlerts, ListView mcolAlertsListView)
        {
            if (MCOLTabbedDash.dataExpired)
            {
                mcolAlerts = null;
                mcolAlertsListView.Visibility = ViewStates.Gone;
            }
        }

        public string GetMcolServAlertsJson()
        {
            var request = (HttpWebRequest)WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/mcol/serveralerts");
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Set("x-auth", MainActivity.jwtToken);

            if (MainActivity._localProxy != null)
            { 
                request.Proxy = new WebProxy("proxy.logica.com", 80);
            }

            string json = "";

            //Get response code without exception
            var response = HttpWebResponseExt.GetResponseNoException(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();

                    //Praise the lord for this line below
                    json = JToken.Parse(json).ToString();

                    return json;
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                MCOLTabbedDash.dataExpired = true;
            }

            return json;
        }        
    }
}