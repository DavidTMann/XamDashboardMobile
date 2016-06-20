using System.Collections.Generic;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using MobileDashboard.JsonAdapters;
using MobileDashboard.SharedClass;
using BarChart;
using Android.Support.V4.Widget;
using System.Net;
using System.IO;

namespace MobileDashboard
{   

    [Activity(Label = "RAG Stats")]
    public class RAGActivity : Activity
    {
        DataExpiry dt = new DataExpiry();
        List<RagJson> remaindingRagApps = new List<RagJson>();
        private bool addedRagApps = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RAG);

            //Grab username from MainActivity
            string json = GetRagJson();

            //Deserialize json and put it in list view
            
            var ragObj = JsonConvert.DeserializeObject<List<RagJson>>(json);
            
            //TOGGLE BUTTON
            ToggleButton ragTogglBtn = FindViewById<ToggleButton>(Resource.Id.ragAppsToggle);
            ragTogglBtn.Checked = false;

            //List view 
            var ragListView = FindViewById<ListView>(Resource.Id.listView);

            //By default only show mcol and darts
            RagOnlyShowMcolAndDarts(ragObj, ragListView);

            ragTogglBtn.Click += (o, e) => {
                // Perform action on clicks
                if (ragTogglBtn.Checked)
                {                                           
                    ragObj.AddRange(remaindingRagApps);
                    addedRagApps = true;                   

                    //Check to see if data has expired
                    dt.IsExpired(DataExpiry.expiryDate);
                    //Check to see if DataExpiry.dataExpired is true if so disable data  
                    CheckExpiryRagData(ragObj, ragListView);

                    RagJsonAdapter ragAdapter = new RagJsonAdapter(this, ragObj);

                    ragListView.Adapter = ragAdapter;
                }
                else if (!ragTogglBtn.Checked)
                {
                    RagOnlyShowMcolAndDarts(ragObj, ragListView);
                }
            };

            //Refresh
            //Swipe to refresh
            var refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresherRag);
            refresher.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

            refresher.Refresh += delegate
            {
                //Refresh list view
                RefreshListView(ragObj, ragListView);
            };
            
            if (!DataExpiry.dataExpired)
            {
                //ListView item clicking
                ragListView.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                {
                    string selectedFromList = ragListView.GetItemAtPosition(e.Position).ToString();

                    ShowAlert("Would you like to go to MCOL Dashboard?", true);
                };
            }

            //Back to menu
            Button menuBtn = FindViewById<Button>(Resource.Id.ragBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent menu = new Intent(this, typeof(MenuActivity));
                StartActivity(menu);
            };

        }

        private void RefreshListView(List<RagJson> ragObj, ListView ragListView)
        {
            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            CheckExpiryRagData(ragObj, ragListView);

            RagJsonAdapter ragAdapter = new RagJsonAdapter(this, ragObj);

            ragListView.Adapter = ragAdapter;
        }

        //Only keeps MCOL AND DARTS Apps, adds rest of apps to remaining rag apps
        private void RagOnlyShowMcolAndDarts(List<RagJson> ragObj, ListView ragListView)
        {
            //remove all apps apart from mcol and darts
            foreach (var r in ragObj.ToArray())
            {
                if (r.Name == "MCOL" || r.Name == "DARTS")
                {
                    continue;
                }

                if (!r.Name.Contains("MCOL") || (!r.Name.Contains("DARTS")))
                {
                    ragObj.Remove(r);
                    
                    if (!addedRagApps)
                    {
                        remaindingRagApps.Add(r);
                    }                                 
                }
            }            

            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            CheckExpiryRagData(ragObj, ragListView);

            RagJsonAdapter ragAdapter = new RagJsonAdapter(this, ragObj);

            ragListView.Adapter = ragAdapter;
        }

        //Run this method, if expired makes data null
        private void CheckExpiryRagData(List<RagJson> ragObj, ListView ragListView)
        {
            if (DataExpiry.dataExpired)
            {
                ragObj = null;
                ragListView.Visibility = ViewStates.Gone;

                ShowAlert("Sorry, the data has expired.", false);
            }
        }

        public string GetRagJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/rag");
            request.ContentType = "application/json; charset=utf-8";

            //Below needs to be commented out if i'm debugging on android device
            request.Proxy = new WebProxy("proxy.logica.com", 80);

            string json;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();

                //Remove \ and quotes wrapped round json
                json = json.Replace(@"\", string.Empty);
                json = json.Substring(1, json.Length - 2);

                return json;
            }
        }

        private void ShowAlert(string str, bool yesAndNo)
        {
            if (yesAndNo)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle(str);
                alert.SetPositiveButton("Yes", (sender, args) =>
                {
                    //Go to mcol dash page                    
                    Intent mcolDash = new Intent(this.ApplicationContext, typeof(MCOLTabbedDash));
                    StartActivity(mcolDash);
                });
                alert.SetNegativeButton("No", (sender, args) =>
                {
                    // User pressed no do nothing
                });

                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
            else if (!yesAndNo)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle(str);
                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    // write your own set of instructions
                });
                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
            
        }        
    }
}

