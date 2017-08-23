using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 通道服务属性。每个实现 IChannelService 接口的非抽象类必须添加该属性，以便描述该通道服务的一些信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ChannelServiceTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 通道服务属性。每个实现 IChannelService 接口的非抽象类必须添加该属性，以便描述该通道服务的一些信息。
        /// </summary>
        /// <param name="name">通道服务类型名。</param>
        /// <param name="description">通道服务描述。有关该通道服务详细的说明。</param>
        public ChannelServiceTypeAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// 通道服务类型名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 通道服务描述。有关该通道服务详细的说明。
        /// </summary>
        public string Description { get; private set; }
    }
}
