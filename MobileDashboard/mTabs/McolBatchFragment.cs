using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace MobileDashboard.mTabs
{
    class McolBatchFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolBatchFrag, container, false);

            Button menuBtn = rootView.FindViewById<Button>(Resource.Id.batchBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent menu = new Intent(this.Activity, typeof(MenuActivity));
                StartActivity(menu);
            };

            return rootView;
        }
    }
}