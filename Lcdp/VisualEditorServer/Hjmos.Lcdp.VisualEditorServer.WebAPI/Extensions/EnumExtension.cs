using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Extensions
{
    /// <summary>
    /// 枚举静态扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns>返回Enum上特性Description的值，没有则返回Enum的名称</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return Enum.GetName(value.GetType(), value);
            IEnumerable<DescriptionAttribute> attributes = fieldInfo.GetCustomAttributes<DescriptionAttribute>(false);
            if (attributes != null && attributes.Any())
                return attributes.First().Description;

            return Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// 将指定枚举类型的每个项，填充到ICollection
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="collection">被填充的ICollection</param>
        public static void FillWithMembers<T>(this ICollection<T> collection) where T : Enum
        {
            collection.Clear();

            foreach (string name in Enum.GetNames(typeof(T)))
                collection.Add((T)Enum.Parse(typeof(T), name));
        }
    }
}
