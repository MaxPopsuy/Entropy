using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Entropy.Functions;

namespace Entropy
{
    public class Commands
    {
        public static Dictionary<string, string[]> _commandsDsc = new() // string[] = ["arguments", "description"]
        {
            ["help"] = ["", "shows every command and short description of them"],
            ["clear"] = ["", "clears console. (aliases: clr, cls)"],
            ["status"] = ["", "shows processes list. (aliases: ps, pl)"],
            ["terminate"] = ["<proccess.id> or <process.name>", "Terminates process by its name or id. (aliases: kill, kl, term)"]
        };

        public static Dictionary<string, string[]> _commandsAliases = new()
        {
            ["help"] = ["h", "help", "g"],
            ["clear"] = ["clr", "cls", "clear"],
            ["status"] = ["ps", "pl", "status"],
            ["terminate"] = ["kill, terminate", "kl", "term"]
        };


        public static Dictionary<string, Action<string, string>> _commands = new Dictionary<string, Action<string, string>>()
        {
            ["help"] = HelpFunction,
            ["clear"] = ClearFunction,
            ["status"] = StatusFunction,
            ["terminate"] = TerminateFunction,
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
