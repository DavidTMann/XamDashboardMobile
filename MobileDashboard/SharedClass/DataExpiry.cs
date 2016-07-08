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

namespace MobileDashboard.SharedClass
{
    public class DataExpiry
    {
        //1 hr behind
        public static DateTime currentTime = DateTime.UtcNow.AddHours(1);
        public static bool dataExpired = false;
        public static DateTime expiryDate; //In milliseconds since 1970/01/01

        //If expiry date is dataExpired = true
        public void IsExpired(DateTime expiryDate)
        {            
            if (expiryDate < DateTime.UtcNow.AddHours(1))
            {
                dataExpired = true;
            }
            else
            {
                dataExpired = false;
            }
        }
    }
}