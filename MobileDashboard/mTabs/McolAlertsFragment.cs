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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolAlertsFrag, container, false);

            string json = GetMcolServAlertsJson();

            Button menuBtn = rootView.FindViewById<Button>(Resource.Id.alertsBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent menu = new Intent(this.Activity, typeof(MenuActivity));
                StartActivity(menu);
            };

            //Deserialize json and put it in list view
            var mcolAlerts = JsonConvert.DeserializeObject<List<McolAlerts>>(json);

            ListView mcolAlertsListView = rootView.FindViewById<ListView>(Resource.Id.mcolAlertsListView);

            mcolAlertsListView.Adapter = new McolAlertsAdapter(this.Activity, mcolAlerts);

            return rootView;
        }

        private string GetMcolServAlertsJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/mcol/serveralerts");
            request.ContentType = "application/json; charset=utf-8";

            //Comment out if debugging on android device
            //request.Proxy = new WebProxy("proxy.logica.com", 80);

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