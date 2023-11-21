using System;
using System.Linq;
using System.Runtime.Serialization;


namespace CounterStrikeSharp.API.Modules.Utils
{
    public static class EnumUtils
    {
        public static string? GetEnumMemberAttributeValue<T>(T enumValue)
        {
            var enumType = typeof(T);

            if(!enumType.IsEnum || enumValue == null) 
            {
                return null;
            }

            var enumString = enumValue.ToString();

            if(string.IsNullOrWhiteSpace(enumString))
            {
                return null;
            }

            var memberInfo = enumType.GetMember(enumString);
            var enumMemberAttribute = memberInfo.FirstOrDefault()?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (enumMemberAttribute != null)
            {
                return enumMemberAttribute.Value;
            }

            return null;
        }
    }
}
