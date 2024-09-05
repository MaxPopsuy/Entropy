﻿using System;
using System.Diagnostics;
using System.Drawing;
using Spectre;
using Spectre.Console;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.Dynamic;
using System.Xml.Linq;

namespace Entropy
{
    class Functions
    {
        public static void HelpFunction(string _, string __)
        {
            Table helpTable = new Table().Centered();
            helpTable.Expand();
            helpTable.Border = TableBorder.Double;

            helpTable.AddColumn(new TableColumn("COMMAND").Centered());
            helpTable.AddColumn(new TableColumn("PARAMS").Centered());
            helpTable.AddColumn(new TableColumn("DESCRIPTION").Centered());
            helpTable.Columns[0].Padding(2, 10);
            helpTable.Columns[1].Padding(2, 10);
            helpTable.Columns[2].Padding(2, 10);

            Console.ResetColor();
            foreach (var (key, value) in Commands._commandsDsc)
            {
                helpTable.AddRow($"[white]{key}[/]", $"[green1]{(value[0].Length == 0 ? "-" : value[0])}[/]", $"[white]{value[1]}[/]");
            }
            AnsiConsole.Write(helpTable);
            Console.WriteLine("\n");
        }

        public static void ClearFunction(string _, string __)
        {
            Console.Clear();
            Utilities.EntropyScreen();
        }

        public static void StatusFunction(string _, string __)
        {
            Console.Write("\n");
            Console.ResetColor();
            var status = Process.GetProcesses();
            var table = new Table().LeftAligned();
            table.Expand();

            table.AddColumn(new TableColumn("id").Centered());
            table.AddColumn(new TableColumn("name").LeftAligned());
            table.AddColumn(new TableColumn("status").Centered());
            table.Columns[0].Padding(4, 2);
            table.Columns[1].Padding(4, 2);
            table.Columns[2].Padding(4, 2);

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
                    table.AddRow(process.Id.ToString(), process.ProcessName.ToString().ToUpper(), "[red]suspended[/]");
                }
                else
                {
                    table.AddRow(process.Id.ToString(), process.ProcessName.ToString().ToUpper(), "[green1]working[/]");
                }
            }
            AnsiConsole.Write(table);
            Console.Write("\n");
        }

        public static void TerminateFunction(string argument, string _)
        {
            Console.Write("\n");
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
                            Console.ResetColor();
                            Panel result = new($"ID:{process.Id}\nNAME:{process.ProcessName}\nSTATUS:[green1]Succesfully Terminated[/]")
                            {
                                Border = BoxBorder.Rounded
                            };
                            result.HeaderAlignment(Justify.Center);
                            result.Header($"Terminate: {process.Id}");
                            AnsiConsole.Write(result);
                            isFinded = true;
                        }
                        if (process.ProcessName.ToLower() == argument.ToLower())
                        {
                            process.Kill();
                            Console.ResetColor();
                            Panel result = new($"ID:{process.Id}\nNAME:{process.ProcessName}\nSTATUS:[green1]Succesfully Terminated[/]")
                            {
                                Border = BoxBorder.Rounded
                            };
                            result.HeaderAlignment(Justify.Center);
                            result.Header($"Terminate: {process.Id}");
                            AnsiConsole.Write(result);
                            isFinded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ResetColor();
                        Panel result = new($"ID: {process.Id}\nNAME:{process.ProcessName}\nSTATUS:[red]Failed to Terminate[/]\n{ex.Message}")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header($"Terminate: {process.Id}");
                        AnsiConsole.Write(result);
                        isFinded = true;
                    }
                }
                if (isFinded == false)
                {
                    Console.ResetColor();
                    Panel result = new($"ARGUMENT:{argument}\nSTATUS:[red]Not Found[/]")
                    {
                        Border = BoxBorder.Rounded
                    };
                    result.HeaderAlignment(Justify.Center);
                    result.Header($"Terminate: [red]ERROR[/]");
                    AnsiConsole.Write(result);
                }
                Console.Write("\n");
            }
            else
            {
                Console.WriteLine("You didn't pass an argument, use the `terminate` command like this: `terminate <process.name>` or `terminate <process.pid>'\n");
            }
            Console.Write("\n");
        }

        public static void FindFunction(string argument, string _)
        {
            Console.Write("\n");
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
                    if (int.TryParse(argument, out id) && process.Id == id || process.ProcessName.ToLower().Contains(argument.ToLower()))
                    {
                        procArray.Add(id);

                        Console.ResetColor();
                        Panel result = new($"ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nSTATUS: [red]Suspended[/]\nRAM: [white]{process.PrivateMemorySize64 / 1000 / 1024} Mb[/]")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header($"Find: [white]{process.Id}[/]");
                        AnsiConsole.Write(result);
                    }
                }
                else
                {
                    int id;
                    if (int.TryParse(argument, out id) && process.Id == id || process.ProcessName.ToLower().Contains(argument.ToLower()))
                    {
                        procArray.Add(id);

                        Console.ResetColor();
                        Panel result = new($"ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nSTATUS: [green1]Working[/]\nRAM: [white]{process.PrivateMemorySize64 / 1000 / 1024} Mb[/]")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header($"Find: [white]{process.Id}[/]");
                        AnsiConsole.Write(result);
                    }
                }
            }
            if (procArray.Count == 0)
            {
                Console.ResetColor();
                Panel result = new("[red]No processes with this specific id or name were found[/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("Find Result");
                AnsiConsole.Write(result);
            }
            Console.WriteLine("\n");
        }

        public static void GetPathFunction(string argument, string _)
        {
            Console.WriteLine();
            var processes = Process.GetProcesses();
            bool isFinded = false;

            foreach (var process in processes)
            {
                if (process.ProcessName.ToLower() == argument.ToLower() || process.Id.ToString() == argument)
                {
                    try
                    {
                        if (process != null && process.MainModule != null)
                        {
                            Console.ResetColor();
                            Panel result = new($"ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nPATH: [green1]{process.MainModule.FileName}[/]")
                            {
                                Border = BoxBorder.Rounded
                            };
                            result.HeaderAlignment(Justify.Center);
                            result.Header($"GetPath: [white]{process.Id}[/]");
                            AnsiConsole.Write(result);
                            isFinded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ResetColor();
                        Panel result = new($"ID:{process.Id}\nNAME:{process.ProcessName}\nERROR: [red]{ex.Message}[/]")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header("GetPath: [red]Error[/]");
                        AnsiConsole.Write(result);
                        isFinded = true;
                    }
                }
            }
            if (!isFinded)
            {
                Console.ResetColor();
                Panel result = new($"ARGUMENT: {argument}\nERROR:[red]Process not found[/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("GetPath: [red]Error[/]");
                AnsiConsole.Write(result);
            }
            Console.WriteLine();
        }

        public static void SuspendFunction(string argument, string _)
        {
            Console.Write("\n");
            if (argument == null)
            {
                Utilities.EntropyWrite(ConsoleColor.Red, "No arguments given, please provide an argument, either <process.id> or <process.name>");
                return;
            }

            var isFinded = false;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id.ToString() == argument)
                    {
                        Utilities.EntropySuspendProcess(int.Parse(argument));
                        Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} >> suspended");
                        isFinded = true;
                    }
                    if (process.ProcessName == argument)
                    {
                        Utilities.EntropySuspendProcess(process.Id);
                        Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} >> suspended");
                        isFinded = true;
                    }
                }
                catch (Exception error)
                {
                    Utilities.EntropyWrite(ConsoleColor.Red, $"Failed to suspend process >> {error.Message}");
                    isFinded = true;
                }
            }
            if (!isFinded)
            {
                Utilities.EntropyWrite(ConsoleColor.Red, $"Process '{argument}' was not found");
            }
            Console.Write("\n");
        }

        public static void UnsuspendFunction(string argument, string _)
        {
            Console.Write("\n");
            if (argument == null)
            {
                Utilities.EntropyWrite(ConsoleColor.Red, "No arguments given, please provide an argument, either <process.id> or <process.name>");
                return;
            }

            var isFinded = false;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id.ToString() == argument)
                    {
                        Utilities.EntropyUnsuspendProcess(int.Parse(argument));
                        Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} >> unsuspended");
                        isFinded = true;
                    }
                    if (process.ProcessName == argument)
                    {
                        Utilities.EntropyUnsuspendProcess(process.Id);
                        Utilities.EntropyWrite(ConsoleColor.Green, $"{process.Id}:::{process.ProcessName} >> unsuspended");
                        isFinded = true;
                    }
                }
                catch (Exception error)
                {
                    Utilities.EntropyWrite(ConsoleColor.Red, $"Failed to unsuspend process >> {error.Message}");
                    isFinded = true;
                }
            }
            if (!isFinded)
            {
                Utilities.EntropyWrite(ConsoleColor.Red, $"Process '{argument}' was not found");
            }
            Console.Write("\n");
        }
    }
}
