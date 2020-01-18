using System;
using System.Security;

namespace BreakingSecureString
{
    class Program
    {
        static void Main(string[] args)
        {
            SecureString ss = new SecureString();
            ss.AppendChar('T');
            ss.AppendChar('h');
            ss.AppendChar('e');
            ss.AppendChar('S');
            ss.AppendChar('e');
            ss.AppendChar('c');
            ss.AppendChar('u');
            ss.AppendChar('r');
            ss.AppendChar('i');
            ss.AppendChar('t');
            ss.AppendChar('y');
            ss.AppendChar('V');
            ss.AppendChar('a');
            ss.AppendChar('u');
            ss.AppendChar('l');
            ss.AppendChar('t');

            ss.MakeReadOnly();

            Console.WriteLine("Generated SecureString object");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
