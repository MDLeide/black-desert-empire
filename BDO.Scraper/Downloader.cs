//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;

//namespace BDO.Scraper
//{
//    class Downloader
//    {
//        //http://bddatabase.net/us/item/9938/
//        //http://bddatabase.net/us/design/9810/
//        //http://bddatabase.net/us/mrecipe/2/
//        //http://bddatabase.net/us/recipe/36/

//        Dictionary<string, string> _baseUrls = new Dictionary<string, string>();

//        string _itemBaseUrl = "http://bddatabase.net/us/item/";
//        string _designBaseUrl = "http://bddatabase.net/us/design/";
//        string _mRecipeBaseUrl = "http://bddatabase.net/us/mrecipe/";
//        string _recipeBaseUrl = "http://bddatabase.net/us/recipe/";
//        Action<string> _callback;

//        public Downloader(Action<string> progressCallback)
//        {
//            _callback = progressCallback;

//            //_baseUrls.Add("item", _itemBaseUrl);
//            _baseUrls.Add("design", _designBaseUrl);
//            _baseUrls.Add("mrecipe", _mRecipeBaseUrl);
//            _baseUrls.Add("recipe", _recipeBaseUrl);
//        }

//        /// <summary>
//        /// The number of consecutive 404s to tolerate before moving to the next category.
//        /// </summary>
//        public int ErrorThreshold { get; set; } = 10;

//        public int PagesPerCategory { get; set; } = 1500;

//        public string TargetDirectory { get; set; } = @"E:\Dropbox\BDO\Scrapes\";

//        public void Download()
//        {
//            int totalCounter = 0;
//            using (var client = new WebClient())
//            {
//                foreach (var baseUrl in _baseUrls)
//                {
//                    int errors = 0;
//                    int counter = 0;
//                    int progress = 0;
//                    while (errors < ErrorThreshold && counter < PagesPerCategory)
//                    {
//                        counter++;
//                        totalCounter++;
//                        var page = BuildAddress(baseUrl.Value, counter);
//                        string contents;
//                        try
//                        {
//                            contents = client.DownloadString(page);
//                        }
//                        catch (WebException e)
//                        {
//                            errors++;
//                            _callback.Invoke($"Exception: {e.Message}");
//                            continue;
//                        }

//                        using (var sw = new StreamWriter(Path.Combine(TargetDirectory, baseUrl.Key + ".html")))
//                            sw.Write(contents);

//                        progress++;
//                        if (progress > 10)
//                        {
//                            progress = 0;
//                            _callback.Invoke($"Items downloaded: {totalCounter}");
//                        }
//                    }
//                }
//            }
//        }

//        void DownloadPage(string address)
//        {

//        }

//        string BuildAddress(string baseAddress, int counter)
//        {
//            return baseAddress + counter;
//        }
//    }
//}