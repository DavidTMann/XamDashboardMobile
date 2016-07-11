using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Http;
using ModernHttpClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MobileDashboard
{
    [Activity(Label = "Dashboard Mobile", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static string userName;
        public static string jwtToken;
        private static string secretKey = "postsecret";

        public static WebProxy _localProxy = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button signInBtn = FindViewById<Button>(Resource.Id.SignInBtn);

            //username field
            TextView user = FindViewById<TextView>(Resource.Id.userNameTxt);
            //user.SetSingleLine();
            //pwd field
            TextView pwd = FindViewById<TextView>(Resource.Id.passwordText);
            //user.SetSingleLine();

            //incorrect credentials txt box
            var incorrectCredTxt = FindViewById<TextView>(Resource.Id.incorrectPwdText);

            signInBtn.Click += delegate
            {
                ValidateUserLogin(user, pwd, incorrectCredTxt, false);
            };
        }

        public override void OnBackPressed()
        {
            //Closes app instead of returning back to menu
            //base.OnBackPressed();
            this.FinishAffinity();
        }

        public bool ValidateUserLogin(TextView user, TextView pwd, TextView incorrectCredTxt, bool prompt)
        {
            var uri = new Uri("https://www.warren-ayling.me.uk:8443/api/user/login");

            string json = string.Format("{{\"username\":\"{0}\"," +
                              "\"passwd\":\"{1}\"}}", user.Text.Trim(), pwd.Text.Trim());

            //Comment out if NOT using groupinfra network
            //_localProxy = new WebProxy("proxy.logica.com", 80);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            //Below needs to be commented out if i'm debugging on android device
            if (MainActivity._localProxy != null)
            {
                httpWebRequest.Proxy = new WebProxy("proxy.logica.com", 80);
            }

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //Get response code
            var httpResponse = HttpWebResponseExt.GetResponseNoException(httpWebRequest);
            var result = "";

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            //if credentials are ok then go to menu
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                //Store jwtToken
                jwtToken = result;

                //Reset data expiry
                MCOLTabbedDash.dataExpired = false;

                //Secret key as bytes
                byte[] secKeyBytes = Encoding.UTF8.GetBytes(secretKey);

                if (!prompt)
                {
                    //Go to menu dashboard page                    
                    Intent menu = new Intent(this.ApplicationContext, typeof(MenuActivity));
                    userName = user.Text.Trim();

                    StartActivity(menu);
                }

                return true;                
            }
            else
            {
                incorrectCredTxt.Visibility = ViewStates.Visible;

                return false;
            }
        }
    }

    public static class HttpWebResponseExt
    {
        public static HttpWebResponse GetResponseNoException(this HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
    }
}

