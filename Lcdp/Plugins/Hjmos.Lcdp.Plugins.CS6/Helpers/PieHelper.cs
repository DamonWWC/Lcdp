using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.CS6.Helpers
{
    public class PieHelper
    {

        /// <summary>
        /// 获取序列
        /// </summary>
        /// <param name="pieArc">饼图弧形数据</param>
        /// <param name="title">标题</param>
        /// <param name="color">渐变的颜色</param>
        /// <param name="pointLabel"></param>
        /// <returns></returns>
        public static PieSeries GetSeries(dynamic pieArc, string title, Color[] color, Func<ChartPoint, string> pointLabel)
        {
            Dictionary<string, double> coordinate;

            // 获取线性渐变的起止方向
            coordinate = PieHelper.GetCoordinates(pieArc.StartRadian, pieArc.EndRadian);

            return new PieSeries
            {
                Title = title,  // 标题
                Values = new ChartValues<ObservableValue> { new ObservableValue(pieArc.Value) },    // 饼图的数值
                LabelPoint = pointLabel,
                DataLabels = true,
                StrokeThickness = 0,
                // 填充线性渐变
                Fill = new LinearGradientBrush()
                {
                    GradientStops = new GradientStopCollection() {
                        new GradientStop() { Offset= 0, Color = color[0] },
                        new GradientStop() { Offset = 1, Color = color[1] }
                    },
                    StartPoint = new Point(coordinate["X"], coordinate["Y"]),
                    EndPoint = new Point(coordinate["X2"], coordinate["Y2"])
                }
            };
        }


        /// <summary>
        /// 根据饼图数值获取饼图弧度数据
        /// </summary>
        /// <remarks>
        /// 定义：弧长等于半径的弧，其所对的圆心角为1弧度，根据定义，一周的弧度数为2πr/r=2π
        /// </remarks>
        /// <param name="data">饼图数值</param>
        /// <returns>饼图弧形对应的弧度数据</returns>
        public static dynamic[] Pie(double[] data)
        {
            double pieStartRadian = 0d;             // 饼图起始弧度
            double pieEndRadian = Math.PI * 2d;     // 饼图结束弧度
            double piePadRadian = 0d;               // 饼图设置的间隔弧度？
            int n = data.Length;                    // 饼图数值个数
            double sum = data.Sum();                // 饼图数值总和
            dynamic[] arcs = new dynamic[n];        // 保存饼图的弧形数据
            double startRadian = pieStartRadian;    // 中间变量，表示弧形的起始弧度
            double da = pieEndRadian;               // 总弧度
            double endRadian;                       // 中间变量，表示弧形的结束弧度
            double pad = Math.Min(Math.Abs(da) / n, piePadRadian); // 实际间隔弧度
            double padRadian = pad * (da < 0 ? -1 : 1);      // 实际间隔弧度？（这里可能为负数，应该是为了兼容其他有弧形的图表）


            // 计算每个弧形的弧度
            // 这里计算了一个比例，代表一个值在起始弧度到终止弧度这个区间内，减去间隔弧度后所占的大小
            // k = sum ? (da - n * pa) / sum
            for (int i = 0; i < n; i++)
            {
                double k = sum > 0 ? (da - n * padRadian) / sum : 0;
                endRadian = startRadian + (data[i] > 0 ? data[i] * k : 0) + padRadian;
                arcs[i] = new
                {
                    Index = i,                  // 索引
                    Value = data[i],            // 饼图数值
                    StartRadian = startRadian,  // 弧形起始弧度
                    EndRadian = endRadian,      // 弧形结束弧度
                    PadRadian = padRadian       // 弧形间隔弧度
                };
                startRadian = endRadian;
            }

            return arcs;
        }

        /// <summary>
        /// 计算拟合弧形的线性渐变方向
        /// </summary>
        /// <param name="startRadian">起始弧度</param>
        /// <param name="endRadian">结束弧度</param>
        /// <returns>四个坐标 X,Y,X2,Y2</returns>
        public static Dictionary<string, double> GetCoordinates(double startRadian, double endRadian)
        {
            // 计算扇形最外层的X,Y坐标
            double[] posi = new double[] {
                Math.Sin(startRadian),
                -Math.Cos(startRadian),
                Math.Sin(endRadian),
                -Math.Cos(endRadian)
            };

            // 计算最外层两个顶点坐标的差值
            double dx = posi[2] - posi[0];
            double dy = posi[3] - posi[1];

            // 根据两点坐标的差值计算X,Y,X2,Y2
            return GetLocation(dx, dy);
        }

        /// <summary>
        /// 返回指示线性渐变方向的坐标X,Y,X2,Y2
        /// </summary>
        /// <param name="dx">弧形顶点坐标X方向差值</param>
        /// <param name="dy">弧形顶点坐标Y方向差值</param>
        /// <returns></returns>
        private static Dictionary<string, double> GetLocation(double dx, double dy)
        {
            double tanV = dx / dy;
            // 判断是横向渐变还是纵向渐变
            bool directSign = Math.Abs(tanV) < 1;
            double t = directSign ? tanV : 1 / tanV;

            double sign1 = t > 0 ? 1 : -1;
            double sign2 = dx > 0 ? 1 : -1;
            double sign = directSign ? sign1 * sign2 : sign2;

            // 把整个圆形的坐标映射到了[0-1]之间，0.5,0.5即为圆心坐标
            double[] group1 = new double[] { 0.5d - sign * t / 2, 0.5d + sign * t / 2 };
            // 区分纵向渐变还是横向渐变，获取group中的第三项和第四项的值
            double[] group2 = sign > 0 ? new double[] { 0, 1 } : new double[] { 1, 0 };
            // 把group1，group2合并到一个数组
            double[] group = new double[] { group1[0], group1[1], group2[0], group2[1] };

            string[] keys = directSign ? new string[] { "X", "X2", "Y", "Y2" } : new string[] { "Y", "Y2", "X", "X2" };

            return new Dictionary<string, double>()
            {
                { keys[0], group[0] },
                { keys[1], group[1] },
                { keys[2], group[2] },
                { keys[3], group[3] }
            };
        }
    }
}
