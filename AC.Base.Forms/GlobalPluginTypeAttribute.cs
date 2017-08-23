using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 全局插件特性。
    /// 添加该特性的类应实现 IGlobalPlugin 或 IGlobalHtmlPlugin 接口。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GlobalPluginTypeAttribute : PluginTypeAttribute
    {
        /// <summary>
        /// 该插件是一个终结插件。通常终结插件用于指明继承的全局插件在当前系统中不可用。
        /// </summary>
        public GlobalPluginTypeAttribute()
            : base()
        {
        }

        /// <summary>
        /// 以分隔符或分组形式显示的全局插件分类。
        /// </summary>
        /// <param name="parentType">用于将插件分类的上级插件类型，如当前是根节点插件则可传入 null。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        public GlobalPluginTypeAttribute(Type parentType, int ordinalNumber)
            : base(parentType, ordinalNumber)
        {
        }

        /// <summary>
        /// 全局插件特性。
        /// </summary>
        /// <param name="name">插件名称</param>
        /// <param name="description">有关该插件详细的说明。</param>
        /// <param name="parentType">用于将插件分类的上级插件类型。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        /// <param name="imageType">该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。</param>
        /// <param name="forCheckType">通过该属性指向的类检查该插件能否使用，指向的类必须实现 IGlobalCheck 接口，如无需对该插件进行检查可传入 null。</param>
        public GlobalPluginTypeAttribute(string name, string description, Type parentType, int ordinalNumber, Type imageType, Type forCheckType)
            : base(name, description, parentType, ordinalNumber, imageType)
        {
            if (forCheckType != null)
            {
                if (forCheckType.GetInterface(typeof(IGlobalCheck).FullName) != null)
                {
                    this.ForCheckType = forCheckType;
                }
                else
                {
                    throw new Exception("参数 forCheckType 传入的类“" + forCheckType.FullName + "”未实现 IGlobalCheck 接口");
                }
            }
        }

        /// <summary>
        /// 通过该属性指向的类检查该插件能否使用，指向的类必须实现 IGlobalCheck 接口。
        /// </summary>
        public Type ForCheckType { get; private set; }
    }
}
