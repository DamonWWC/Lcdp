namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class Parameter
    {
        public int Id { get; set; }

        /// <summary>
        /// 变量名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 使用范围
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 应用ID    
        /// </summary>
        public int AppId { get; set; }
    }
}
