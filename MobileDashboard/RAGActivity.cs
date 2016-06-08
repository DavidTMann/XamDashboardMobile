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

namespace MobileDashboard
{   

    [Activity(Label = "RAG Stats")]
    public class RAGActivity : Activity
    {

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
            
            var rag = JsonConvert.DeserializeObject<List<RagJson>>(json);

            //remove all apps apart from mcol and darts
            foreach (var r in rag.ToArray())
            {
                if (r.Name == "MCOL" || r.Name == "DARTS")
                {
                    continue;
                }

                if (!r.Name.Contains("MCOL") || (!r.Name.Contains("DARTS")))
                {
                    rag.Remove(r);
                }
            }

            var listView = FindViewById<ListView>(Resource.Id.listView);
            listView.Adapter = new RagJsonAdapter(this, rag);
            
            //Come back to this when Im not stressed
            List<int> appRagScores = new List<int>();
            List<string> appNames = new List<string>();
            
            //Add to array of app name 
            foreach (RagJson r in rag.ToArray())
            {
                appRagScores.Add(r.RAGStatus);
                appNames.Add(r.Name);
            }

            int[] modelAllocValues = appRagScores.ToArray();
            string[] modelAllocations = appNames.ToArray();

            string[] colors = new string[] { "#7DA137", "#6EA6F3"};
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

        }        
    }
        
    public class RagJson
    {
        public string Name { get; set; }
        public int RAGStatus { get; set; }
        public string RAGColour { get; set; }
        public string BusinessUnit { get; set; }
        public string Business { get; set; }
    }

    public class RagJsonAdapter : BaseAdapter<RagJson>
    {
        private readonly IList<RagJson> _items;
        private readonly Context _context;

        public RagJsonAdapter(Context context, IList<RagJson> items)
        {
            _items = items;
            _context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var view = convertView;

            if (view == null)
            {
                var inflater = LayoutInflater.FromContext(_context);
                view = inflater.Inflate(Resource.Layout.RAGrow, parent, false);
            }

            TextView appName = view.FindViewById<TextView>(Resource.Id.AppName);
            TextView rating = view.FindViewById<TextView>(Resource.Id.Rating);
            TextView buName = view.FindViewById<TextView>(Resource.Id.BuName);

            appName.Text = item.Name;
            rating.Text = item.RAGStatus.ToString();
            buName.Text = item.BusinessUnit;

            //Set text color black
            appName.SetTextColor(Color.Black);
            rating.SetTextColor(Color.Black);
            buName.SetTextColor(Color.Black);

            if (Convert.ToInt32(rating.Text) <= 20)
            {
                appName.SetBackgroundColor(Color.Black);
                rating.SetBackgroundColor(Color.Black);
                buName.SetBackgroundColor(Color.Black);
                //Set to white if blk background color
                appName.SetTextColor(Color.White);
                rating.SetTextColor(Color.White);
                buName.SetTextColor(Color.White);
            }
            if (Convert.ToInt32(rating.Text) <= 40 && Convert.ToInt32(rating.Text) > 20)
            {
                appName.SetBackgroundColor(Color.Red);
                rating.SetBackgroundColor(Color.Red);
                buName.SetBackgroundColor(Color.Red);
            }
            if (Convert.ToInt32(rating.Text) <= 85 && Convert.ToInt32(rating.Text) > 40)
            {
                appName.SetBackgroundColor(Color.Yellow);
                rating.SetBackgroundColor(Color.Yellow);
                buName.SetBackgroundColor(Color.Yellow);
            }
            if (Convert.ToInt32(rating.Text) == 100)
            {
                appName.SetBackgroundColor(Color.Green);
                rating.SetBackgroundColor(Color.Green);
                buName.SetBackgroundColor(Color.Green);
            }


            return view;
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override RagJson this[int position]
        {
            get { return _items[position]; }
        }
    }
}

