using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.MarketScraper.Img;
using Tesseract;

namespace BDO.MarketScraper
{
    public class TestMethods
    {
        static string _dataPath = @"./tessdata";

        public static void Test(string imagePath)
        {
            Test(imagePath, Console.WriteLine);
        }

        public static void TestEnhancedIcon()
        {
            
        }

        public static void TestDecompose()
        {
            var dir = new DirectoryInfo(@"C:\Users\Michael\Dropbox\BDO\OCR\ui scaling\fullscreens");
            var output = new DirectoryInfo(@"C:\Users\Michael\Dropbox\BDO\OCR\ui scaling\fullscreens\output");

            foreach (var img in dir.EnumerateFiles())
            {
                Console.WriteLine($"Processiong {img.Name} : {img.Length/1000000} mb");

                var intScale = Convert.ToInt32(img.Name.Replace(img.Extension, ""));
                var scale = intScale/100d;
                var bitmap = (Bitmap)Image.FromFile(img.FullName);

                var region = ImageDecomposer.Decompose(bitmap, scale, bitmap.Width, bitmap.Height);
                
                var saveDir = new DirectoryInfo(output + @"\" + img.Name);
                if (!saveDir.Exists)
                    saveDir.Create();

                var queue = new Queue<ImageRegion>();
                queue.Enqueue(region);

                while (queue.Count > 0)
                {
                    var currentRegion = queue.Dequeue();
                    foreach (var c in currentRegion.ChildRegions)
                        queue.Enqueue(c);

                    var newBitmap = currentRegion.GetColorData(bitmap);

                    newBitmap.Save(Path.Combine(saveDir.FullName, currentRegion.RegionName + ".png"));

                    newBitmap.Dispose();
                }

                bitmap.Dispose();
            }
        }

        public static void Test(string imagePath, Action<string> output)
        {
            using (var engine = new TesseractEngine(_dataPath, "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        output.Invoke("Full Text:");
                        output.Invoke(text);
                        output.Invoke(string.Empty);

                        output.Invoke("Confidence: " + page.GetMeanConfidence());

                        using (var iter = page.GetIterator())
                        {
                            do
                            {
                                do
                                {
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                        output.Invoke("Block");
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Para))
                                        output.Invoke("Para");
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine))
                                        output.Invoke("Line");

                                    output.Invoke($"Word: {iter.GetText(PageIteratorLevel.Word)}");

                                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                                
                            } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                        }

                    }
                }
            }
        }
    }
}
