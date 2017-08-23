using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 插件信息。
    /// </summary>
    public class PluginTypeInfo
    {
        internal PluginTypeInfo(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// 当前插件类型。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 当前插件所继承的基类插件信息。
        /// </summary>
        public PluginTypeInfo BaseInfo { get; internal set; }

        private PluginTypeInfoInheritCollection m_Inherits;
        /// <summary>
        /// 继承当前插件的派生插件集合。
        /// </summary>
        public PluginTypeInfoInheritCollection Inherits
        {
            get
            {
                if (this.m_Inherits == null)
                {
                    this.m_Inherits = new PluginTypeInfoInheritCollection(this);
                }
                return this.m_Inherits;
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Type.FullName;
        }

        /// <summary>
        /// 派生插件集合，继承自某一插件的派生插件。
        /// </summary>
        public class PluginTypeInfoInheritCollection : ReadOnlyCollection<PluginTypeInfo>
        {
            private PluginTypeInfo m_BaseInfo;

            internal PluginTypeInfoInheritCollection(PluginTypeInfo baseInfo)
            {
                this.m_BaseInfo = baseInfo;
            }

            internal new void Add(PluginTypeInfo pluginTypeInfo)
            {
                pluginTypeInfo.BaseInfo = this.m_BaseInfo;
                base.Add(pluginTypeInfo);
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个继承的插件信息";
            }
        }
    }
}
