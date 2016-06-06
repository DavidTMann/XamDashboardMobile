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
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MobileDashboard
{
    [Activity(Label = "RAGActivity")]
    public class RAGActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RAG);

            string json = GetRagJson();

            var rag = JsonConvert.DeserializeObject<List<RagJson>>(json);
            
        }

        private string GetRagJson()
        {
            var request = WebRequest.Create(@"https://www.warren-ayling.me.uk:8443/api/dashboard/rag");
            request.ContentType = "application/json; charset=utf-8";

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
    }
        
    public class RagJson
    {
        public string Name { get; set; }
        public int RAGStatus { get; set; }
        public string RAGColour { get; set; }
        public string BusinessUnit { get; set; }
        public string Business { get; set; }
    }
}