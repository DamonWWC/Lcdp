using System;

namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    /// <summary>
    /// 组件类库
    /// </summary>
    public class WidgetLib
    {
        public int Id { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件MD5校验码
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// 程序集存放路径
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 状态标记（0：删除，1：有效）
        /// </summary>
        public int Status { get; set; }
    }

}
