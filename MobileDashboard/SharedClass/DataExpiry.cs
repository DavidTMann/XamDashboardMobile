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
        public static DateTime currentTime = DateTime.UtcNow;

        //If expiry date is passed return true and make obj = null and show alert.
        public bool IsExpired(DateTime expiryDate)
        {
            //UTC is 1 hr behind
            if (expiryDate < DateTime.UtcNow.AddHours(1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}