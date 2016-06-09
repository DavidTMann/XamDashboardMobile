using Android.Support.V4.App;
using Android.OS;
using Android.Views;

namespace MobileDashboard
{
    class McolAlertsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolAlertsFrag, container, false);

            return rootView;
        }
    }
}