using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BDO.Scraper
{
    class DownloadAsync
    {
        Action<string> _callback;
        string _baseUrl;
        int _errorCount;
        int _start;

        public DownloadAsync(Action<string> progressCallback, string baseUrl, int startNumber = 0)
        {
            _callback = progressCallback;
            _baseUrl = baseUrl;
            _start = startNumber;
        }
        
        /// <summary>
        /// The number of consecutive 404s to tolerate before moving to the next category.
        /// </summary>
        public int ErrorThreshold { get; set; } = 10;

        public int TotalPages { get; set; } = 1500;

        public string TargetDirectory { get; set; } = @"C:\Users\Michael\Dropbox\BDO\Scrapes\cooking\";

        public void Download(IEnumerable<string> pages)
        {
            var tasks = new List<Task>();

            foreach (var p in pages)
            {
                var page = _baseUrl + p;
                var file = p + ".html";
                var t = new Task(() => DownloadPage(page, file));

                tasks.Add(t);
                t.Start();
                t.ContinueWith(task => tasks.Remove(task));
                
                if (tasks.Count > 500)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void Download()
        {
            int counter = _start;
            var tasks = new List<Task>();

            while (_errorCount < 10)
            {
                counter++;
                var page = BuildAddress(_baseUrl, counter);
                var file = counter + ".html";
                var t = new Task(() => DownloadPage(page, file));
                tasks.Add(t);
                t.Start();
                t.ContinueWith(task => tasks.Remove(task));


                if (tasks.Count > 500)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        void DownloadPage(string address, string fileName)
        {
            var client = new WebClient();
            try
            {
                var contents = client.DownloadString(address);
                using (var sw = new StreamWriter(Path.Combine(TargetDirectory, fileName)))
                    sw.Write(contents);
            }
            catch (Exception e)
            {
                _callback.Invoke($"Error: {address}");
                _callback.Invoke($"Message: {e.Message}");
                _errorCount++;
                return;
            }
            _errorCount = 0;
        }

        string BuildAddress(string baseAddress, int counter)
        {
            return baseAddress + counter;
        }
    }
}