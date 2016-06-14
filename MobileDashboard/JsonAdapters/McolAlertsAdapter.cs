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
    public class McolAlertsAdapter : BaseAdapter<McolAlerts>
    {
        private readonly IList<McolAlerts> _items;
        private readonly Context _context;

        public McolAlertsAdapter(Context context, IList<McolAlerts> items)
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
                view = inflater.Inflate(Resource.Layout.MCOLAlertsRow, parent, false);
            }
            
            view.FindViewById<TextView>(Resource.Id.WhenAlerts).Text = Convert.ToDateTime(item.serverAlerts.when.Substring(item.serverAlerts.when.Length - 3)).ToString();                        

            view.FindViewById<TextView>(Resource.Id.ServAlerts).Text = item.serverAlerts.server;
            view.FindViewById<TextView>(Resource.Id.PriorityAlerts).Text = item.serverAlerts.priority;
            view.FindViewById<TextView>(Resource.Id.FileAlerts).Text = item.serverAlerts.file;
            view.FindViewById<TextView>(Resource.Id.TitleAlerts).Text = item.serverAlerts.title;

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

        public override McolAlerts this[int position]
        {
            get { return _items[position]; }
        }
    }

    public class ServerAlerts
    {
        public string received { get; set; }
        public string created { get; set; }
        public string server { get; set; }
        public string priority { get; set; }
        public string when { get; set; }
        public string file { get; set; }
        public string title { get; set; }
        public object detail { get; set; }
    }

    public class McolAlerts
    {
        public ServerAlerts serverAlerts { get; set; }
    }
}