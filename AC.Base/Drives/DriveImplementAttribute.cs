using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 设备驱动功能实现特性。用于实现 IDriveImplement 接口的接口说明具体实现的功能。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class DriveImplementAttribute : System.Attribute
    {
        /// <summary>
        /// 设备驱动功能实现特性。
        /// </summary>
        /// <param name="name">功能名称。</param>
        public DriveImplementAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 功能名称。
        /// </summary>
        public string Name { get; private set; }
    }
}
