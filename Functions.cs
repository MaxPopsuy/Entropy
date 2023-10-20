using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entropy.Commands;

namespace Entropy
{
    class Functions
    {
        public static void HelpFunction(string? argument, string? argument2)
        {
            foreach (var (key, value) in Commands._commandsDsc)
            {
                Console.WriteLine($">>$>>: {key} - {value}");
            }

        }
    }
}
