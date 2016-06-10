using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace MobileDashboard
{
    class McolAlertsFragment : Fragment
    {
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

            return rootView;
        }
    }
}