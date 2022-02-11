using System;
using System.ComponentModel;
using System.Reflection;

namespace Hero.AutoTrading.Bitkub.Utils
{
    public static class EnumUtil
    {
        public static string GetEnumDescription(Enum enumVal)
        {
            MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            DescriptionAttribute attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);
            return attribute.Description;
        }
    }
}
