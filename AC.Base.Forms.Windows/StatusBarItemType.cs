using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 状态栏插件类型描述。
    /// </summary>
    public class StatusBarItemType
    {
        private WindowsFormApplicationClass m_Application;

        /// <summary>
        /// 状态栏插件类型描述。
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        internal StatusBarItemType(WindowsFormApplicationClass application, Type type)
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
                return ((StatusBarItemTypeAttribute)this.Type.GetCustomAttributes(typeof(StatusBarItemTypeAttribute), false)[0]).Name;
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
                return ((StatusBarItemTypeAttribute)this.Type.GetCustomAttributes(typeof(StatusBarItemTypeAttribute), false)[0]).Description;
            }
        }

        /// <summary>
        /// 创建状态栏实例。
        /// </summary>
        /// <returns></returns>
        public System.Windows.Forms.ToolStripItem CreateStatusBarItem()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            IStatusBarItem statusbarItem = objInstance as IStatusBarItem;
            statusbarItem.SetApplication(this.m_Application);
            return statusbarItem as ToolStripItem;
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
        /// 状态栏插件类型描述集合。
        /// </summary>
        public class StatusBarItemTypeCollection : ReadOnlyCollection<StatusBarItemType>
        {
            /// <summary>
            /// 初始化状态栏插件类型描述集合。
            /// </summary>
            internal StatusBarItemTypeCollection()
            {
            }

            /// <summary>
            /// 将状态栏插件类型描述添加到集合中。
            /// </summary>
            /// <param name="item"></param>
            internal new void Add(StatusBarItemType item)
            {
                base.Add(item);
            }

            /// <summary>
            /// 获取指定索引处的[元素]。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public new StatusBarItemType this[int index]
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
