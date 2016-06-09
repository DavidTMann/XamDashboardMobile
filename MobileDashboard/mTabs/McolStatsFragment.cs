using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace MobileDashboard
{
    class McolStatsFragment : Fragment
    {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolStatsFrag, container, false);

            return rootView;
        }
    }
}
