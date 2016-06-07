using System.Collections.Generic;
using System.Linq;

namespace Iris.Core.Modules
{
    public class Searcher
    {
        public double Match(string keyword, List<string> strings)
        {
            // Get Trait
            var pool = new List<int[]>();
            foreach (var ctx in strings)
            {
                var traits = SearchWorker(ctx.ToLower(), keyword.ToLower());
                // Remove Dumplication
                traits = traits.Where(
                    v => pool.All(trait => !trait.SequenceEqual(v))
                );
                pool.AddRange(traits.ToArray());
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

            var fcScores = (
                from trait in pool
                let matchCount = trait.Count(pos => fcPosList.Contains(pos))
                select matchCount / ((double)(trait.Length + strings[0].Length) / 2)
            ).ToList();
            
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
            var fntScores = pool.Select(
                trait => (strings[0].Length - trait[0]) / (double)strings[0].Length
            ).ToList();

            var fntScore = fntScores.Count > 0 ? fntScores.Max() : 0;

            var aveScore = fcScore + conScore + fntScore;

            return aveScore;
        }

        private IEnumerable<int[]> SearchWorker(string src, string keyword)
        {
            var parentTraits = new Dictionary<int, List<int[]>>();

            for (var kcIndex = 0; kcIndex < keyword.Length; kcIndex += 1)
            {
                var kc = keyword[kcIndex];
                if (kcIndex == 0)
                {
                    const int matchFrom = 0;
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
    }
}
