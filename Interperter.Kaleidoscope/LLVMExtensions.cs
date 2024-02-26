using LLVMSharp.Interop;

namespace Interperter.Kaleidoscope;

public static unsafe class LLVMExtensions
{
    public static LLVMOpaqueValue* ToArrayPointer(this LLVMOpaqueValue[] array)
    {
        fixed (LLVMOpaqueValue* start = &array[0])
        {
            return start;
        }
    }

    public static LLVMOpaqueType* ToArrayPointer(this LLVMOpaqueType[] array)
    {
        fixed (LLVMOpaqueType* start = &array[0])
        {
            return start;
        }
    }
}