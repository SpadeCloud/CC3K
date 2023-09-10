using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace cc3k.Attributes
{
    public static class AttributesHelper
    {
        public static TAttribute GetAttribute<TAttribute, TValue>(this TValue value)
            where TValue : Enum 
            where TAttribute : Attribute
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type type = value.GetType();
            MemberInfo[] members = type.GetMember(value.ToString());

            if (members.Length == 0)
                throw new ArgumentException($"Member not found for value: {value}", nameof(value));

            MemberInfo member = members.First();
            TAttribute? stats = member.GetCustomAttribute<TAttribute>();
            if (stats == null)
                throw new ArgumentException($"Attribute not found for value: {value}", nameof(value));

            return stats;
        }
    }
}
