using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entropy.Commands;

namespace Entropy
{
    internal class Utilities
    {
        public static void EntropyScreen()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("███████╗███╗   ██╗████████╗██████╗  ██████╗ ██████╗ ██╗   ██╗");
            Console.WriteLine("██╔════╝████╗  ██║╚══██╔══╝██╔══██╗██╔═══██╗██╔══██╗╚██╗ ██╔╝");
            Console.WriteLine("█████╗  ██╔██╗ ██║   ██║   ██████╔╝██║   ██║██████╔╝ ╚████╔╝ ");
            Console.WriteLine("██╔══╝  ██║╚██╗██║   ██║   ██╔══██╗██║   ██║██╔═══╝   ╚██╔╝  v - 0.0.0");
            Console.WriteLine("███████╗██║ ╚████║   ██║   ██║  ██║╚██████╔╝██║        ██║   ");
            Console.WriteLine("╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═╝        ╚═╝   ");

            Console.WriteLine("Write h or help for list of commands, welcome to entropy!");

            Console.ResetColor();
        }

        public static void EntropyWaitAnimation()
        {
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
    }
}
