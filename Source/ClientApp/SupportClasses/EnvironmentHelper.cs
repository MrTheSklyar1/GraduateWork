using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.SupportClasses
{
    public static class EnvironmentHelper
    {
        public static void SendLog(string log)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + log);
            }
        }
    }
}
