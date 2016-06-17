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

namespace MobileDashboard
{   

    [Activity(Label = "RAG Stats")]
    public class RAGActivity : Activity
    {
        DataExpiry dt = new DataExpiry();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RAG);

            //Grab username from MainActivity
            string json = Intent.GetStringExtra("json") ?? null;

            //Deserialize json and put it in list view
            
            var ragObj = JsonConvert.DeserializeObject<List<RagJson>>(json);

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
                }
            }           

            
            var ragListView = FindViewById<ListView>(Resource.Id.listView);

            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            CheckExpiryRagData(ragObj, ragListView);

            ragListView.Adapter = new RagJsonAdapter(this, ragObj);
                      

            if (!DataExpiry.dataExpired)
            {
                //ListView item clicking
                ragListView.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                {

                    string selectedFromList = ragListView.GetItemAtPosition(e.Position).ToString();

                    ShowAlert("Would you like to go to MCOL Dashboard?", true);
                };
            }            
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

