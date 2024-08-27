using System;

namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class File
    {
        public int Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 目录ID
        /// </summary>
        public int FolderId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 文件种类
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// 页面文件Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public int CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public int UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 状态标记（0：删除，1：有效）
        /// </summary>
        public int Status { get; set; }
    }
}
