using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Entropy.Commands;
using static Entropy.Native;

namespace Entropy
{
    internal class Utilities
    {
        public static void EntropyScreen(bool help = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"███████╗███╗   ██╗████████╗██████╗  ██████╗ ██████╗ ██╗   ██╗");
            Console.WriteLine($"██╔════╝████╗  ██║╚══██╔══╝██╔══██╗██╔═══██╗██╔══██╗╚██╗ ██╔╝");
            Console.WriteLine($"█████╗  ██╔██╗ ██║   ██║   ██████╔╝██║   ██║██████╔╝ ╚████╔╝ ");
            Console.WriteLine($"██╔══╝  ██║╚██╗██║   ██║   ██╔══██╗██║   ██║██╔═══╝   ╚██╔╝  v - {Common.EntropyVersion}");
            Console.WriteLine($"███████╗██║ ╚████║   ██║   ██║  ██║╚██████╔╝██║        ██║   ");
            Console.WriteLine($"╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═╝        ╚═╝   ");

            Console.WriteLine(help ? "Write h or help for list of commands, welcome to entropy!" : null);

            Console.ResetColor();
        }

        public static void EntropyWaitAnimation()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(">>");
            Thread.Sleep(50);
            Console.Write("\b");
            Console.Write(">>");
            Thread.Sleep(50);
            Console.Write("\b");
            Console.Write(">>");
            Thread.Sleep(50);
            Console.Write("\b");
            Console.Write(": ");
            Console.ResetColor();
        }

        public static string EntropyGetCommandFromAlias(string command)
        {
            foreach (KeyValuePair<string, string[]> entry in _commandsAliases)
            {
                if (entry.Value.Contains(command))
                {
                    return entry.Key;
                }
            }
            return command;
        }

        public static void EntropyWrite(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void EntropySuspendProcess(int processId)
        {
            var process = Process.GetProcessById(processId);

            foreach (ProcessThread processThread in process.Threads)
            {
                IntPtr openThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)processThread.Id);
                    
                if (openThread == IntPtr.Zero)
                {
                    continue;
                }

                _ = SuspendThread(openThread);

                CloseHandle(openThread);
            }
        }

        public static void EntropyUnsuspendProcess(int processId)
        {
            var process = Process.GetProcessById(processId);

            if (process.ProcessName == string.Empty)
            {
                return;
            }

            foreach (ProcessThread processThread in process.Threads)
            {
                IntPtr processOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)processThread.Id);

                if (processOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                int suspendCount;
                do
                {
                    suspendCount = ResumeThread(processOpenThread);
                } while (suspendCount > 0);

                CloseHandle(processOpenThread);
            }
        }
    }
}
