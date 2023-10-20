using System;
using Entropy;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;


class Program
{
    //private static Dictionary<string, Action<string, string>> _commands = new Dictionary<string, Action<string, string>>()
    //{
    //    ["help"] = "help",

    //};
    public static void Main(string[] args)
    {
        Console.Title = "Entropy";
        Utilities.EntropyScreen();

        while (true)
        {
            Thread.Sleep(500);
            Console.Write(">>");
            Thread.Sleep(1000);
            Console.Write("\b");
            Console.Write(">>");
            Thread.Sleep(500);
            Console.Write("\b");
            Console.Write(">>");
            Thread.Sleep(500);
            Console.Write("\b");
            Console.Write(": ");

            string? input = Console.ReadLine();
            string?[] output = input?.Split("&") ?? Array.Empty<string>();
            Entropy.Functions.HelpFunction(null, null);

        }
    }
}