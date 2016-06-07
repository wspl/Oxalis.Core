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
            var testString = "网易云音乐";

            var rs = cjk.TranslateToList(testString.ToLower());

            var pool = new List<int[]>();

            var beforDT = DateTime.Now;

            // Get Trait
            foreach (var ctx in rs)
            {
                SearchWorker(pool, ctx, new int[Keyword.Length].Select((v) => -1).ToArray(), 0, 0);
            }

            Console.WriteLine("Get Trait Array Count：{0}", pool.Count);

            // Calculate First Chars Pos Score
            var fcPosList = new List<int> { 0 };
            var currentFcPos = 0;
            while (true)
            {
                var nextSpacePos = rs[0].IndexOf(' ', currentFcPos);
                currentFcPos = nextSpacePos + 1;
                if (nextSpacePos == -1) break;
                fcPosList.Add(currentFcPos);
            }

            var fcScores = new List<double>();
            foreach (var trait in pool)
            {
                var matchCount = 0;
                foreach (var pos in trait)
                {
                    if (fcPosList.Contains(pos))
                    {
                        matchCount += 1;
                    }
                }
                fcScores.Add(matchCount / (double)trait.Length);
            }
            var fcScore = fcScores.Count > 0 ? fcScores.Max() : 0;

            // Calculate Continuity Score
            var conScores = new List<double>();
            foreach (var trait in pool)
            {
                var continueCount = 0;
                for (var i = 1; i < trait.Length; i += 1)
                {
                    if (trait[i] == trait[i - 1] + 1)
                    {
                        continueCount += 1;
                    }
                }
                conScores.Add(continueCount / (double)trait.Length);
            }
            var conScore = conScores.Count > 0 ? conScores.Max() : 0;

            var aveScore = (fcScore + conScore) / 2;

            var afterDT = DateTime.Now;
            var ts = afterDT.Subtract(beforDT);
            Console.WriteLine("FirstChar Score: {0}", fcScore);
            Console.WriteLine("Continuity Score: {0}", conScore);
            Console.WriteLine("Final Score: {0}", aveScore);
            Console.WriteLine("Using {0} ms.", ts.TotalMilliseconds);
            Console.ReadKey();
        }

        static string Keyword = "wyiyuyyue".ToLower();

        // TODO: Demand Optimization
        static void SearchWorker(List<int[]> pool, string src, int[] lastTrait, int kwPos, int lastSearchPos)
        {
            if (lastTrait[lastTrait.Length - 1] == -1)
            {
                var searchPos = lastSearchPos;

                while (true)
                {
                    var attachPos = src.IndexOf(Keyword[kwPos], searchPos);
                    if (attachPos == -1) break;

                    var trait = lastTrait.Select((v, i) => i == kwPos ? attachPos : v).ToArray();
                    SearchWorker(pool, src, trait, kwPos + 1, attachPos + 1);

                    searchPos = attachPos + 1;
                }
            }
            else
            {
                var existSame = false;
                foreach (var trait in pool)
                {
                    var sameCount = 0;
                    for (var i = 0; i < trait.Length; i += 1)
                    {
                        if (trait[i] == lastTrait[i])
                        {
                            sameCount += 1;
                        }
                    }
                    if (sameCount == trait.Length)
                    {
                        existSame = true;
                        break;
                    }
                }

                if (!existSame)
                {
                    pool.Add(lastTrait);
                }
            }
        }
    }
}
