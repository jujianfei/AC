using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 数据库特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 数据库特性。
        /// </summary>
        /// <param name="name">数据库类型名。</param>
        /// <param name="description">数据库描述。</param>
        /// <param name="imageType">该数据库所使用的图标类。指向该类型的类必须实现 IIcon 接口。</param>
        public DbTypeAttribute(string name, string description, Type imageType)
        {
            this.Name = name;
            this.Description = description;
            this.ImageType = imageType;
        }

        /// <summary>
        /// 数据库类型名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 数据库描述。
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 该数据库所使用的图标类。
        /// </summary>
        public Type ImageType { get; private set; }
    }
}
