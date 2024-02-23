namespace Interperter.Kaleidoscope;

public static class StringHelpers
{
    public static unsafe sbyte* ConvertToSByte(this string str)
    {
        var sbytes = Convert.ToSByte(str);
        return &sbytes;
    }
}