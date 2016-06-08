using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
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
    [Activity(Label = "MCOL Dash")]
    public class MCOLDash : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MCOLDash);
            
            string json = GetMcolStatsJson();
            
            //Deserialize json and put it in list view
            if (json != null)
            {
                var mcol = JsonConvert.DeserializeObject<List<McolStats>>(json);

                var mcolListView = FindViewById<ListView>(Resource.Id.mcolListView);
                mcolListView.Adapter = new McolStatsAdapter(this, mcol);

                Button mcolStatsBtn = FindViewById<Button>(Resource.Id.McolServerStatsBtn);
                mcolStatsBtn.Click += delegate
                {
                    //Fill list view with server stats, activated by default
                    mcolListView.Adapter = new McolStatsAdapter(this, mcol);
                };

            }
            else
            {
                ShowAlert("No JSON received cannot display data.");
            }           

        }

        private string GetMcolStatsJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/mcol/stats");
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

        private void ShowAlert(string str)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(str);
            alert.SetPositiveButton("OK", (senderAlert, args) => {
                // write your own set of instructions
            });

            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }

        public class Stat
        {
            public string received { get; set; }
            public string created { get; set; }
            public string server { get; set; }
            public string mem { get; set; }
            public string uptime { get; set; }
            public string avgRequestTime { get; set; }
            public int activeSessions { get; set; }
        }

        public class McolStats
        {
            public Stat stat { get; set; }
        }
        /// <summary>
        /// Custom adapter for MCOL Server Stats
        /// </summary>
        public class McolStatsAdapter : BaseAdapter<McolStats>
        {
            private readonly IList<McolStats> _items;
            private readonly Context _context;

            public McolStatsAdapter(Context context, IList<McolStats> items)
            {
                _items = items;
                _context = context;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var item = _items[position];
                var view = convertView;

                if (view == null)
                {
                    var inflater = LayoutInflater.FromContext(_context);
                    view = inflater.Inflate(Resource.Layout.MCOLStatsrow, parent, false);
                }

                //Convert created to date time format                
                view.FindViewById<TextView>(Resource.Id.DateTime).Text = Convert.ToDateTime(item.stat.created).ToString();

                view.FindViewById<TextView>(Resource.Id.Server).Text = item.stat.server;
                view.FindViewById<TextView>(Resource.Id.Memory).Text = item.stat.mem;
                view.FindViewById<TextView>(Resource.Id.Uptime).Text = item.stat.uptime;
                view.FindViewById<TextView>(Resource.Id.AvgReqTime).Text = item.stat.avgRequestTime;
                view.FindViewById<TextView>(Resource.Id.ActiSessions).Text = item.stat.activeSessions.ToString();
                
                return view;
            }

            public override int Count
            {
                get { return _items.Count; }
            }

            public override McolStats this[int position]
            {
                get { return _items[position]; }
            }
        }
    }
}
