using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 设备功能实现特性。用于实现 IDeviceImplement 接口的接口说明具体实现的功能。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class DeviceImplementAttribute : System.Attribute
    {
        /// <summary>
        /// 设备功能实现特性。
        /// </summary>
        /// <param name="name">功能名称。</param>
        public DeviceImplementAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 功能名称。
        /// </summary>
        public string Name { get; private set; }
    }
}
