using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    public static class PrintoutMessage
    {
        public static void Print(MessageSource source, string message)
        {
            Console.WriteLine($"{source} - [{DateTime.Now}] {message}");
        }
    }
}
