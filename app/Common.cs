using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static Entropy.Attributes;
using static Entropy.Functions;
using static Entropy.SettingsActions;

namespace Entropy
{
    public class Common
    {
        public XDocument doc = XDocument.Load("Entropy.csproj");


        public static string EntropyAssemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
        public static bool EntropyIsLTS = bool.Parse(Assembly.GetEntryAssembly().GetCustomAttribute<IsLTSAttribute>().IsLTS);
        public static int EntropyLTSBuild = Int32.Parse(Assembly.GetEntryAssembly().GetCustomAttribute<LTSBuildAttribute>().LTSBuild);

        public static string EntropyVersionA = EntropyAssemblyVersion.Substring(0, EntropyAssemblyVersion.Length - 2);
        public static string EntropyVersion = Utilities.EntropyGetVersion(EntropyVersionA, EntropyIsLTS, EntropyLTSBuild);
        public const string EntropyName = "Entropy";
    }

    public class Attributes
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        public class IsLTSAttribute : Attribute
        {
            public string IsLTS { get; set; }
            public IsLTSAttribute(string value)
            {
                IsLTS = value;
            }
        }
        public class LTSBuildAttribute : Attribute
        {
            public string LTSBuild { get; set; }
            public LTSBuildAttribute(string value)
            {
                LTSBuild = value;
            }
        }

    }

    public class SettingsCommon
    {
        internal static readonly Dictionary<string, Action<object>> settingsActions = new Dictionary<string, Action<object>>
        {
            { nameof(Settings.AutoStart), value => HandleAutoStart((bool)value) },
            { nameof(Settings.CheckForUpdates), value => HandleCheckForUpdates((bool)value) },
            { nameof(Settings.PHAutoUpdate), value => HandleAutoUpdate((bool)value) }
        };
    }

    public class Commands
    {
        public static Dictionary<string, string[]> _commandsDsc = new() // string[] = ["parameters", "description"]
        {
            ["help"] = ["<mode> / <command>", "Displays help information for all commands or a specific command. <mode>: 'd' for detailed and 's' for short"],
            ["clear"] = ["", "Clears the console screen."],
            ["status"] = ["<mode>", "Lists all running processes. Use `<mode>`: `d` for detailed view, `s` for short summary."],
            ["terminate"] = ["<process.id> | <process.name>", "Stops a process by its ID or name."],
            ["find"] = ["<process.id> | <process.name> / <mode>", "Finds a process by name or ID and shows its status. Use `<mode>`: `s` for strict, `f` for flexible matching."],
            ["getpath"] = ["<process.id> | <process.name>", "Displays the executable path of a specified process by its ID or name."],
            ["suspend"] = ["<process.id> | <process.name>", "Pauses a process by its ID or name."],
            ["unsuspend"] = ["<process.id> | <process.name>", "Resumes a suspended process by its ID or name."],
            ["settings"] = ["<option>", "[red]EXPERIMENTAL[/]Modifies or displays application settings. Use `<key>` and `<value>` to change settings directly or `<option>`: `show` to view all settings."],
            ["check"] = ["<>", "[red]EXPERIMENTAL[/]Should be versatile function for checking various stuff, for now at least it checks for available updates."],
            ["exit"] = ["<>", "Terminates the application."],
        };

        public static Dictionary<string, string[]> _commandsAliases = new()
        {
            ["help"] = ["h", "g"],
            ["clear"] = ["clr", "cls"],
            ["status"] = ["ps", "pl"],
            ["terminate"] = ["kill", "kl", "term"],
            ["find"] = ["f"],
            ["getpath"] = ["gp"],
            ["suspend"] = ["sp", "spnd"],
            ["unsuspend"] = ["uns", "us"],
            ["settings"] = ["s", "set"],
            ["check"] = ["c"],
            ["exit"] = ["ex", "e"],
        };


        public static Dictionary<string, Action<string, string>> _commands = new Dictionary<string, Action<string, string>>()
        {
            ["help"] = HelpFunction,
            ["clear"] = ClearFunction,
            ["status"] = StatusFunction,
            ["terminate"] = TerminateFunction,
            ["find"] = FindFunction,
            ["getpath"] = GetPathFunction,
            ["suspend"] = SuspendFunction,
            ["unsuspend"] = UnsuspendFunction,
            ["settings"] = SettingsFunction,
            ["check"] = CheckFunction,
            ["exit"] = ExitFunction,
        };
    }

    public class Native
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
    }

}
