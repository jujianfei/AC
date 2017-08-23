using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 分类插件。
    /// </summary>
    public class ClassifyPluginType : PluginType
    {
        internal ClassifyPluginType(ApplicationClass application, Type type)
            : base(application, type)
        {
        }

        /// <summary>
        /// 获取继承自 PluginTypeAttribute 的特性。
        /// </summary>
        protected override PluginTypeAttribute PluginTypeAttr
        {
            get { return (PluginTypeAttribute)this.Type.GetCustomAttributes(typeof(PluginTypeAttribute), false)[0]; }
        }

        /// <summary>
        /// 该插件支持的最大同时处理的分类数量（用于在分类列表选定多个分类后判断该插件是否在菜单上显示）。该值默认为1，表示该功能插件只支持一个分类；该值大于1表示该插件可以同时显示或处理不超过该数量的分类；该值为0表示对同时显示或处理的分类数量无限制。
        /// </summary>
        public int ClassifyMaximum
        {
            get
            {
                if (base.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false).Length > 0)
                {
                    ClassifyPluginTypeAttribute attr = base.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false)[0] as ClassifyPluginTypeAttribute;
                    if (attr.ClassifyMaximum < 1)
                    {
                        return 0;
                    }
                    else
                    {
                        return attr.ClassifyMaximum;
                    }
                }

                return 1;
            }
        }

        /// <summary>
        /// 该插件可以使用在哪些类型的分类上。如果该值为null则表示可用在所有分类上。
        /// </summary>
        public Type ForClassifyType
        {
            get
            {
                if (base.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false).Length > 0)
                {
                    ClassifyPluginTypeAttribute attr = base.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false)[0] as ClassifyPluginTypeAttribute;
                    return attr.ForClassifyType;
                }

                return null;
            }
        }

        /// <summary>
        /// 通过该属性指向的类检查该插件能否用于指定的分类，指向的类必须实现IClassifyCheck接口。
        /// </summary>
        public Type ForCheckType
        {
            get
            {
                if (base.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false).Length > 0)
                {
                    ClassifyPluginTypeAttribute attr = base.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false)[0] as ClassifyPluginTypeAttribute;
                    return attr.ForCheckType;
                }

                return null;
            }
        }
    }
}
