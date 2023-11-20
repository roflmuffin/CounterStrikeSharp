using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public static class EnumUtils
    {
        public static string GetEnumMemberAttributeValue<T>(T enumValue)
        {
            var enumType = typeof(T);

            var memberInfo = enumType.GetMember(enumValue.ToString());
            var enumMemberAttribute = memberInfo.FirstOrDefault()?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (enumMemberAttribute != null)
            {
                return enumMemberAttribute.Value;
            }

            return null;
        }
    }
}
