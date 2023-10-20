using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entropy
{
    internal class Commands
    {
        public static Dictionary<string, string> _commandsDsc = new Dictionary<string, string>
        {
            ["help"] = "show every command, and quick description of them",
            ["clear"] = "clear console. (aliases: clr, cls)",
            ["status"] = "(aliases: ps, pl) - shows process list"
        };
    }
}
