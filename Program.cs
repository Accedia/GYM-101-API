using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ImageAnalyze
{
    class Gym101
    {
        private static readonly List<string> _topics = new List<string> { "" };

        // subscriptionKey = ""
        private const string subscriptionKey = "";

        //private const string remoteImageUrl =
        //    "https://upload.wikimedia.org/wikipedia/commons/3/3c/Shaki_waterfall.jpg";

        private const string remoteImageUrl =
            "https://trainwithpg.com/wp-content/uploads/2017/01/insigniabenchpressv3_-2-e1500232088616.png";


        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags, VisualFeatureTypes.Objects
        };

        static void Main(string[] args)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // You must use the same region as you used to get your subscription
            // keys. For example, if you got your subscription keys from westus,
            // replace "westcentralus" with "westus".
            //
            // Free trial subscription keys are generated in the "westus"
            // region. If you use a free trial subscription key, you shouldn't
            // need to change the region.

            // Specify the Azure region
            computerVision.Endpoint = "https://westcentralus.api.cognitive.microsoft.com";

            Console.WriteLine("Images being analyzed ...");

            string html = GetHtmlCode();
            List<string> urls = GetUrls(html);

            var t1 = AnalyzeRemoteAsync(computerVision, remoteImageUrl);
            Task.WhenAll(t1).Wait(5000);

            //foreach (var item in urls)
            //{
            //    var t1 = AnalyzeRemoteAsync(computerVision, item);
            //    Task.WhenAll(t1).Wait(1000);
            //}

            //var t2 = AnalyzeLocalAsync(computerVision, localImagePath);
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        // Analyze a remote image
        private static async Task AnalyzeRemoteAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            ImageAnalysis analysis =
                await computerVision.AnalyzeImageAsync(imageUrl, features);
            DisplayResults(analysis, imageUrl);
        }

        // Display the most relevant caption for the image
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine(imageUri);

            Console.WriteLine(analysis.Description.Captions[0].Text);

            var objects = string.Empty;
            foreach (var item in analysis.Objects)
            {
                objects += " " + item.ObjectProperty;
            }

            Console.WriteLine("Objects: " + objects);

            objects = string.Empty;
            foreach (var item in analysis.Tags)
            {
                objects += " " + item.Name;
            }

            Console.WriteLine("Tags: " + objects + "\n");
        }

        private static string GetHtmlCode()
        {
            var rnd = new Random();

            int topic = rnd.Next(0, _topics.Count - 1);

            string url = "https://www.google.com/search?q=" + _topics[topic] + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private static List<string> GetUrls(string html)
        {
            var urls = new List<string>();

            int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
            }
            return urls;
        }
    }
}