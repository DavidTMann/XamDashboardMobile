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
using MobileDashboard.SharedClass;

namespace MobileDashboard
{
    class McolAlertsFragment : Fragment
    {
        public static bool Level5Notify = false;
        public static bool AlreadyNotified = false;
        DataExpiry dt = new DataExpiry();

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

            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            ExpireAlertsData(mcolAlerts, mcolAlertsListView);

            mcolAlertsListView.Adapter = new McolAlertsAdapter(this.Activity, mcolAlerts);
            
            return rootView;
        }

        //Run this method, if expired makes data null
        private void ExpireAlertsData(List<McolAlerts> mcolAlerts, ListView mcolAlertsListView)
        {
            if (DataExpiry.dataExpired)
            {
                mcolAlerts = null;
                mcolAlertsListView.Visibility = ViewStates.Gone;
            }
        }

        public string GetMcolServAlertsJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/mcol/serveralerts");
            request.ContentType = "application/json; charset=utf-8";

            //Comment out if debugging on android device
            request.Proxy = new WebProxy("proxy.logica.com", 80);

            string json;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();

                //Remove \ and quotes wrapped round json
                json = json.Replace(@"\", string.Empty);
                json = json.Substring(1, json.Length - 2);
                                
                //Nasty hack due to escape characters in json..
                json = json.Replace("\".\"", string.Empty);
                json = json.Replace("oc4j/mcol-web-services/WEBs/processRequest.avg", string.Empty);
                json = json.Replace("/", string.Empty);
                // / oc4j / mcol - web - services / WEBs / processRequest.avg "." workaround

                return json;
            }
        }        
    }
}