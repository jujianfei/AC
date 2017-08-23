using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 分类插件特性。
    /// 添加该特性的类应实现 IClassifyPlugin 或 IClassifyHtmlPlugin 接口。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassifyPluginTypeAttribute : PluginTypeAttribute
    {
        /// <summary>
        /// 该插件是一个终结插件。通常终结插件用于指明继承的插件在某种分类上不可用。
        /// </summary>
        public ClassifyPluginTypeAttribute()
            : base()
        {
        }

        /// <summary>
        /// 以分隔符或分组形式显示的全局插件分类。
        /// </summary>
        /// <param name="parentType">用于将插件分类的上级插件类型，如当前是根节点插件则可传入 null。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        public ClassifyPluginTypeAttribute(Type parentType, int ordinalNumber)
            : base(parentType, ordinalNumber)
        {
        }

        /// <summary>
        /// 分类插件特性。
        /// </summary>
        /// <param name="name">插件名称</param>
        /// <param name="description">有关该插件详细的说明。</param>
        /// <param name="parentType">用于将插件分类的上级插件类型。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        /// <param name="imageType">该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。</param>
        /// <param name="classifyMaximum">该插件支持的最大同时处理的分类数量（用于在分类列表选定多个分类后判断该插件是否在菜单上显示）。该值为1时，表示该功能插件只支持一个分类；该值大于1表示该插件可以同时显示或处理不超过该数量的分类；该值为0表示对同时显示或处理的分类数量无限制。</param>
        /// <param name="forClassifyOrCheckType">指定该插件可以在哪种类型的分类上使用或指向插件检查的类（根据类型自动判断是指定分类类型还是分类检查类）。</param>
        public ClassifyPluginTypeAttribute(string name, string description, Type parentType, int ordinalNumber , Type imageType, int classifyMaximum, Type forClassifyOrCheckType)
            : base(name, description, parentType, ordinalNumber, imageType)
        {
            this.ClassifyMaximum = classifyMaximum;

            if (forClassifyOrCheckType != null)
            {
                if (forClassifyOrCheckType.GetInterface(typeof(IClassifyCheck).FullName) != null)
                {
                    this.ForCheckType = forClassifyOrCheckType;
                }
                else
                {
                    this.ForClassifyType = forClassifyOrCheckType;
                }
            }
        }

        /// <summary>
        /// 该插件支持的最大分类数量（用于在分类列表选定多个分类后判断该插件是否在菜单上显示）。该值为1时，表示该功能插件只支持一个分类；该值大于1表示该插件可以同时显示或处理不超过该数量的分类；该值为0表示对同时显示或处理的分类数量无限制。
        /// 该值默认为0，允许不限数量的分类。
        /// </summary>
        public int ClassifyMaximum { get; private set; }

        /// <summary>
        /// 该插件可以使用在哪种类型的分类上。该类型是一个分类类型、分类已实现的接口、分类驱动类型或分类驱动已实现的接口。如果该值为 null 则表示可用在所有分类上。
        /// </summary>
        public Type ForClassifyType { get; private set; }

        /// <summary>
        /// 通过该属性指向的类检查该插件能否用于指定的分类，指向的类必须实现 IClassifyCheck 接口。
        /// </summary>
        public Type ForCheckType { get; private set; }
    }
}
