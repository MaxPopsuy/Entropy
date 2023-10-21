using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entropy
{
    class Functions
    {
        public static void HelpFunction(string _, string __)
        {
            foreach (var (key, value) in Commands._commandsDsc)
            {
                //Utilities.EntropyWrite(ConsoleColor.DarkMagenta, $"{key} {value[0]} - {value[1]}");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{key}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" {value[0]}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" - {value[1]}");
                Console.ResetColor();
            }
            Console.WriteLine("\n");
        }

        public static void ClearFunction(string _, string __)
        {
            Console.Clear();
            Utilities.EntropyScreen();
        }

        public static void StatusFunction(string _, string __)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("pID:::pName:::status \n");
            Console.ResetColor();

            var status = Process.GetProcesses();

            foreach (var process in status)
            {
                var isSuspended = false;

                foreach (ProcessThread thread in process.Threads)
                {
                    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
                        thread.WaitReason == ThreadWaitReason.Suspended)
                    {
                        isSuspended = true;
                        break;
                    }
                }

                if (isSuspended)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Suspended");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Working");
                    Console.ResetColor();
                }
            }
            Console.Write("\n");
        }

        public static void TerminateFunction(string argument, string _)
        {
            if (argument != null)
            {
                bool isFinded = false;
                foreach (var process in Process.GetProcesses())
                {
                    try
                    {
                        if (process.Id.ToString() == argument)
                        {
                            process.Kill();
                            Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} was succesfully terminated.");
                            isFinded = true;
                        }
                        if (process.ProcessName == argument)
                        {
                            process.Kill();
                            Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} was succesfully terminated.");
                            isFinded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.EntropyWrite(ConsoleColor.Red, $"Failed to kill process >>> {ex.Message}.");
                        isFinded = true;
                    }
                }
                if (isFinded == false)
                {
                    Utilities.EntropyWrite(ConsoleColor.Red, $"{argument} not found.");
                }
                Console.Write("\n");
            }
            else
            {
                Console.WriteLine("You didn't pass an argument, use the `terminate` command like this: `terminate <process.name>` or `terminate <process.pid>'\n");
            }
        }

        public static void FindFunction(string argument, string _)
        {
            if (string.IsNullOrEmpty(argument))
            {
                Utilities.EntropyWrite(ConsoleColor.Red, "Please provide an argument, either the process id or the process name \n");
                return;
            }
            var processes = Process.GetProcesses();
            var procArray = new List<int>();

            foreach (var process in processes)
            {
                var isSuspended = false;


                foreach (ProcessThread thread in process.Threads)
                {
                    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
                        thread.WaitReason == ThreadWaitReason.Suspended)
                    {
                        isSuspended = true;
                        break;
                    }
                }

                if (isSuspended)
                {
                    int id;
                    if (int.TryParse(argument, out id) && process.Id == id || process.ProcessName == argument)
                    {
                        procArray.Add(id);
                        Utilities.EntropyWrite(ConsoleColor.Red, $"{process.Id}:::{process.ProcessName}:::Suspended");
                    }
                }
                else
                {
                    int id;
                    if (int.TryParse(argument, out id) && process.Id == id || process.ProcessName == argument)
                    {
                        procArray.Add(id);
                        Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName}:::Working");
                    }
                }
            }
            if (procArray.Count == 0)
            {
                Utilities.EntropyWrite(ConsoleColor.Red, $"No processes with this specific id or name were found");
            }
            /*Console.WriteLine("\n");*/
        }

        public static void GetPathFunction(string argument, string _)
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if(process.ProcessName == argument || process.Id.ToString() == argument)
                {
                    try
                    {
                        if (process != null && process.MainModule != null)
                        {
                            Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} > {process.MainModule.FileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.EntropyWrite(ConsoleColor.Red, $"Failed to access path for process: {process.Id}:::{process.ProcessName} >> {ex.Message}");
                    }
                }
            }
        }

        public static void SuspendFunction(string argument, string _)
        {
            
        }
    }
}
