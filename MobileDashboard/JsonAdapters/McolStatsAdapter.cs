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

namespace MobileDashboard.JsonAdapters
{    
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
            get
            {
                if (_items != null)
                {
                    return _items.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override McolStats this[int position]
        {
            get { return _items[position]; }
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
        public DateTime ExpiryDate { get; set; }
    }
}