using System;
using Iris.Core.Modules;

namespace Iris.Core
{

    class Program
    {
        private static CJKTranslator cjk = new CJKTranslator();

        static void Main(string[] args)
        {
            var lnkSearcher = new LinkSearcher(new Everything(), cjk);
            var searcher = new Searcher();
            while (true)
            {
                Console.Write("\n> ");
                var keyword = Console.ReadLine();
                var beforDT = DateTime.Now;

                var result = lnkSearcher.Search(keyword);

                foreach (var ri in result)
                {
                    Console.WriteLine(ri);
                }

                var afterDT = DateTime.Now;
                var ts = afterDT.Subtract(beforDT);
                //Console.WriteLine("Final Score: {0}", av+eScore);
                Console.WriteLine("Using {0} ms.", ts.TotalMilliseconds);
            }
        }
    }
}
