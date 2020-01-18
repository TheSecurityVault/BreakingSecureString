using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DllToInject
{
    //this awesome code was extracted from here: https://stackoverflow.com/a/53029501
    class PointerToObject
    {
       

        [StructLayout(LayoutKind.Explicit)]
        private struct ObjectReinterpreter
        {
            [FieldOffset(0)] public ObjectWrapper AsObject;
            [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
        }

        private class ObjectWrapper
        {
            public object Object;
        }

        private class IntPtrWrapper
        {
            public IntPtr Value;
        }

        ObjectReinterpreter or;

        public PointerToObject(IntPtr pointer)
        {
            or = new ObjectReinterpreter();
            or.AsObject = new ObjectWrapper();
            or.AsIntPtr.Value = pointer;
        }

        public object GetObject()
        {
            return or.AsObject.Object;
        }
    }
}
