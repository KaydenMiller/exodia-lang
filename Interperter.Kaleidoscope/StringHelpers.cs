namespace Interperter.Kaleidoscope;

public static class StringHelpers
{
    public static unsafe sbyte* ToSByte(this string str)
    {
        sbyte[] sbytes = Array.ConvertAll(str.ToArray(), Convert.ToSByte);

        fixed (sbyte* sbytePtr = &sbytes[0])
        {
            return sbytePtr;
        }
    }
}