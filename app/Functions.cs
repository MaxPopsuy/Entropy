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
using System.Text;

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

                helpTable.AddColumn(new TableColumn("COMMAND").Centered()).Alignment(Justify.Center);
                helpTable.AddColumn(new TableColumn("PARAMS").Centered());
                helpTable.AddColumn(new TableColumn("ALIASES").Centered());
                helpTable.Alignment(Justify.Center);

                if (mode == "detailed")
                {
                    helpTable.AddColumn(new TableColumn("DESCRIPTION").Centered());
                    helpTable.Alignment(Justify.Center);
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
        public static void SettingsFunction(string _, string __)
        {
            SettingsManager.InitializeSettings();
            var settings = SettingsManager.LoadSettings();
            Console.ResetColor();

            while (true)
            {
                AnsiConsole.Write(
                    new Panel($"[purple]Current Settings:\n\n{GetSettingsDisplay(settings)}[/]")
                    .Header("[white]Entropy Settings[/]")
                    .Border(BoxBorder.Square).HeaderAlignment(Justify.Center)
                );

                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("[purple]Select a setting to change:[/]")
                    .AddChoices(GetSettingsNames())
                    .HighlightStyle(Spectre.Console.Color.Fuchsia)
                    .PageSize(15)
                );


                if (choice == "Exit" | choice == "")
                {
                    break;
                }

                var property = typeof(Settings).GetProperty(choice);
                if (property == null)
                {
                    AnsiConsole.MarkupLine($"[red]Setting '{choice}' not found.[/]");
                    continue;
                }

                var settingType = property.PropertyType;

                if (settingType.IsEnum)
                {
                    var enumType = settingType;
                    var enumValues = Enum.GetNames(enumType);
                    UpdateEnumSetting(settings, choice, enumValues, enumType);
                }
                else
                {
                    switch (Type.GetTypeCode(settingType))
                    {
                        case TypeCode.Boolean:
                            UpdateBoolSetting(settings, choice);
                            break;

                        case TypeCode.Int32:
                            UpdateIntSetting(settings, choice);
                            break;

                        case TypeCode.String:
                            UpdateStringSetting(settings, choice);
                            break;

                        default:
                            AnsiConsole.MarkupLine($"[red]Unsupported setting type '{settingType.Name}' for setting '{choice}'.[/]");
                            break;
                    }
                }


                SettingsManager.SaveSettings(settings);
                AnsiConsole.MarkupLine("[green]Settings updated successfully![/]");
            }
            Console.WriteLine();
        }

        private static string GetSettingsDisplay(Settings settings)
        {
            var display = new StringBuilder();
            foreach (var prop in typeof(Settings).GetProperties())
            {
                var value = prop.GetValue(settings);
                var displayName = prop.Name;

                display.AppendLine($"{displayName}: [white]{value}[/]");
            }
            return display.ToString();
        }


        private static IEnumerable<string> GetSettingsNames()
        {
            var settingsNames = new List<string>();

            foreach (var prop in typeof(Settings).GetProperties())
            {
                var tags = GetPropertyTags(prop.Name);
                settingsNames.Add($"{tags}{prop.Name}");
            }

            settingsNames.Add("");
            settingsNames.Add("Exit");

            return settingsNames;
        }

        private static void UpdateEnumSetting(Settings settings, string settingName, string[] enumValues, Type enumType)
        {
            var choices = enumValues.Select((value, index) => $"{index}: {value}").Append("").Append("Exit").ToArray();
            var selectedEnumValue = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[purple]Select new value for '{settingName}':[/]")
                    .AddChoices(choices)
                    .HighlightStyle(Spectre.Console.Color.Fuchsia)
            );

            if (selectedEnumValue == "Exit" | selectedEnumValue == "")
            {
                return;
            }

            if (int.TryParse(selectedEnumValue.Split(":")[0], out var index) && index >= 0 && index < enumValues.Length)
            {
                var enumName = enumValues[index];
                if (Enum.TryParse(enumType, enumName, true, out var newEnumValue))
                {
                    var property = typeof(Settings).GetProperty(settingName);
                    property?.SetValue(settings, newEnumValue);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Invalid enum value '{enumName}' for setting '{settingName}'.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Invalid selection '{selectedEnumValue}' for setting '{settingName}'.[/]");
            }
        }

        private static void UpdateBoolSetting(Settings settings, string settingName)
        {
            var choices = new[] { "True", "False", "", "Exit" };
            var currentValue = (bool)typeof(Settings).GetProperty(settingName)?.GetValue(settings);
            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[purple]Enable {settingName}?[/]")
                    .AddChoices(choices)
                    .PageSize(4)
                    .HighlightStyle(Spectre.Console.Color.Fuchsia)
            );

            if (selectedChoice == "Exit" | selectedChoice == "")
            {
                return;
            }

            var newValue = selectedChoice == "True";
            typeof(Settings).GetProperty(settingName)?.SetValue(settings, newValue);
        }

        private static void UpdateIntSetting(Settings settings, string settingName)
        {
            var currentValue = (int)typeof(Settings).GetProperty(settingName)?.GetValue(settings);
            var newValue = AnsiConsole.Prompt(
                new TextPrompt<int>($"[purple]Enter new value for {settingName}:[/]").DefaultValue(currentValue)
            );

            typeof(Settings).GetProperty(settingName)?.SetValue(settings, newValue);
        }

        private static void UpdateStringSetting(Settings settings, string settingName)
        {
            var currentValue = (string)typeof(Settings).GetProperty(settingName)?.GetValue(settings);
            var newValue = AnsiConsole.Prompt(
                new TextPrompt<string>($"[purple]Enter new value for {settingName}:[/]").DefaultValue(currentValue)
            );

            typeof(Settings).GetProperty(settingName)?.SetValue(settings, newValue);
        }
    }
}
