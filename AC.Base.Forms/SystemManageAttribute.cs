using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 提供给用户使用的系统管理插件特性，该特性只能用于实现 ISystemManage 接口的类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemManageAttribute : System.Attribute
    {
        /// <summary>
        /// 提供给用户使用的系统管理插件特性，该特性只能用于实现 ISystemManage 接口的类。
        /// </summary>
        /// <param name="name">插件名称。</param>
        /// <param name="imageType">该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。</param>
        public SystemManageAttribute(string name, Type imageType)
        {
            this.Name = name;
            this.ImageType = imageType;
        }

        /// <summary>
        /// 插件名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。
        /// </summary>
        public Type ImageType { get; private set; }
    }
}
