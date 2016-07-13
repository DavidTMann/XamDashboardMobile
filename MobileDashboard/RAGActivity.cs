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
using BarChart;
using Android.Support.V4.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MobileDashboard
{   

    [Activity(Label = "RAG Stats")]
    public class RAGActivity : Activity
    {
        List<RagJson> remaindingRagApps = new List<RagJson>();
        private bool addedRagApps = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RAG);
            
            //Deserialize json and put it in list view
            string json = GetRagJson();

            var ragObj = JsonConvert.DeserializeObject<List<RagJson>>(json);
            
            //TOGGLE BUTTON
            ToggleButton ragTogglBtn = FindViewById<ToggleButton>(Resource.Id.ragAppsToggle);
            ragTogglBtn.Checked = false;

            //List view 
            var ragListView = FindViewById<ListView>(Resource.Id.listView);

            //Get MyTeam json to show by default accounts owned app's e.g. DAVE is MCOL, DARTS.
            ContactActivity con = new ContactActivity();
            string myTeamJson = con.GetMyTeamJson();

            //Deserialize json into c# object MyTeam
            var myTeamObj = JsonConvert.DeserializeObject<List<MyTeam>>(myTeamJson);

            //Get the allocated apps from team member
            List<string> defaultApps = new List<string>();

            //Hardcoded hack to get defaultApps
            foreach (var obj in myTeamObj)
            {
                if (MainActivity.userName == "aylingw" && obj.Name == "Warren Ayling")
                {
                    defaultApps = obj.Teams;
                }

                if (MainActivity.userName == "mannd" && obj.Name == "Dave Mann")
                {
                    defaultApps = obj.Teams;
                }

                if (MainActivity.userName == "malikq" && obj.Name == "Qas Malik")
                {
                    defaultApps = obj.Teams;
                }
            }

            //By default only show mcol and darts
            RagOnlyShowDefaultApps(ragObj, ragListView, defaultApps);

            ragTogglBtn.Click += (o, e) => {
                // Perform action on clicks
                if (ragTogglBtn.Checked)
                {                                           
                    ragObj.AddRange(remaindingRagApps);
                    addedRagApps = true;
                    
                    //Check to see if DataExpiry.dataExpired is true if so disable data  
                    CheckExpiryRagData(ragObj, ragListView);

                    RagJsonAdapter ragAdapter = new RagJsonAdapter(this, ragObj);

                    ragListView.Adapter = ragAdapter;
                }
                else if (!ragTogglBtn.Checked)
                {
                    RagOnlyShowDefaultApps(ragObj, ragListView, defaultApps);
                }
            };

            //Refresh
            //Swipe to refresh
            var refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresherRag);
            refresher.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

            refresher.Refresh += delegate
            {
                //Refresh list view
                this.Recreate();
            };
            
            if (!MCOLTabbedDash.dataExpired)
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

        //Only keeps logged in acc's default Apps, adds rest of apps to remaining rag apps
        private void RagOnlyShowDefaultApps(List<RagJson> ragObj, ListView ragListView, List<string> defaultApps)
        {

            string defaultApp1 = defaultApps[0];
            string defaultApp2 = defaultApps[1];

            //remove all apps apart from mcol and darts
            foreach (var r in ragObj.ToArray())
            {
                if (r.Name == defaultApp1 || r.Name == defaultApp2)
                {
                    continue;
                }

                if (!r.Name.Contains(defaultApp1) || (!r.Name.Contains(defaultApp2)))
                {
                    ragObj.Remove(r);
                    
                    if (!addedRagApps)
                    {
                        remaindingRagApps.Add(r);
                    }                                 
                }
            }            

            //Check to see if DataExpiry.dataExpired is true if so disable data  
            CheckExpiryRagData(ragObj, ragListView);

            RagJsonAdapter ragAdapter = new RagJsonAdapter(this, ragObj);

            ragListView.Adapter = ragAdapter;
        }

        //Run this method, if expired makes data null
        private void CheckExpiryRagData(List<RagJson> ragObj, ListView ragListView)
        {
            if (MCOLTabbedDash.dataExpired)
            {
                ragObj = null;
                ragListView.Visibility = ViewStates.Gone;

                //ShowAlert("Sorry, the data has expired. Please log back in.", false);

                //Introduce new DialogFragment
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialog_LogIn loginPrompt = new dialog_LogIn();
                loginPrompt.Cancelable = false;
                loginPrompt.Show(transaction, "dialog fragment");
            }
        }

        public string GetRagJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/rag");
            request.ContentType = "application/json; charset=utf-8";

            //Below needs to be commented out if i'm debugging on android device
            if (MainActivity._localProxy != null)
            {
                request.Proxy = new WebProxy("proxy.logica.com", 80);
            }

            string json;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();

                //Praise the lord for this line below
                json = JToken.Parse(json).ToString();

                //Remove \ and quotes wrapped round json
                //json = json.Replace(@"\", string.Empty);
                //json = json.Substring(1, json.Length - 2);

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
                    //Go back to log in page
                    Intent logInPage = new Intent(this.ApplicationContext, typeof(MainActivity));
                    StartActivity(logInPage);
                });
                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
            
        }        
    }
}

