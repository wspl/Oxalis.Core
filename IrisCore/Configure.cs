using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Iris.Core
{
    class Configure
    {
        public static string AppFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string ResourcesPath = Path.Combine(AppFolderPath, "Resources");
    }
}
