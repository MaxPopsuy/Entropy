﻿using static Entropy.Commands;
using static Entropy.Common;


namespace Entropy
{

    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Entropy";
            Utilities.EntropyScreen(true);

            SettingsManager.InitializeSettings();
            Settings settings = SettingsManager.LoadSettings();

            Task updateCheckTask = UpdateManager.CheckForUpdates(EntropyVersion, settings);
            updateCheckTask.Wait();

            while (true)
            {

                Utilities.EntropyWaitAnimation();

                string? input = Console.ReadLine();
                string?[] output = input?.Split("&") ?? Array.Empty<string>();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;

                foreach (string? raw in output)
                {
                    string[] inputArray = raw?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

                    string? command = null;
                    string? firstArgument = null;
                    string? secondArgument = null;

                    try
                    {
                        command = inputArray[0];
                        firstArgument = inputArray[1];
                        secondArgument = inputArray[2];
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(command))
                    {
                        command = Utilities.EntropyGetCommandFromAlias(command);

                        if (!_commands.TryGetValue(command, out var commandMethod))
                        {
                            Console.WriteLine($"{command} is an unknown command, if you need help with commands, type 'help' \n");
                        }
                        else
                        {
                            if (commandMethod != null)
                            {
                                if (firstArgument != null && secondArgument != null)
                                {
                                    commandMethod.Invoke(firstArgument, secondArgument);
                                }
                                else if (firstArgument != null)
                                {
                                    commandMethod.Invoke(firstArgument, string.Empty);
                                }
                                else if (secondArgument != null)
                                {
                                    commandMethod.Invoke(string.Empty, secondArgument);
                                }
                                else
                                {
                                    commandMethod.Invoke(string.Empty, string.Empty);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
