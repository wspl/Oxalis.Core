using System.Collections.Generic;
using System.Linq;

namespace Iris.Core.Modules
{
    public class LinkSearcher
    {
        private readonly Searcher _searcher = new Searcher();

        private readonly Dictionary<string, List<EverythingResultItem>> _displayLink = 
            new Dictionary<string, List<EverythingResultItem>>();
        private readonly Dictionary<string, List<string>> _displayLinksDict =
            new Dictionary<string, List<string>>();

        public LinkSearcher(Everything everything, CJKTranslator cjkTranslator)
        {
            var links = everything.Search(@"Start Menu regex:.lnk$");
            foreach (var link in links.Results)
            {
                var linkName = link.Name.Remove(link.Name.Length - 4, 4);
                if (_displayLink.ContainsKey(linkName))
                {
                    _displayLink[linkName].Add(link);
                }
                else
                {
                    _displayLink.Add(linkName, new List<EverythingResultItem> { link });
                    _displayLinksDict.Add(linkName, cjkTranslator.TranslateToList(linkName));
                }
            }
        }

        public List<string> Search(string keyword)
        {
            var result = (
                from di in _displayLinksDict
                let score = _searcher.Match(keyword, di.Value)
                where score > 0
                select new KeyValuePair<string, double>(di.Key, score)
            ).ToList();

            result.Sort((pair1, pair2) => -pair1.Value.CompareTo(pair2.Value));

            return result.Select(v => v.Key).ToList();
        }
    }
}
