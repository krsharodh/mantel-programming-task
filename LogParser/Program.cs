using System;
using System.IO;

namespace LogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: LogParser <logfile>");
                Console.WriteLine("Example: LogParser access.log");
                return;
            }

            string logFilePath = args[0];

            if (!File.Exists(logFilePath))
            {
                Console.WriteLine($"Error: File '{logFilePath}' not found.");
                return;
            }

            var parser = new HttpLogParser();
            var report = parser.ParseAndGenerateReport(logFilePath);

            Console.WriteLine(report);
        }
    }
}
