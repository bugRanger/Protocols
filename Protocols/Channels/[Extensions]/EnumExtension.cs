namespace Protocols.Channels
{
    using System;

    public static class EnumExtension
    {
        public static string Pack<T>(this T GenericEnum) where T : Enum
        {
            return GenericEnum.ToString();
        }

        public static T Unpack<T>(this string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
