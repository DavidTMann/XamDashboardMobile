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
using System.Net;

namespace MobileDashboard.PushNotifications
{
    public class AndroidGCMPushNotification
    {
        public string SendNotification(string deviceId, string message)
        {
            string GoogleAppID = "google application id";
            var SENDER_ID = "9999999999";
            var value = message;
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + 
            System.DateTime.Now.ToString() + "�istration_id=" + deviceId + "";
            Console.WriteLine(postData);
            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();

            dataStream = tResponse.GetResponseStream();

            StreamReader tReader = new StreamReader(dataStream);

            String sResponseFromServer = tReader.ReadToEnd();

            tReader.Close();
            dataStream.Close();
            tResponse.Close();
            return sResponseFromServer;
        }
    }
}