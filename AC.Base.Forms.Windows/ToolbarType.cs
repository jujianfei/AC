using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 工具栏插件类型描述。
    /// </summary>
    public class ToolbarType
    {
        private WindowsFormApplicationClass m_Application;

        /// <summary>
        /// 工具栏插件类型描述。
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        internal ToolbarType(WindowsFormApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 获取当前插件的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 插件名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((ToolbarItemTypeAttribute)this.Type.GetCustomAttributes(typeof(ToolbarItemTypeAttribute), false)[0]).Name;
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
                return ((ToolbarItemTypeAttribute)this.Type.GetCustomAttributes(typeof(ToolbarItemTypeAttribute), false)[0]).Description;
            }
        }

        /// <summary>
        /// 创建工具栏实例。
        /// </summary>
        /// <returns></returns>
        public System.Windows.Forms.ToolStrip CreateToolbar()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            IToolbar toolbar = objInstance as IToolbar;
            toolbar.SetApplication(this.m_Application);
            return toolbar as ToolStrip;
        }

        private ToolbarItemType.ToolbarItemTypeCollection m_Items;
        /// <summary>
        /// 属于该工具栏的工具栏项类型集合。
        /// </summary>
        public ToolbarItemType.ToolbarItemTypeCollection Items
        {
            get
            {
                if (this.m_Items == null)
                {
                    this.m_Items = new ToolbarItemType.ToolbarItemTypeCollection();
                }
                return this.m_Items;
            }
        }
        

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// 工具栏插件类型描述集合。
        /// </summary>
        public class ToolbarTypeCollection : ReadOnlyCollection<ToolbarType>
        {
            /// <summary>
            /// 初始化工具栏插件类型描述集合。
            /// </summary>
            internal ToolbarTypeCollection()
            {
            }

            /// <summary>
            /// 将工具栏插件类型描述添加到集合中。
            /// </summary>
            /// <param name="item"></param>
            internal new void Add(ToolbarType item)
            {
                base.Add(item);
            }

            /// <summary>
            /// 获取指定索引处的[元素]。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public new ToolbarType this[int index]
            {
                get
                {
                    return base.Items[index];
                }
                internal set
                {
                    base.Items[index] = value;
                }
            }
        }
    }
}
