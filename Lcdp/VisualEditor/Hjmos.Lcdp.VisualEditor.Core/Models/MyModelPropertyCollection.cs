using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 表示设计项属性的集合。
    /// </summary>
    internal sealed class MyModelPropertyCollection : DesignItemPropertyCollection
    {
        private readonly MyDesignItem _item;
        private readonly Dictionary<string, MyModelProperty> _propertiesDictionary = new();

        public MyModelPropertyCollection(MyDesignItem item)
        {
            _item = item;
        }

        public override DesignItemProperty GetProperty(string name)
        {
            if (_propertiesDictionary.TryGetValue(name, out MyModelProperty property))
                return property;

            property = new MyModelProperty(_item, name);
            _propertiesDictionary.Add(name, property);
            return property;
        }

        public override DesignItemProperty GetAttachedProperty(Type ownerType, string name) => new MyModelProperty(_item, name);

        public override IEnumerator<DesignItemProperty> GetEnumerator()
        {
            foreach (MyModelProperty value in _propertiesDictionary.Values)
            {
                yield return value;
            }
        }
    }
}
