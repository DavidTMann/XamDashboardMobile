using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Newtonsoft.Json;
using MobileDashboard.JsonAdapters;
using MobileDashboard.SharedClass;

namespace MobileDashboard.mTabs
{
    class McolSummaryFragment : Fragment
    {
        MenuActivity m = new MenuActivity();
        McolAlertsFragment a = new McolAlertsFragment();
        DataExpiry dt = new DataExpiry();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolSummFrag, container, false);

            //Get rag json from 
            string ragJson = m.GetRagJson();

            //Deserialize json into c# obj
            var ragObj = JsonConvert.DeserializeObject<List<RagJson>>(ragJson);

            //Only get MCOL RAG obj
            foreach (var rag in ragObj.ToArray())
            {
                if (rag.Name != "MCOL")
                {
                    ragObj.Remove(rag);
                }
            }

            //Assign RAG Score to button text can use 0 index as only MCOL in list
            Button ragScoreBtn = rootView.FindViewById<Button>(Resource.Id.mcolSummRagButton);
            ragScoreBtn.Text = ragObj[0].RAGStatus.ToString();
            ragScoreBtn.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Black));

            //Set rag button colour
            SetRagButtonColour(ragScoreBtn, ragObj);

            //Get server alert json for MCOL
            string alertJson = a.GetMcolServAlertsJson();

            //Deserialize into alert c# obj
            var mcolAlerts = JsonConvert.DeserializeObject<List<McolAlerts>>(alertJson);

            

            //MCOL Alerts for summary buttons
            Button summAlertsLv3Btn = rootView.FindViewById<Button>(Resource.Id.mcolSummAlertLv3);
            Button summAlertsLv4Btn = rootView.FindViewById<Button>(Resource.Id.mcolSummAlertLv4);
            Button summAlertsLv5Btn = rootView.FindViewById<Button>(Resource.Id.mcolSummAlertLv5);
            
            //Alert counters
            int lv3AlertCount = 0;
            int lv4AlertCount = 0;
            int lv5AlertCount = 0;

            foreach (var al in mcolAlerts)
            {
                if (al.serverAlerts.priority == "Level 3")
                {
                    lv3AlertCount++;
                }

                if (al.serverAlerts.priority == "Level 4")
                {
                    lv4AlertCount++;
                }

                if (al.serverAlerts.priority == "Level 5")
                {
                    lv5AlertCount++;
                }
            }

            //Assign alert count to buttons
            summAlertsLv3Btn.Text = string.Format("Level 3 Alerts : {0}", lv3AlertCount.ToString());
            summAlertsLv4Btn.Text = string.Format("Level 4 Alerts : {0}", lv4AlertCount.ToString());
            summAlertsLv5Btn.Text = string.Format("Level 5 Alerts : {0}", lv5AlertCount.ToString());

            //DATA EXPIRY
            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            ExpireSummaryData(ragObj, mcolAlerts, ragScoreBtn, summAlertsLv3Btn, summAlertsLv4Btn, summAlertsLv5Btn);

            Button menuBtn = rootView.FindViewById<Button>(Resource.Id.summBackToMenuBtn);
            menuBtn.Click += delegate
            {
                //Go to mcol tabbed dash page                    
                Intent menu = new Intent(this.Activity, typeof(MenuActivity));
                StartActivity(menu);
            };
            
            return rootView;
        }

        //Expires summary data if data has expired
        private void ExpireSummaryData(List<RagJson> ragObj, List<McolAlerts> mcolObj, Button ragScoreBtn, Button lvl3AlertBtn, Button lvl4AlertBtn, Button lvl5AlertBtn)
        {
            if (DataExpiry.dataExpired)
            {
                ragObj = null;
                mcolObj = null;
                ragScoreBtn.Text = "Data Expired";
                lvl3AlertBtn.Text = "Data Expired";
                lvl4AlertBtn.Text = "Data Expired";
                lvl5AlertBtn.Text = "Data Expired";
            }
        }        

        //Decides what colour to set RAG button
        private void SetRagButtonColour(Button ragScoreBtn, List<RagJson> ragObj)
        {
            if (ragObj[0].RAGStatus <= 20)
            {
                ragScoreBtn.SetBackgroundColor(Android.Graphics.Color.Black);
            }

            if (ragObj[0].RAGStatus <= 40 && ragObj[0].RAGStatus > 20)
            {
                ragScoreBtn.SetBackgroundColor(Android.Graphics.Color.Red);
            }

            if (ragObj[0].RAGStatus <= 85 && ragObj[0].RAGStatus > 40)
            {
                ragScoreBtn.SetBackgroundColor(Android.Graphics.Color.Yellow);
            }

            if (ragObj[0].RAGStatus == 100)
            {
                ragScoreBtn.SetBackgroundColor(Android.Graphics.Color.Green);
            }

        }
    }
}