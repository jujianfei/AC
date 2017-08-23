using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 插件类型描述。
    /// </summary>
    public abstract class PluginType
    {
        private ApplicationClass m_Application;

        /// <summary>
        /// 插件类型描述。
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        protected PluginType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 获取继承自 PluginTypeAttribute 的特性。
        /// </summary>
        protected abstract PluginTypeAttribute PluginTypeAttr { get; }

        /// <summary>
        /// 获取当前插件的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 该插件的上级分类类型声明。
        /// </summary>
        public Type ParentType
        {
            get
            {
                return this.PluginTypeAttr.ParentType;
            }
        }

        /// <summary>
        /// 插件名。
        /// </summary>
        public string Name
        {
            get
            {
                return this.PluginTypeAttr.Name;
            }
        }

        /// <summary>
        /// 该插件的代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        /// <summary>
        /// 有关该插件详细的说明。
        /// </summary>
        public string Description
        {
            get
            {
                return this.PluginTypeAttr.Description;
            }
        }

        private int m_OrdinalNumber = -1;
        /// <summary>
        /// 相对于当前插件集合内的排序顺序。按照数字从大到小的顺序列出功能项，如果使用默认值则功能项将在所有指定顺序号的功能项后列出。
        /// </summary>
        public int OrdinalNumber
        {
            get
            {
                if (this.m_OrdinalNumber == -1)
                {
                    this.m_OrdinalNumber = this.PluginTypeAttr.OrdinalNumber;
                    if (this.m_OrdinalNumber == 0)
                    {
                        this.m_OrdinalNumber = Int32.MaxValue;
                    }
                }
                return this.m_OrdinalNumber;
            }
        }

        private Type[] m_ParameterTypes;
        /// <summary>
        /// 该插件所需要的参数类型的类型声明集合。
        /// </summary>
        public Type[] ParameterTypes
        {
            get
            {
                if (this.m_ParameterTypes == null)
                {
                    List<Type> lstTypes = new List<Type>();
                    foreach (Type typParameter in this.Type.GetInterfaces())
                    {
                        if (typParameter.GetInterface(typeof(IParameter).FullName) != null)
                        {
                            lstTypes.Add(typParameter);
                        }
                    }

                    this.m_ParameterTypes = new Type[lstTypes.Count];
                    lstTypes.CopyTo(this.m_ParameterTypes);
                }
                return this.m_ParameterTypes;
            }
        }

        /// <summary>
        /// 该插件是否需要分页控件支持。
        /// </summary>
        public bool IsPagination
        {
            get
            {
                return this.Type.GetInterface(typeof(IPagination).FullName) != null;
            }
        }

        /// <summary>
        /// 该插件是否为终结插件，如果此属性为 true 则表示当前插件不可用。
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return this.PluginTypeAttr.IsEnd;
            }
        }

        private bool m_IsLoadIcon16 = false;
        private System.Drawing.Image m_Icon16;
        /// <summary>
        /// 该类插件使用的 16 * 16 图标。如果插件未提供图标则此属性返回null。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get
            {
                if (this.m_IsLoadIcon16 == false)
                {
                    PluginTypeAttribute attr = this.PluginTypeAttr;
                    if (attr.ImageType != null && attr.ImageType.GetInterface(typeof(IIcon).FullName) != null)
                    {
                        System.Reflection.ConstructorInfo ci = attr.ImageType.GetConstructor(new System.Type[] { });
                        object objInstance = ci.Invoke(new object[] { });

                        this.m_Icon16 = ((IIcon)objInstance).Icon16;
                    }

                    this.m_IsLoadIcon16 = true;
                }
                return this.m_Icon16;
            }
        }

        private bool m_IsLoadIcon32 = false;
        private System.Drawing.Image m_Icon32;
        /// <summary>
        /// 该类插件使用的 32 * 32 图标。如果插件未提供图标则此属性返回null。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get
            {
                if (this.m_IsLoadIcon32 == false)
                {
                    PluginTypeAttribute attr = this.PluginTypeAttr;
                    if (attr.ImageType != null && attr.ImageType.GetInterface(typeof(IIcon).FullName) != null)
                    {
                        System.Reflection.ConstructorInfo ci = attr.ImageType.GetConstructor(new System.Type[] { });
                        object objInstance = ci.Invoke(new object[] { });

                        this.m_Icon32 = ((IIcon)objInstance).Icon32;
                    }

                    this.m_IsLoadIcon32 = true;
                }
                return this.m_Icon32;
            }
        }

        /// <summary>
        /// 该插件的上级插件分类。
        /// </summary>
        public PluginType Parent { get; internal set; }

        private PluginTypeCollection m_Children;
        /// <summary>
        /// 该插件的下级插件集合。
        /// </summary>
        public PluginTypeCollection Children
        {
            get
            {
                if (this.m_Children == null)
                {
                    this.m_Children = new PluginTypeCollection(this);
                }
                return this.m_Children;
            }
        }

        /// <summary>
        /// 确定指定的对象是否等于当前的对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PluginType)
            {
                PluginType plugin = obj as PluginType;
                if (plugin.Type.Equals(this.Type))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
