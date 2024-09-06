using System;
using System.Reflection;
using System.Runtime.InteropServices;
using static Entropy.Functions;

namespace Entropy
{
    public class Common
    {
        public static string EntropyAssemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
        public static string EntropyVersion = EntropyAssemblyVersion.Substring(0, EntropyAssemblyVersion.Length - 2);
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
            ["unsuspend"] = ["<process.id> | <process.name>", "Resumes a suspended process by its ID or name."]
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
            ["unsuspend"] = ["uns", "us"]
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
