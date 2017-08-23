﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 桌面应用程序工具栏项插件特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolbarItemTypeAttribute: System.Attribute
    {
        /// <summary>
        /// 初始化桌面应用程序工具栏项插件特性。
        /// </summary>
        /// <param name="name">工具栏项插件名称</param>
        /// <param name="description">有关该工具栏项插件详细的说明。</param>
        public ToolbarItemTypeAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// 工具栏项插件名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 工具栏项插件描述。有关该工具栏项插件详细的说明。
        /// </summary>
        public string Description { get; private set; }

    }
}