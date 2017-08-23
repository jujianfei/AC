using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 设备类型特性。每个继承 Device 的非抽象类必须添加该特性，以便描述该设备类型的一些信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DeviceTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 描述继承 Device 的设备的特征。
        /// </summary>
        /// <param name="name">设备类型名。</param>
        /// <param name="sort">设备类型分类。例如“公司名/系列名”，分类之间使用“/”符号分隔。</param>
        /// <param name="description">设备描述。有关该设备详细的说明。</param>
        /// <param name="driveType">该设备使用的设备驱动类。如该设备无驱动，则传入 null。</param>
        /// <param name="parentTypes">该设备必须挂接的上级设备类型、上级设备已实现的接口、上级设备驱动类型、上级设备驱动已实现的接口。如果该设备是通道则传入 null，如果该设备对上级设备类型无任何限制则传入 new Type[] { }，否则指明上级设备类型。</param>
        /// <param name="childrenTypes">该设备允许连接的下级设备类型、下级设备已实现的接口、下级设备驱动类型、下级设备驱动已实现的接口。如果该设备是末端设备不允许再连接子设备则传入 null，如果该设备对下级设备类型无任何限制则传入 new Type[] { }，否则指明下级设备类型。</param>
        /// <param name="imageType">该设备所使用的图标类，指向该类型的类必须实现 IDeviceTypeImage 接口。如不提供图标可传入 null。</param>
        public DeviceTypeAttribute(string name, string sort, string description, Type driveType, Type[] parentTypes, Type[] childrenTypes, Type imageType)
        {
            this.Name = name;
            this.Sort = sort;
            this.Description = description;
            this.DriveType = driveType;
            this.ParentTypes = parentTypes;
            this.ChildrenTypes = childrenTypes;
            this.ImageType = imageType;
        }

        /// <summary>
        /// 设备类型名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 设备分类。例如“公司名/系列名”，分类之间使用“/”符号分隔。
        /// </summary>
        public string Sort { get; private set; }

        /// <summary>
        /// 设备描述。有关该设备详细的说明。
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 该设备使用的设备驱动类。
        /// </summary>
        public Type DriveType { get; private set; }

        /// <summary>
        /// 该设备必须挂接的上级设备类型。
        /// </summary>
        public Type[] ParentTypes { get; private set; }

        /// <summary>
        /// 该设备允许连接的下级设备类型。
        /// </summary>
        public Type[] ChildrenTypes { get; private set; }

        /// <summary>
        /// 该设备所使用的图标类。
        /// </summary>
        public Type ImageType { get; private set; }
    }
}
