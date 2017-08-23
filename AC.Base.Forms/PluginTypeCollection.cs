using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 插件类型集合。
    /// </summary>
    public class PluginTypeCollection : ReadOnlyCollection<PluginType>
    {
        private PluginType Parent;

        internal PluginTypeCollection(PluginType parent)
        {
            this.Parent = parent;
        }

        internal new void Add(PluginType item)
        {
            item.Parent = this.Parent;

            for (int intIndex = 0; intIndex < this.Count; intIndex++)
            {
                if (item.OrdinalNumber < this[intIndex].OrdinalNumber)
                {
                    base.Items.Insert(intIndex, item);
                    return;
                }
            }

            base.Items.Add(item);
        }

        internal PluginType FindParent(Type parentType)
        {
            foreach (PluginType _PluginType in this)
            {
                if (Function.IsInheritableBaseType(_PluginType.Type, parentType))
                {
                    return _PluginType;
                }
                else if (_PluginType.Children.Count > 0)
                {
                    PluginType parent = _PluginType.Children.FindParent(parentType);
                    if (parent != null)
                    {
                        return parent;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个插件";
        }
    }
}
