using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 桌面应用程序状态栏插件特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StatusBarItemTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 初始化桌面应用程序状态栏插件特性。
        /// </summary>
        /// <param name="name">状态栏插件名称</param>
        /// <param name="description">有关该状态栏插件详细的说明。</param>
        public StatusBarItemTypeAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// 状态栏插件名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 状态栏插件描述。有关该状态栏插件详细的说明。
        /// </summary>
        public string Description { get; private set; }

    }
}
