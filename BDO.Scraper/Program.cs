using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BDO.WebScraper
{
    class Program
    {
        static string _scrapes = @"C:\Users\Michael\Dropbox\BDO\Scrapes\";

        static void Main(string[] args)
        {
            //var designs = ParseDesignList();
            //var pages = designs.Where(p => !string.IsNullOrEmpty(p.DesignName)).Select(p => p.Id.ToString());
            //var downloader = new DownloadAsync(Console.WriteLine, "http://bddatabase.net/us/recipe/");
            //downloader.Download(pages);
            
            Parse();

            Console.Beep();
        }

        static IEnumerable<DesignParse> ParseDesignList()
        {
            var file = @"C:\Users\Michael\Dropbox\BDO\Scrapes\cooking lists\page ";
            
            List<DesignParse> designs = new List<DesignParse>();

            for (int i = 0; i < 1; i++)
            {
                string html;
                using (var sr = new StreamReader(file + (i + 1) + ".html"))
                    html = sr.ReadToEnd();
                var parser = new DesignListParser();

                designs.AddRange(parser.Parse(html));
            }

            var sb = new StringBuilder();
            foreach (var d in designs)
                sb.AppendLine($"{d.Id}, {d.DesignName}");

            var target = @"C:\Users\Michael\Dropbox\BDO\Scrapes\cooking list.csv";
            using (var sw = new StreamWriter(target))
                sw.Write(sb.ToString());

            return designs;
        }

        static void Parse()
        {
            var dir = @"C:\Users\Michael\Dropbox\BDO\Scrapes\cooking\";
            var results = ParseDirectory(dir);
            foreach (var r in results)
                r.RecipeType = "alchemy";
            var header = _scrapes + @"recipe_header.csv";
            var detail = _scrapes + @"recipe_details.csv";
            var count   = RecipePreimport.FilterAndSaveToDatabase(results);
            Console.WriteLine(count);
            Console.ReadKey();
            RecipePreimport.SaveParseResults(results, header, detail);
        }

        static IEnumerable<RecipeParse> ParseDirectory(string directory)
        {
            var dir = new DirectoryInfo(directory);
            var results = new List<RecipeParse>();
            var parser = new RecipeParser();
            foreach (var f in dir.EnumerateFiles())
            {
                try
                {
                    string html;
                    using (var sr = new StreamReader(f.FullName))
                        html = sr.ReadToEnd();

                    if (string.IsNullOrEmpty(html))
                        continue;
                    var result = parser.Parse(html);
                    result.Id = f.Name.Replace(".html", "");
                    results.Add(result);
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"File {f.Name} failed: {e.Message}");
                }
            }

            return results;
        } 

        static void Download()
        {
            var d = new DownloadAsync((s) => Console.WriteLine(s), "http://bddatabase.net/us/design/");
            d.TargetDirectory = @"C:\Users\Michael\Dropbox\BDO\Scrapes\design";
            d.Download();

            //var d = new Downloader((s) => Console.WriteLine(s));
            //d.Download();
            Console.WriteLine("Complete");
            Console.ReadKey();
        }
    }
}
