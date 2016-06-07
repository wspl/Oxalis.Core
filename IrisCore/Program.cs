using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing;
using TsudaKageyu;

namespace Iris.Core
{

    class Program
    {
        private static CJKTranslator cjk = new CJKTranslator();
        public static List<string> TranslatedResult = new List<string>();

        static void Main(string[] args)
        {
            

            var cjk = new CJKTranslator();
            var everything = new Everything();

            var displayLink = new Dictionary<string, List<Everything.EverythingResultItem>>();
            var displayLinksDict = new Dictionary<string, List<string>>();
            var startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var links = everything.Search(@"Start Menu regex:.lnk$");
            foreach (var link in links.Results)
            {
                var linkName = link.Name.Remove(link.Name.Length - 4, 4);
                if (displayLink.ContainsKey(linkName))
                {
                    displayLink[linkName].Add(link);
                }
                else
                {
                    displayLink.Add(linkName, new List<Everything.EverythingResultItem> { link });
                    displayLinksDict.Add(linkName, cjk.TranslateToList(linkName));
                }
            }

            var searcher = new Searcher();
            while (true)
            {
                Console.Write("\n> ");
                var keyword = Console.ReadLine();
                var beforDT = DateTime.Now;

                var result = new List<KeyValuePair<string, double>>();
                foreach (var di in displayLinksDict)
                {
                    var score = searcher.Match(keyword, di.Value);
                    if (score > 0)
                    {
                        result.Add(new KeyValuePair<string, double>(di.Key, score));
                    }
                }
                result.Sort((pair1, pair2) => -pair1.Value.CompareTo(pair2.Value));

                foreach (var ri in result)
                {
                    Console.WriteLine(ri.Key);
                }


                var afterDT = DateTime.Now;
                var ts = afterDT.Subtract(beforDT);
                //Console.WriteLine("Final Score: {0}", aveScore);
                Console.WriteLine("Using {0} ms.", ts.TotalMilliseconds);
            }
        }

        static string Keyword = "wangyi".ToLower();

        
    }
}
