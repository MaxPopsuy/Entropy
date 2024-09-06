using System;
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
        public static void HelpFunction(string mode, string command)
        {
            Console.ResetColor();
            if (string.IsNullOrWhiteSpace(mode) || mode == "d")
            {
                mode = "detailed";
            }
            if (!string.IsNullOrWhiteSpace(command))
            {
                string fullCommand = Utilities.EntropyGetCommandFromAlias(command);

                if (Commands._commandsDsc.ContainsKey(fullCommand))
                {
                    var commandInfo = Commands._commandsDsc[fullCommand];
                    var aliases = Commands._commandsAliases.ContainsKey(fullCommand)
                        ? string.Join(", ", Commands._commandsAliases[fullCommand])
                        : "None";

                    Console.ResetColor();
                    Panel result;

                    if (mode == "detailed")
                    {
                        result = new Panel(
                            $"[purple]Command:[/] [white]{fullCommand}[/]\n" +
                            $"[purple]Aliases:[/] [yellow1]{aliases}[/]\n" +
                            $"[purple]Parameters:[/] [green1]{(commandInfo[0].Length == 0 ? "-" : commandInfo[0])}[/]\n" +
                            $"[purple]Description:[/] [white]{commandInfo[1]}[/]"
                        )
                        {
                            Border = BoxBorder.Double
                        };
                        result.Header($"[purple]Help: [white]{fullCommand}[/][/]").HeaderAlignment(Justify.Center);
                    }
                    else
                    {
                        result = new Panel(
                            $"[purple]Command:[/] [white]{fullCommand}[/]\n" +
                            $"[purple]Aliases:[/] [yellow1]{aliases}[/]\n" +
                            $"[purple]Parameters:[/] [green1]{(commandInfo[0].Length == 0 ? "-" : commandInfo[0])}[/]"
                        )
                        {
                            Border = BoxBorder.Double
                        };
                        result.Header($"[purple]Help: [white]{fullCommand}[/][/]").HeaderAlignment(Justify.Center);
                    }

                    AnsiConsole.Write(result);
                }
                else
                {
                    Console.WriteLine($"[red]No such command '{command}' found.[/]");
                }
            }
            else
            {
                Table helpTable = new Table().Centered().Expand();
                helpTable.Border = TableBorder.Double;

                helpTable.AddColumn(new TableColumn("COMMAND").Centered());
                helpTable.AddColumn(new TableColumn("PARAMS").Centered());
                helpTable.AddColumn(new TableColumn("ALIASES").Centered());

                if (mode == "detailed")
                {
                    helpTable.AddColumn(new TableColumn("DESCRIPTION").Centered());
                }

                helpTable.Columns[0].Padding(2, 10);
                helpTable.Columns[1].Padding(2, 10);
                helpTable.Columns[2].Padding(2, 10);

                if (mode == "detailed")
                {
                    helpTable.Columns[3].Padding(2, 10);
                }
                else
                {
                    helpTable.Collapse().Alignment(Justify.Left);
                }

                foreach (var (key, value) in Commands._commandsDsc)
                {
                    var aliases = Commands._commandsAliases.ContainsKey(key)
                        ? string.Join(", ", Commands._commandsAliases[key])
                        : "None";

                    if (mode == "detailed")
                    {
                        helpTable.AddRow(
                            $"[white]{key}[/]",
                            $"[green1]{(value[0].Length == 0 ? "-" : value[0])}[/]",
                            $"[yellow1]{aliases}[/]",
                            $"[white]{value[1]}[/]"
                        );
                    }
                    else
                    {
                        helpTable.AddRow(
                            $"[white]{key}[/]",
                            $"[green1]{(value[0].Length == 0 ? "-" : value[0])}[/]",
                            $"[yellow1]{aliases}[/]"
                        );
                    }
                }

                AnsiConsole.Write(helpTable);
            }

            Console.WriteLine();
        }

        public static void ClearFunction(string _, string __)
        {
            Console.Write("\f\u001bc\x1b[3J");
            Utilities.EntropyScreen();
        }

        public static void StatusFunction(string _, string __)
        {
            Console.WriteLine();
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
            Console.WriteLine();
        }

        public static void TerminateFunction(string argument, string _)
        {
            Console.WriteLine();
            if (argument != null)
            {
                bool isFinded = false;
                foreach (var process in Process.GetProcesses())
                {
                    try
                    {

                        if (process.Id.ToString() == argument || process.ProcessName.ToLower() == argument.ToLower())
                        {
                            process.Kill();
                            Console.ResetColor();
                            Panel result = new($"[purple]ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nSTATUS: [green1]Succesfully Terminated[/][/]")
                            {
                                Border = BoxBorder.Rounded
                            };
                            result.HeaderAlignment(Justify.Center);
                            result.Header($"[purple]Terminate: [white]{process.Id}[/][/]");
                            AnsiConsole.Write(result);
                            isFinded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ResetColor();
                        Panel result = new($"[purple]ID: {process.Id}\nNAME:{process.ProcessName}\nSTATUS:[red]Failed to Terminate[/]\n{ex.Message}[/]")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header($"[purple]Terminate: [white]{process.Id}[/][/]");
                        AnsiConsole.Write(result);
                        isFinded = true;
                    }
                }
                if (isFinded == false)
                {
                    Console.ResetColor();
                    Panel result = new($"[purple]ARGUMENT: [white]{(argument == "" ? "You did not pass an argument" : argument)}[/]\nSTATUS: [red]Not Found[/][/]")
                    {
                        Border = BoxBorder.Rounded
                    };
                    result.HeaderAlignment(Justify.Center);
                    result.Header($"[purple]Terminate: [red]ERROR[/][/]");
                    AnsiConsole.Write(result);
                }
            }
            Console.WriteLine();
        }

        public static void FindFunction(string argument, string mode)
        {
            Console.ResetColor();
            var processes = Process.GetProcesses();
            var procArray = new List<int>();

            foreach (var process in processes)
            {
                var isSuspended = false;
                bool processMatches = mode == "s" ? process.ProcessName.Equals(argument)
                                          : process.ProcessName.ToLower().Contains(argument.ToLower());

                if (!processMatches)
                {
                    continue;
                }

                foreach (ProcessThread thread in process.Threads)
                {
                    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
                        thread.WaitReason == ThreadWaitReason.Suspended)
                    {
                        isSuspended = true;
                        break;
                    }
                }

                int id;
                bool isIdMatches = int.TryParse(argument, out id) && process.Id == id;

                if (processMatches || isIdMatches)
                {
                    procArray.Add(process.Id);
                    string statusText = isSuspended ? "[red]Suspended[/]" : "[green1]Working[/]";

                    Panel result = new($"ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nSTATUS: {statusText}\nRAM: [white]{process.PrivateMemorySize64 / 1024 / 1024} Mb[/]")
                    {
                        Border = BoxBorder.Rounded
                    };
                    result.HeaderAlignment(Justify.Center);
                    result.Header($"Find: [white]{process.Id}[/]");
                    AnsiConsole.Write(result);
                }
            }
            if (procArray.Count == 0)
            {
                Console.ResetColor();
                Panel result = new("[red]No processes with this specific ID or name were found[/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("Find Result");
                AnsiConsole.Write(result);
            }

            Console.WriteLine();
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
                        Panel result = new($"ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nERROR: [red]{ex.Message}[/]")
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
                Panel result = new($"ARGUMENT: [white]{argument}[/]\nERROR:[red]Process not found[/]")
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
            Console.WriteLine();
            if (argument == null)
            {
                Console.ResetColor();
                Panel result = new($"[purple]Error: [red]No arguments given, please provide an argument, either [green]<process.id>[/] or [green]<process.name>[/][/][/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("[purple]Suspend: [red]Error[/][/]");
                AnsiConsole.Write(result);
                return;
            }

            var isFinded = false;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id.ToString() == argument || process.ProcessName == argument)
                    {
                        Utilities.EntropySuspendProcess(process.Id);
                        Console.ResetColor();
                        Panel result = new($"[purple]ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nSTATUS: [red]Suspended[/][/]")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header($"[purple]Suspend: [white]{process.Id}[/][/]");
                        AnsiConsole.Write(result);
                        isFinded = true;
                    }

                }
                catch (Exception error)
                {
                    Console.ResetColor();
                    Panel result = new($"[purple]STATUS: [white]Failed to suspend process[/]\nError: [red]{error.Message}[/][/]")
                    {
                        Border = BoxBorder.Rounded
                    };
                    result.HeaderAlignment(Justify.Center);
                    result.Header("[purple]Suspend: [red]Error[/][/]");
                    AnsiConsole.Write(result);
                    isFinded = true;
                }
            }
            if (!isFinded)
            {
                Console.ResetColor();
                Panel result = new($"[purple]Error: [red]Process '{argument}' was not found[/][/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("[purple]Suspend: [red]Error[/][/]");
                AnsiConsole.Write(result);
            }
            Console.WriteLine();
        }

        public static void UnsuspendFunction(string argument, string _)
        {
            Console.WriteLine();
            if (argument == null)
            {
                Console.ResetColor();
                Panel result = new($"[purple]Error: [red]No arguments given, please provide an argument, either [green]<process.id>[/] or [green]<process.name>[/][/][/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("[purple]Suspend: [red]Error[/][/]");
                AnsiConsole.Write(result);
                return;
            }


            var isFinded = false;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id.ToString() == argument || process.ProcessName == argument)
                    {
                        Utilities.EntropyUnsuspendProcess(process.Id);
                        Console.ResetColor();
                        Panel result = new($"[purple]ID: [white]{process.Id}[/]\nNAME: [white]{process.ProcessName}[/]\nSTATUS: [green]Working[/][/]")
                        {
                            Border = BoxBorder.Rounded
                        };
                        result.HeaderAlignment(Justify.Center);
                        result.Header($"[purple]Unsuspend: [white]{process.Id}[/][/]");
                        AnsiConsole.Write(result);
                        isFinded = true;
                    }
                }
                catch (Exception error)
                {
                    Console.ResetColor();
                    Panel result = new($"[purple]STATUS: [white]Failed to suspend process[/]\nError: [red]{error.Message}[/][/]")
                    {
                        Border = BoxBorder.Rounded
                    };
                    result.HeaderAlignment(Justify.Center);
                    result.Header("[purple]Suspend: [red]Error[/][/]");
                    AnsiConsole.Write(result);
                    isFinded = true;
                }
            }
            if (!isFinded)
            {
                Console.ResetColor();
                Panel result = new($"[purple]Error: [red]Process '{argument}' was not found[/][/]")
                {
                    Border = BoxBorder.Rounded
                };
                result.HeaderAlignment(Justify.Center);
                result.Header("[purple]Suspend: [red]Error[/][/]");
                AnsiConsole.Write(result);
            }
            Console.WriteLine();
        }
    }
}
