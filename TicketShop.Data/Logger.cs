using System;
using System.IO;
using System.Text;
using TicketShop.Core;

namespace TicketShop.Data
{
    public class Logger
    {
        private static readonly string logPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
            "Logs");

        private static string logFilePath;
        private static Object _locker = new Object();
        private static Logger _default;

        public static Logger Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Logger();
                }
                return _default;
            }
        }

        private Logger()
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            ClearDirectory(logPath);
            string logFileName = string.Format("{0:yyyy-MM-dd_hh-mm-ss}.txt", DateTime.Now);
            logFilePath = Path.Combine(logPath, logFileName);
            CreateAppLog(logFilePath);
        }

        public void Append(string appAction)
        {
            lock (_locker)
            {
                using (var sw = new StreamWriter(logFilePath, true, Encoding.Unicode))
                {
                    sw.WriteLine("{0} {1}", appAction, DateTime.Now.ToString("F"));
                }
            }
        }

        private void ClearDirectory(string logPath)
        {
            const int MaxFiles = 300;
            var directoryInfo = new DirectoryInfo(logPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            int fileInfosLength = fileInfos.Length;

            if (fileInfosLength > MaxFiles)
            {
                for (int i = MaxFiles; i < fileInfosLength; i++)
                {
                    fileInfos[i - MaxFiles].Delete();
                }
            }
        }

        private void CreateAppLog(string p)
        {
            using (var sw = new StreamWriter(p, false, Encoding.Unicode))
            {
                sw.WriteLine("\t\tTicketshop application log");
                sw.WriteLine("This is an informational event log only. No user action is required.");
                sw.WriteLine("-----------------------------------------------------------------------------");
            }
        }

        public static void Exit()
        {
            var directoryInfo = new DirectoryInfo(logPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            foreach (FileInfo fi in fileInfos)
            {
                Log log = new Log();
                log.LogName = fi.Name;
                using (var streamReader = new StreamReader(fi.FullName))
                {
                    log.LogText = streamReader.ReadToEnd();
                }

                bool result = Access.SendLog(log);
                if (result)
                {
                    fi.Delete();
                }
            }
        }
    }
}