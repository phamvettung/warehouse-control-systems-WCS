using Intech.FortnaRollerConveyor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.WCS.UI.Logging
{
    public class Logger
    {
        public static void WriteLog(string message, LogLevel logLevel, string logAction)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var filePath = string.Format(@"{0}\logs\", currentDirectory);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var fileName = string.Format(@"{0}\{1}.txt", filePath, DateTime.Now.ToString("yyyy_MM_dd"));

            using (var writer = new StreamWriter(fileName, true))
            {
                lock (writer)
                {
                    writer.WriteLineAsync(string.Format("[{0}] [{1}] {2} {3}", DateTime.Now.ToString("HH:mm:ss"), logLevel, logAction, message));
                }
            }
        }
    }
}
