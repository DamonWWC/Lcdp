namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class Widget
    {
        public int Id { get; set; }

        /// <summary>
        /// 组件类库ID
        /// </summary>
        public int LibId { get; set; }

        /// <summary>
        /// 组件类名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 组件类型全名
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// 是否渲染成样例（0：否，1：是）
        /// </summary>
        public int RenderAsSample { get; set; }

        /// <summary>
        /// 样例组件类型全名
        /// </summary>
        public string SampleFullName { get; set; }
    }
}
