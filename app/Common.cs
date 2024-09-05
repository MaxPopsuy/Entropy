using System;
using System.Runtime.InteropServices;
using static Entropy.Functions;

namespace Entropy
{
    public class Common
    {
        public static string EntropyVersion = "1.1.0";
    }

    public class Commands
    {
        public static Dictionary<string, string[]> _commandsDsc = new() // string[] = ["arguments", "description"]
        {
            ["help"] = ["", "shows every command and short description of them"],
            ["clear"] = ["", $"clears console. (aliases: clr, cls)"],
            ["status"] = ["<mode>", "shows processes list. (aliases: ps, pl) (mode: d - for detailed, s - for short)"],
            ["terminate"] = ["<process.id> or <process.name>", "Terminates process by its name or id. (aliases: kill, kl, term)"],
            ["find"] = ["<process.id> or <process.name> / <mode>", "Finds the process with specific name or id and returns its status (mode: s - for strict, f - for flexible)"],
            ["getpath"] = ["<process.id> or <process.name>", "Gets the process path from id or name (aliases: gp)"],
            ["suspend"] = ["<process.id> or <process.name>", "Suspends the process from id or name (aliases: sp, spnd)"],
            ["unsuspend"] = ["<process.id> or <process.name>", "Unsuspends the process from id or name (aliases: uns, us)"]
        };

        public static Dictionary<string, string[]> _commandsAliases = new()
        {
            ["help"] = ["h", "g"],
            ["clear"] = ["clr", "cls"],
            ["status"] = ["ps", "pl"],
            ["terminate"] = ["kill", "kl", "term"],
            ["find"] = [],
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
