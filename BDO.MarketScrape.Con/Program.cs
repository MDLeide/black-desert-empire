using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BDO.Domain.Enum;
using BDO.MarketScraper;
using BDO.Persistence.Repo;
using NTC.UTL;

namespace BDO.MarketScrape.Con
{
    class Program
    {
        static string _tessData = @"./tessdata";
        static string _rootPath = @"C:\Users\Michael\Dropbox\BDO\OCR\";

        static string _resultsDirectory = _rootPath + @"output\";
        static string _imagesDirectory = _rootPath + @"images\";

        static string _itemSamplesPath = @"C:\Users\Michael\Dropbox\BDO\OCR\item samples";

        static string _engineOutput = @"C:\Users\Michael\Dropbox\BDO\Engine_Test";
        static string _engineLog = @"C:\Users\Michael\Dropbox\BDO\Engine_Test\log.txt";
        static string _engineImages = @"C:\Users\Michael\Dropbox\BDO\engine_images";

        static void Main(string[] args)
        {
            //TestMethods.TestDecompose();\
            //TestEngine();
            //return;

            //TestMethods.Test(@"C:\Users\Michael\Dropbox\BDO\OCR\images\icon.png");
            //TestMethods.Test(@"C:\Users\Michael\Dropbox\BDO\OCR\images\plus ten.png");
            //TestMethods.Test(@"C:\Users\Michael\Dropbox\BDO\OCR\images\plus one wheel.png");

            var dir = new DirectoryInfo(@"C:\Users\Michael\Dropbox\BDO\OCR\enhance test\enhanced");

            var sb = new StringBuilder();
            foreach (var i in dir.EnumerateFiles().Where(p => p.Extension == ".png"))
            {
                Console.WriteLine(i.Name);
                TestMethods.Test(i.FullName, str => Console.WriteLine(str));
            }

            Console.ReadKey();
        }

        static void TestEngine()
        {
            var output = new DirectoryInfo(_engineOutput);
            if (!output.Exists)
                output.Create();

            var log = new FileInfo(_engineLog);
            if (!log.Exists)
                log.Create().Dispose();

            int unitCount = 0;

            using (var itemRepo = new ItemRepository())
            {
                using (var marketRepo = new MarketObservationRepository())
                {
                    var engine = new Engine(_tessData, itemRepo, marketRepo);
                    engine.UIScale = 1.7;
                    engine.LogObservationsToFile = false;
                    engine.ObservationLogFile = _engineLog;
                    engine.SaveScreenshots = false;
                    engine.ScreenshotDirectory = _engineOutput;
                    engine.SaveDecomposedImages = false;
                    engine.DecomposedImageDirectory = _engineOutput;
                    engine.MinimumConfidence = .7;
                    engine.SaveObservationsToDatabase = false;

                    engine.RequireMarketPrice = true;

                    Console.WriteLine("Press a key to snap a screenshot.");
                    while (true)
                    {
                        Console.ReadKey();
                        Console.WriteLine("Snap");
                        engine.ScreenshotAndProcess(true);
                        Console.WriteLine("Done");
                    }
                    


                    Console.WriteLine("Press any key to start auto snapping.");
                    Console.ReadKey();
                    engine.StartAutoSnapping(1000);
                    Console.WriteLine("Press the E key to stop autosnapping.");
                    
                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.E)
                        {
                            var workers = engine.StopAutoSnapping();
                            break;
                        }
                    }



                    //var unit = engine.ScreenshotAndProcess();
                    //unit.Complete += (sender, args) => unitCount--;
                    //unit.Start();
                    //unitCount++;


                    //var imageDirectory = new DirectoryInfo(_engineImages);

                    //foreach (var img in imageDirectory.EnumerateFiles())
                    //{
                    //    unitCount++;
                    //    var bmp = (Bitmap) Image.FromFile(img.FullName);
                    //    var unit = engine.ProcessImage(bmp, false);
                    //    unit.Complete += (sender, args) => unitCount--;
                    //    unit.Start();
                    //}
                }
            }

            while (unitCount > 0)
                Thread.Sleep(10);

            Console.WriteLine("Complete");
        }
    }
}
