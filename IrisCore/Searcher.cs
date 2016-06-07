using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iris.Core
{
    class Searcher
    {
        public double Match(string keyword, List<string> strings)
        {
            // Get Trait
            var pool = new List<int[]>();
            foreach (var ctx in strings)
            {
                //SearchWorker(keyword, pool, ctx, new int[keyword.Length].Select((v) => -1).ToArray(), 0, 0);
                var traits = SearchWorkerII(ctx.ToLower(), keyword.ToLower());
                pool.AddRange(traits.Where((v) => {
                    foreach (var trait in pool)
                    {
                        if (trait.SequenceEqual(v)) return false;
                    }
                    return true;
                }).ToArray());
            }

            // Calculate First Chars Pos Score
            var fcPosList = new List<int> { 0 };
            var currentFcPos = 0;
            while (true)
            {
                var nextSpacePos = strings[0].IndexOf(' ', currentFcPos);
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
                fcScores.Add(matchCount / ((double)(trait.Length + strings[0].Length) / 2));
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
                conScores.Add(continueCount / ((double)(trait.Length + strings[0].Length) / 2));
            }
            var conScore = conScores.Count > 0 ? conScores.Max() : 0;

            // Front Score
            var fntScores = new List<double>();
            foreach (var trait in pool)
            {
                fntScores.Add((strings[0].Length - trait[0]) / (double)strings[0].Length);
            }
            var fntScore = fntScores.Count > 0 ? fntScores.Max() : 0;

            var aveScore = fcScore + conScore + fntScore;

            return aveScore;
        }

        private List<int[]> SearchWorkerII(string src, string keyword)
        {
            var initTrait = new int[keyword.Length].Select((v) => -1).ToArray();
            var parentTraits = new Dictionary<int, List<int[]>>();

            for (var kcIndex = 0; kcIndex < keyword.Length; kcIndex += 1)
            {
                var kc = keyword[kcIndex];
                if (kcIndex == 0)
                {
                    var matchFrom = 0;
                    var nextMatchPos = matchFrom;

                    parentTraits[0] = new List<int[]>();

                    while (true)
                    {
                        var attachPos = src.IndexOf(kc, nextMatchPos);
                        if (attachPos > -1)
                        {
                            var trait = new int[keyword.Length].Select((v, i) => i == 0 ? attachPos : -1).ToArray();
                            parentTraits[0].Add(trait);
                            nextMatchPos = attachPos + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    parentTraits[kcIndex] = new List<int[]>();

                    foreach (var parentTrait in parentTraits[kcIndex - 1])
                    {
                        var matchFrom = parentTrait[kcIndex - 1];
                        var nextMatchPos = matchFrom + 1;

                        while (true)
                        {
                            var attachPos = src.IndexOf(kc, nextMatchPos);
                            if (attachPos > -1)
                            {
                                var trait = parentTrait.Select((v, i) => i == kcIndex ? attachPos : v).ToArray();
                                parentTraits[kcIndex].Add(trait);
                                nextMatchPos = attachPos + 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return parentTraits[keyword.Length - 1];
        }

        // Too slowly
        private void SearchWorker(string keyword, List<int[]> pool, string src, int[] lastTrait, int kwPos, int lastSearchPos)
        {
            if (lastTrait[lastTrait.Length - 1] == -1)
            {
                var searchPos = lastSearchPos;

                while (true)
                {
                    var attachPos = src.IndexOf(keyword[kwPos], searchPos);
                    if (attachPos == -1) break;

                    var trait = lastTrait.Select((v, i) => i == kwPos ? attachPos : v).ToArray();
                    SearchWorker(keyword, pool, src, trait, kwPos + 1, attachPos + 1);

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
