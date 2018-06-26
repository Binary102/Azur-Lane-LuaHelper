using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Azurlane
{
    internal static class Utils
    {
        internal static void Command(string argument)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = $"/c {argument}";
                process.StartInfo.WorkingDirectory = PathMgr.Thirdparty();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();
            }
        }

        internal static void ExceptionLogger(string message, Exception exception)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Logs.txt");

            if (File.Exists(path))
                File.WriteAllText(path, string.Empty);

            using (var streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine("=== START =================================================================================");
                streamWriter.WriteLine(message);
                streamWriter.WriteLine($"Date: {DateTime.Now.ToString()}");
                streamWriter.WriteLine($"Exception Message: {exception.Message}");
                streamWriter.WriteLine($"Exception StackTrace: {exception.StackTrace}");
                streamWriter.WriteLine("=== END ===================================================================================");
                streamWriter.WriteLine();
            }
        }
    }
}