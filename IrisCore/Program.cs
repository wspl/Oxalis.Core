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
            var testString = "Visual Studio 2015";

            var rs = cjk.TranslateToList(testString.ToLower());

            var pool = new List<int[]>();
            foreach (var ctx in rs)
            {
                SearchWorker(pool, ctx, new int[Keyword.Length].Select((v) => -1).ToArray(), 0, 0);
            }


        }

        static string Keyword = "vs2015".ToLower();

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
