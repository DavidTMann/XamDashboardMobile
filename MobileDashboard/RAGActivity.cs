using System.Collections.Generic;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;
using MobileDashboard.JsonAdapters;
using MobileDashboard.SharedClass;

namespace MobileDashboard
{   

    [Activity(Label = "RAG Stats")]
    public class RAGActivity : Activity
    {
        DataExpiry dt = new DataExpiry();
        private PlotView plotViewModel;
        private LinearLayout mLLayoutModel;
        public PlotModel MyModel { get; set; }

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
                //Pie chart stuff; come back to this when Im not stressed
                List<int> appRagScores = new List<int>();
                List<string> appNames = new List<string>();

                //Add to array of app name 
                foreach (RagJson r in ragObj.ToArray())
                {
                    appRagScores.Add(r.RAGStatus);
                    appNames.Add(r.Name);
                }

                int[] modelAllocValues = appRagScores.ToArray();
                string[] modelAllocations = appNames.ToArray();

                string[] colors = new string[] { "#7DA137", "#6EA6F3" };
                int total = 0;

                //Pie chart oxyPlot
                plotViewModel = FindViewById<PlotView>(Resource.Id.plotViewModel);
                mLLayoutModel = FindViewById<LinearLayout>(Resource.Id.linearLayoutModel);

                //Model Allocation Pie char
                var plotModel2 = new PlotModel();
                var pieSeries2 = new PieSeries();
                pieSeries2.InsideLabelPosition = 0.0;
                pieSeries2.InsideLabelFormat = null;

                for (int i = 0; i < modelAllocations.Length && i < modelAllocValues.Length && i < colors.Length; i++)
                {
                    pieSeries2.Slices.Add(new PieSlice(modelAllocations[i], modelAllocValues[i]) { Fill = OxyColor.Parse(colors[i]) });
                    pieSeries2.OutsideLabelFormat = null;

                    double mValue = modelAllocValues[i];
                    double percentValue = (mValue / total) * 100;
                    string percent = percentValue.ToString("#.##");

                    //Add horizontal layout for titles and colors of slices
                    LinearLayout hLayot = new LinearLayout(this);
                    hLayot.Orientation = Android.Widget.Orientation.Horizontal;
                    LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                    hLayot.LayoutParameters = param;

                    //Add views with colors
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(15, 15);

                    View mView = new View(this);
                    lp.TopMargin = 5;
                    mView.LayoutParameters = lp;
                    mView.SetBackgroundColor(Android.Graphics.Color.ParseColor(colors[i]));

                    //Add titles
                    TextView label = new TextView(this);
                    label.TextSize = 15;
                    label.SetTextColor(Android.Graphics.Color.White);
                    label.Text = string.Join(" ", modelAllocations[i]);
                    param.LeftMargin = 8;
                    label.LayoutParameters = param;

                    hLayot.AddView(mView);
                    hLayot.AddView(label);
                    mLLayoutModel.AddView(hLayot);

                }

                plotModel2.Series.Add(pieSeries2);
                MyModel = plotModel2;
                plotViewModel.Model = MyModel;

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

