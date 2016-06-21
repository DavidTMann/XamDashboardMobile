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
using Android.Support.V4.Widget;

namespace MobileDashboard.mTabs
{
    class McolSummaryFragment : Fragment
    {
        RAGActivity rag = new RAGActivity();
        McolAlertsFragment a = new McolAlertsFragment();
        DataExpiry dt = new DataExpiry();
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.McolSummFrag, container, false);

            //Get rag json from 
            string ragJson = rag.GetRagJson();

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
            Button summAlertsLv1Btn = rootView.FindViewById<Button>(Resource.Id.mcolSummAlertLv1);
            Button summAlertsLv2Btn = rootView.FindViewById<Button>(Resource.Id.mcolSummAlertLv2);
            Button summAlertsLv3Btn = rootView.FindViewById<Button>(Resource.Id.mcolSummAlertLv3);

            //Alert counters
            int lv1AlertCount = 0;
            int lv2AlertCount = 0;
            int lv3AlertCount = 0;

            foreach (var al in mcolAlerts)
            {
                if (al.serverAlerts.priority == "Level 1")
                {
                    lv1AlertCount++;
                }

                if (al.serverAlerts.priority == "Level 2")
                {
                    lv2AlertCount++;
                }

                if (al.serverAlerts.priority == "Level 3")
                {
                    lv3AlertCount++;
                }
            }

            //Assign alert count to buttons
            summAlertsLv1Btn.Text = string.Format("Level 1 Alerts : {0}", lv1AlertCount.ToString());
            summAlertsLv2Btn.Text = string.Format("Level 2 Alerts : {0}", lv2AlertCount.ToString());
            summAlertsLv3Btn.Text = string.Format("Level 3 Alerts : {0}", lv3AlertCount.ToString());

            //Set alert background btn colour
            SetAlertBackgroundColour(summAlertsLv1Btn, summAlertsLv2Btn, summAlertsLv3Btn, lv1AlertCount, lv2AlertCount, lv3AlertCount);

            //DATA EXPIRY
            //Check to see if data has expired
            dt.IsExpired(DataExpiry.expiryDate);
            //Check to see if DataExpiry.dataExpired is true if so disable data  
            ExpireSummaryData(ragObj, mcolAlerts, ragScoreBtn, summAlertsLv1Btn, summAlertsLv2Btn, summAlertsLv3Btn);

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

        //Set button background colour depending on num of alerts
        private void SetAlertBackgroundColour(Button lv1AlertBtn, Button lv2AlertBtn, Button lv3AlertBtn, int lv1AlertCount, int lv2AlertCount, int lv3AlertCount)
        {
            //Level 1 Alert
            if (lv1AlertCount == 0)
            {
                lv1AlertBtn.SetBackgroundColor(Android.Graphics.Color.Green);
            }

            if (lv1AlertCount < 15 && lv1AlertCount > 0)
            {
                lv1AlertBtn.SetBackgroundColor(Android.Graphics.Color.Yellow);
            }

            if (lv1AlertCount > 15 && lv1AlertCount < 50)
            {
                lv1AlertBtn.SetBackgroundColor(Android.Graphics.Color.DarkOrange);
            }

            if (lv1AlertCount > 50)
            {
                lv1AlertBtn.SetBackgroundColor(Android.Graphics.Color.OrangeRed);
            }

            //Level 2
            if (lv2AlertCount == 0)
            {
                lv2AlertBtn.SetBackgroundColor(Android.Graphics.Color.Green);
            }

            if (lv2AlertCount < 15 && lv2AlertCount > 0)
            {
                lv2AlertBtn.SetBackgroundColor(Android.Graphics.Color.Yellow);
            }

            if (lv2AlertCount > 15 && lv2AlertCount < 50)
            {
                lv2AlertBtn.SetBackgroundColor(Android.Graphics.Color.DarkOrange);
            }

            if (lv2AlertCount > 50)
            {
                lv2AlertBtn.SetBackgroundColor(Android.Graphics.Color.OrangeRed);
            }

            //Level 3
            if (lv3AlertCount == 0)
            {
                lv3AlertBtn.SetBackgroundColor(Android.Graphics.Color.Green);
            }

            if (lv3AlertCount < 15 && lv3AlertCount > 0)
            {
                lv3AlertBtn.SetBackgroundColor(Android.Graphics.Color.Yellow);
            }

            if (lv3AlertCount > 15 && lv3AlertCount < 50)
            {
                lv3AlertBtn.SetBackgroundColor(Android.Graphics.Color.DarkOrange);
            }

            if (lv3AlertCount > 50)
            {
                lv3AlertBtn.SetBackgroundColor(Android.Graphics.Color.OrangeRed);
            }

        }
    }
}