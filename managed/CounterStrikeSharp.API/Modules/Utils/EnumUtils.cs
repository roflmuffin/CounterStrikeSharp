using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;


namespace CounterStrikeSharp.API.Modules.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Brute force search using Enum.GetNames as enum members pointing to other enum members do not have the correct attributes.
        /// </summary>
        public static T? GetEnumMemberAttribute<T>(this Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            foreach (var name in Enum.GetNames(type))
            {
                var field = type.GetField(name);
                if (field == null) continue;

                var fieldValue = field.GetValue(null)!;
                if (fieldValue.Equals(enumValue))
                {
                    var attribute = field.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }

            return null;
        }

        public static string? GetEnumMemberAttributeValue<T>(T? enumValue) where T : Enum
        {
            var enumType = typeof(T);

            if (!enumType.IsEnum || enumValue == null)
            {
                return null;
            }

            var enumString = enumValue.ToString();

            if (string.IsNullOrWhiteSpace(enumString))
            {
                return null;
            }

            var memberInfo = enumType.GetMember(enumString);
            var enumMemberAttribute = memberInfo.FirstOrDefault()?.GetCustomAttributes(false)
                .OfType<EnumMemberAttribute>().FirstOrDefault();
            if (enumMemberAttribute != null)
            {
                return enumMemberAttribute.Value;
            }

            // Brute force search by name if we still can't find it.
            return enumValue.GetEnumMemberAttribute<EnumMemberAttribute>()?.Value;
        }
    }
}