using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using MobileDashboard.mTabs;
using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MobileDashboard
{
    [Activity(Label = "MCOL Dashboard")]
    public class MCOLTabbedDash : FragmentActivity
    {
        private bool swipeRight = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Create your application here
            SetContentView(Resource.Layout.MCOLTabbedDash);

            var adapter =
            new TabsPageAdapter(SupportFragmentManager, new McolSummaryFragment(), new McolStatsFragment(), new McolAlertsFragment(), new McolBatchFragment());

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.mcolViewPager);
            viewPager.Adapter = adapter;

            if (swipeRight)
            {
                ShowAlert("Swipe right to view other MCOL data.");
                swipeRight = false;
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
