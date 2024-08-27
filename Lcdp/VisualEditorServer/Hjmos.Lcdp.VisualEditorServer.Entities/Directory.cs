using System;

namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class Directory
    {
        public int Id { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上级目录
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 目录等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 所有上级目录
        /// </summary>
        public string ParentIds { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

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
