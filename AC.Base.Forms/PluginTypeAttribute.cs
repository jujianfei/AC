using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 插件特性。
    /// 添加该特性的类表示为系统内有界面的一个插件，该类应实现 IPlugin 接口派生的接口，如IDevicePlugin、IGlobalPlugin等。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 该插件是一个终结插件。通常终结插件用于指明继承的插件在某种设备或某种分类上不可用。
        /// </summary>
        protected PluginTypeAttribute()
        {
            this.IsEnd = true;
        }

        /// <summary>
        /// 以分隔符或分组形式显示的插件分类。
        /// </summary>
        /// <param name="parentType">用于将插件分类的上级插件类型，如当前是根节点插件则可传入 null。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        protected PluginTypeAttribute(Type parentType, int ordinalNumber)
        {
            this.ParentType = parentType;
            this.OrdinalNumber = ordinalNumber;
        }

        /// <summary>
        /// 使用指定图标的子节点插件或插件分类。
        /// </summary>
        /// <param name="name">插件名称</param>
        /// <param name="description">有关该插件详细的说明。</param>
        /// <param name="parentType">用于将插件分类的上级插件类型。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        /// <param name="imageType">该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。</param>
        protected PluginTypeAttribute(string name, string description, Type parentType, int ordinalNumber,Type imageType)
        {
            this.Name = name;
            this.Description = description;
            this.ParentType = parentType;
            this.OrdinalNumber = ordinalNumber;
            this.ImageType = imageType;
        }

        /// <summary>
        /// 插件名称，如果该属性为 null 则此插件是以分隔符或分组形式显示的插件分类。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 开始一个新的分组，菜单之间加一个分隔符"|"
        /// </summary>
        public bool BeginGroup { get; private set; }

        /// <summary>
        /// 插件描述。有关该插件详细的说明。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 用于将插件分类的上级插件类型，如果当前插件为根节点插件则此属性为null。上级插件通常是用于给插件分类的插件。继承的插件无法修改基插件所设置的分类。
        /// </summary>
        public Type ParentType { get; private set; }

        /// <summary>
        /// 相对于当前插件集合内的排序顺序。按照数字从小到大的顺序列出功能项，如果使用默认值则功能项将在所有指定顺序号的功能项后列出。
        /// </summary>
        public int OrdinalNumber { get; private set; }

        /// <summary>
        /// 该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。
        /// </summary>
        public Type ImageType { get; private set; }

        /// <summary>
        /// 该插件是否为终结插件，如果此属性为 true 则表示当前插件不可用。
        /// </summary>
        public bool IsEnd { get; private set; }
    }
}
