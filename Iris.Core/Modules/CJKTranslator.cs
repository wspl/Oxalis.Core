using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Iris.Core.Modules
{
    public class CJKTranslator
    {
        public Dictionary<string, List<string>> Dict = new Dictionary<string, List<string>>();

        public CJKTranslator()
        {
            ParseChinesePinyin();
        }

        private void MultiTranslate(ref List<string> tResult, string left, string right = "")
        {
            if (left.Length > 0)
            {
                var lastChar = left.Substring(left.Length - 1);

                if (Dict.ContainsKey(lastChar))
                {
                    foreach (var pho in Dict[lastChar])
                    {
                        MultiTranslate(ref tResult, left.Remove(left.Length - 1), " " + pho + " " + right);
                    }
                }
                else
                {
                    MultiTranslate(ref tResult, left.Remove(left.Length - 1), lastChar + right);;
                }
            }
            else
            {
                tResult.Add(new Regex(@"\s+").Replace(right, " "));
            }
        }

        public List<string> TranslateToList(string input)
        {
            var tResult = new List<string>();
            MultiTranslate(ref tResult, input);
            return tResult;
        }

        public string TranslateToString(string input)
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
            var chineseDictPath = Path.Combine(Configuration.ResourcesPath, "CJK", "dict_chinese.js");
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
                    var noPhoneticPinyin = pinyin.Select(py =>
                    {
                        var pIndex = phoneticDictA.IndexOf(py.ToString(), StringComparison.Ordinal);
                        return pIndex == -1 ? py : phoneticDictB[pIndex];
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
