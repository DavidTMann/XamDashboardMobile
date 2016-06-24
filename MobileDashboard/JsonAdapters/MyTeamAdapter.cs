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
using System.IO;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace MobileDashboard.JsonAdapters
{
    class MyTeamAdapter : BaseAdapter<MyTeam>
    {
        private readonly IList<MyTeam> _items;
        private readonly Context _context;

        public MyTeamAdapter(Context context, IList<MyTeam> items)
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
                view = inflater.Inflate(Resource.Layout.ContactTeamRow, parent, false);
            }

            ImageView teamPicture = view.FindViewById<ImageView>(Resource.Id.myTeamImg);
            TextView teamName = view.FindViewById<TextView>(Resource.Id.myTeamName);
            Button teamEmail = view.FindViewById<Button>(Resource.Id.myTeamEmail);
            Button teamSms = view.FindViewById<Button>(Resource.Id.myTeamSms);

            //Remove data:image/jpeg;base64, from base64string
            item.Picture = item.Picture.Replace("data:image/jpeg;base64,", string.Empty);
            //Code to decode base64 string into image
            System.IO.Stream s = new MemoryStream(Convert.FromBase64String(item.Picture));

            byte[] arr = Convert.FromBase64String(item.Picture);
            Drawable img = Drawable.CreateFromStream(s, null);

            teamPicture.SetImageDrawable(img);
            
            //Assign team name to textview
            teamName.Text = item.Name;

            //Buttons for email
            teamEmail.Click += delegate
            {
                //Qas's email
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { item.Email });

                email.PutExtra(Android.Content.Intent.ExtraSubject, "Dashboard Mobile - ");
                email.PutExtra(Android.Content.Intent.ExtraText, "Hello, " + System.Environment.NewLine);
                email.SetType("message/rfc822");
                _context.StartActivity(email);
            };

            //SMS
            teamSms.Click += delegate
            {
                //Qas's number
                var smsUri = Android.Net.Uri.Parse(string.Format("smsto:{0}", item.Number));
                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.PutExtra("sms_body", "Hello, ");
                _context.StartActivity(smsIntent);
            };

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

        public override MyTeam this[int position]
        {
            get { return _items[position]; }
        }
    }

    public class MyTeam
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public List<string> Teams { get; set; }
        public string Picture { get; set; }
    }
}
