using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Iris.Core
{
    class CJKTranslator
    {
        public Dictionary<string, List<string>> Dict = new Dictionary<string, List<string>>();

        public CJKTranslator()
        {
            ParseChinesePinyin();
        }

        public string Translate(string input)
        {
            var translatedString = "";
            foreach (var character in input)
            {
                var charStr = character.ToString();
                if (Dict.ContainsKey(charStr))
                {
                    translatedString += string.Join(",", Dict[charStr]) + " ";
                }
                else
                {
                    translatedString += charStr;
                }
            }
            return new Regex(@"\s+").Replace(translatedString, " ");
        }

        private void ParseChinesePinyin()
        {
            var chineseDictPath = Path.Combine(Configure.ResourcesPath, "CJK", "dict_chinese.js");
            var fileLines = Regex.Split(File.ReadAllText(chineseDictPath), "dict");
            var dictLines = fileLines.Where((val, i) => i > 1 && i < fileLines.Length - 1).ToArray();

            foreach (var lineText in dictLines)
            {
                var hexString = Regex.Split(lineText, "0x")[1].Split(']')[0];
                var character = char.ConvertFromUtf32(Convert.ToInt32(hexString, 16));

                var pinyinString = lineText.Split('"')[1].Split('"')[0];
                var pinyins = new List<string>();

                var phoneticDictA = "āáǎàēéěèōóǒòīíǐìūúǔùüǘǚǜńň";
                var phoneticDictB = "aaaaeeeeooooiiiiuuuuvvvvnnm";

                foreach (var pinyin in pinyinString.Split(','))
                {
                    var noPhoneticPinyin = pinyin.Select((py) => {
                        var pIndex = phoneticDictA.IndexOf(py.ToString());
                        if (pIndex == -1) return py;
                        else return phoneticDictB[pIndex];
                    }).ToArray();
                    var pyStr = string.Join("", noPhoneticPinyin);
                    if (!pinyins.Contains(pyStr))
                    {
                        pinyins.Add(pyStr);
                    }
                }

                Dict.Add(character, pinyins);
            }
        }
    }
}
