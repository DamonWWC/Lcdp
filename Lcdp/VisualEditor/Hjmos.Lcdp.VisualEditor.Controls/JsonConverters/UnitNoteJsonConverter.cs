using Hjmos.Lcdp.VisualEditor.Controls.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using Newtonsoft.Json.Linq;
using System;

namespace Hjmos.Lcdp.VisualEditor.Controls.JsonConverters
{
    /// <summary>
    /// UnitNode对象的Json转换器
    /// </summary>
    public class UnitNoteJsonConverter : BaseJsonConverter<IUnitNode>
    {
        protected override IUnitNode Create(Type objectType, JObject jObject)
        {
            // 将JSON序列化成NodeType枚举指示的对象
            if (FieldExists("NodeType", jObject, out NodeType nodeType))
            {
                switch (nodeType)
                {
                    case NodeType.Layer:
                        return new LayerNode();
                    case NodeType.Region:
                        return new UnitNode();
                    case NodeType.Root:
                        return new RootNode();
                    case NodeType.Widget:
                    case NodeType.PageContainer:
                        return new UnitNode();
                    default:
                        return new UnitNode();
                }
            }
            else
            {
                return new UnitNode();
            }
        }
        private bool FieldExists(string fieldName, JObject jObject, out NodeType nodeType)
        {
            nodeType = jObject[fieldName] == null ? NodeType.Widget : (NodeType)Enum.Parse(typeof(NodeType), jObject[fieldName].ToString());
            return jObject[fieldName] != null;
        }
    }
}
