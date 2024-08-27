using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    public class DimensionJsonHelper
    {

        /// <summary>
        /// 获取标准二维表中某一维度的值列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">维度字段</param>
        /// <param name="jArray">标准二维表JSON</param>
        /// <param name="orderby">排序</param>
        /// <param name="desc">是否倒序</param>
        /// <returns></returns>
        public static T[] GetSingleDimensionArray<T>(string field, JArray jArray, Func<T, T> orderby = null, bool desc = false)
        {
            IEnumerable<T> array = jArray.Select<dynamic, T>(x => x[field]).Distinct();

            if (orderby != null)
            {
                array = desc ? array.OrderByDescending(orderby) : array.OrderBy(orderby);
            }

            return array.ToArray();
        }

        /// <summary>
        /// 获取标准二维表的所有维度
        /// TODO：数据源必须是二维表，后续加入变体二维表的支持
        /// </summary>
        /// <param name="jArray">标准二维表JSON</param>
        /// <returns></returns>
        public static string[] GetDimensions(JArray jArray) => (jArray.First() as JObject).Properties().Select(x => x.Name).ToArray();

        /// <summary>
        /// 给定一个维度值列表，获取另一个维度的序列
        /// </summary>
        /// <param name="labelField">标签字段</param>
        /// <param name="labelArray">标签字段维度值列表</param>
        /// <param name="seriesField">序列字段</param>
        /// <param name="valueField">值字段</param>
        /// <param name="jArray">标准二维表JSON</param>
        /// <returns></returns>
        public static Dictionary<string, Array> GetSeriesCollection(string labelField, string[] labelArray, string seriesField, string valueField, JArray jArray)
        {
            Dictionary<string, Array> dic = new();

            // 获取序列字段值列表（即图例列表）
            string[] series = GetSingleDimensionArray<string>(seriesField, jArray, x => x);

            foreach (string value in series)
            {
                IEnumerable<JToken> list = jArray.Where(x => x.ToObject<dynamic>()[seriesField] == value);
                // 按照labelArray的顺序排序对应的值列表
                list = list.OrderBy(x => Array.IndexOf(labelArray, x.ToObject<dynamic>()[labelField]));

                dic.Add(value, list.Select<dynamic, double>(x => x[valueField]).ToArray());
            }

            return dic;
        }

    }
}
