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
            var everything = new Everything();
            var a = everything.Search("test");

            var cjk = new CJKTranslator();
            var testString = "网易云的 casdf 210123 测试";

            var rs = cjk.Translate(testString);
        }
        
    }
}
