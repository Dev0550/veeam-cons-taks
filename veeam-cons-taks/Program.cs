using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veeam_cons_taks
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            string[] arg = new string[3];
            arg[0] = "notepad++.exe";
            arg[1] = "5";
            arg[2] = "5";
            args = arg;
            if (args.Length != 3)
            {
                Console.WriteLine("Started with arguments");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"Process Name: {args[0]}");
                Console.WriteLine($"Lifetime: {args[1]}");
                Console.WriteLine($"Periodicity: {args[2]}");
            }
            TimerCallback tm = new TimerCallback(GetProccesses);
            Timer timer = new Timer(tm, args, 0, Convert.ToInt32(args[2]) * 1000);

            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }

        public static void GetProccesses(object obj)
        {
            string[] args = (string[])obj;
            int minute = Convert.ToInt32(args[1]);
            string proccessName = args[0].Replace(".exe", "");
            Process[] proc = Process.GetProcessesByName(proccessName.Trim());
            if (proc.Length > 0)
            {
                foreach (var item in proc)
                {
                    try
                    {
                        TimeSpan ts = DateTime.Now.Subtract(item.StartTime);
                        if (ts.Minutes >= minute)
                        {
                            item.Kill();
                            _log.Debug(item.ProcessName + " - task was killed");
                        }
                    }
                    catch (Exception e)
                    {
                        WriteColoredLine($"{e.Message}: {item.ProcessName}", ConsoleColor.Red);
                    }
                }
            }
            else
            {
                Console.WriteLine("Process not found");
            }
        }
        public static void WriteColoredLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

