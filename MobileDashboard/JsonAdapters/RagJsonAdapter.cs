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
using Android.Graphics;

namespace MobileDashboard.JsonAdapters
{
    public class RagJsonAdapter : BaseAdapter<RagJson>
    {
        private readonly IList<RagJson> _items;
        private readonly Context _context;

        public RagJsonAdapter(Context context, IList<RagJson> items)
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
                view = inflater.Inflate(Resource.Layout.RAGrow, parent, false);
            }

            TextView appName = view.FindViewById<TextView>(Resource.Id.AppName);
            TextView rating = view.FindViewById<TextView>(Resource.Id.Rating);
            TextView buName = view.FindViewById<TextView>(Resource.Id.BuName);

            appName.Text = item.Name;
            rating.Text = item.RAGStatus.ToString();
            buName.Text = item.BusinessUnit;

            //Set text color black
            appName.SetTextColor(Color.Black);
            rating.SetTextColor(Color.Black);
            buName.SetTextColor(Color.Black);

            if (Convert.ToInt32(rating.Text) <= 20)
            {
                appName.SetBackgroundColor(Color.Black);
                rating.SetBackgroundColor(Color.Black);
                buName.SetBackgroundColor(Color.Black);
                //Set to white if blk background color
                appName.SetTextColor(Color.White);
                rating.SetTextColor(Color.White);
                buName.SetTextColor(Color.White);
            }
            if (Convert.ToInt32(rating.Text) <= 40 && Convert.ToInt32(rating.Text) > 20)
            {
                appName.SetBackgroundColor(Color.Red);
                rating.SetBackgroundColor(Color.Red);
                buName.SetBackgroundColor(Color.Red);
            }
            if (Convert.ToInt32(rating.Text) <= 85 && Convert.ToInt32(rating.Text) > 40)
            {
                appName.SetBackgroundColor(Color.Yellow);
                rating.SetBackgroundColor(Color.Yellow);
                buName.SetBackgroundColor(Color.Yellow);
            }
            if (Convert.ToInt32(rating.Text) == 100)
            {
                appName.SetBackgroundColor(Color.Green);
                rating.SetBackgroundColor(Color.Green);
                buName.SetBackgroundColor(Color.Green);
            }

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

        public override RagJson this[int position]
        {
            get { return _items[position]; }
        }
    }

    public class RagJson
    {
        public string Name { get; set; }
        public int RAGStatus { get; set; }
        public string RAGColour { get; set; }
        public string BusinessUnit { get; set; }
        public string Business { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
