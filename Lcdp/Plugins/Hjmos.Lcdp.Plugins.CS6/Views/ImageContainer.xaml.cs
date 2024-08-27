using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "图片容器", DefaultWidth = 500d)]
    public partial class ImageContainer
    {
        public ImageContainer()
        {
            InitializeComponent();
            this.ImageSource = BitmapFrame.Create(new Uri(@"pack://SiteOfOrigin:,,,/123.jpg"));
        }

        /// <summary>
        /// 跟属性面板绑定的图片
        /// TODO：设置特性不保存到JSON文件
        /// </summary>
        [SerializeToOption(false)]
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }


        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageContainer), new PropertyMetadata(default(ImageSource), (d, e) =>
            {
                ImageContainer imageContainer = (ImageContainer)d;

                BitmapFrame imageSource = (BitmapFrame)e.NewValue;

                if (imageSource == null) return;

                // WPF路径
                string path = imageSource.Decoder.ToString();

                if (path.EndsWith("123.jpg"))
                {
                    imageContainer.ImageSourceForDisplay = imageSource;
                    return;
                }

                // 获取程序根目录
                string rootPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;

                // 获取扩展名
                string[] array = path.Split(new char[] { '.' });
                string extension = array[array.Length - 1];

                // 生成文件名
                string fileName = $"{DateTime.Now:yyyy-M-d-h-m-s.fff}.{extension}";

                // 目标路径
                string targetPath = Path.Combine(rootPath, "images");

                // 创建目录
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                // 文件完整路径
                string fullName = Path.Combine(targetPath, fileName);

                // 保存图片到本地
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));
                FileStream file = new(fullName, FileMode.Create);
                encoder.Save(file);
                file.Close();

                // 显示到界面上
                imageContainer.ImageSourceForDisplay = BitmapFrame.Create(new Uri(fullName)); ;

            }));// 标记IsOption为false，属性不会被序列化到JSON


        /// <summary>
        /// 显示在界面上的图片
        /// </summary>
        [Browsable(false)]
        public ImageSource ImageSourceForDisplay
        {
            get => (ImageSource)GetValue(ImageSourceForDisplayProperty);
            set => SetValue(ImageSourceForDisplayProperty, value);
        }

        public static readonly DependencyProperty ImageSourceForDisplayProperty =
            DependencyProperty.Register("ImageSourceForDisplay", typeof(ImageSource), typeof(ImageContainer), new PropertyMetadata(default(ImageSource)));
    }
}
