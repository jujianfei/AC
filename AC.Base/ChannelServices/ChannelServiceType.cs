using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 通道服务类型。
    /// </summary>
    public class ChannelServiceType
    {
        internal ChannelServiceType(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// 获取当前通道服务的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 通道服务类型名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((ChannelServiceTypeAttribute)this.Type.GetCustomAttributes(typeof(ChannelServiceTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 通道服务类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        /// <summary>
        /// 通道服务描述。有关该通道服务详细的说明。
        /// </summary>
        public string Description
        {
            get
            {
                return ((ChannelServiceTypeAttribute)this.Type.GetCustomAttributes(typeof(ChannelServiceTypeAttribute), false)[0]).Description;
            }
        }

        /// <summary>
        /// 获取当前类型通道服务的新实例。
        /// </summary>
        /// <returns>当前类型通道服务的新实例。</returns>
        public IChannelService CreateChannelService()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            IChannelService channelService = objInstance as IChannelService;
            return channelService;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// 通道服务类型集合。
    /// </summary>
    public class ChannelServiceTypeCollection : ReadOnlyCollection<ChannelServiceType>
    {
        internal ChannelServiceTypeCollection()
        {
        }

        internal new void Add(ChannelServiceType item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取指定类型名的通道服务类型。
        /// </summary>
        /// <param name="typeName">类型名称，如“命名空间1.命名空间2.设备类名”</param>
        /// <returns>通道服务类型。如无对应的通道服务类型则返回 null。</returns>
        public ChannelServiceType GetChannelServiceType(string typeName)
        {
            foreach (ChannelServiceType serviceType in this)
            {
                if (serviceType.Type.FullName.Equals(typeName))
                {
                    return serviceType;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型名的通道服务类型。
        /// </summary>
        /// <param name="type">类型声明。</param>
        /// <returns>通道服务类型。如无对应的通道服务类型则返回 null。</returns>
        public ChannelServiceType GetChannelServiceType(Type type)
        {
            foreach (ChannelServiceType serviceType in this)
            {
                if (serviceType.Type.Equals(type))
                {
                    return serviceType;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个通道服务类型";
        }
    }
}
