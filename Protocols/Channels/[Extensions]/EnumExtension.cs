namespace Protocols.Channels
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.ComponentModel;

    public static class EnumExtension
    {
        public static string GetDescription<T>(this T GenericEnum) where T : Enum
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Any())
                {
                    return ((DescriptionAttribute)attributes.ElementAt(0)).Description;
                }
            }

            return GenericEnum.ToString();
        }
    }
}
