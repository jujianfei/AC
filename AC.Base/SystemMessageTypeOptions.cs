using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    internal enum SystemMessageTypeOptions
    {
        /// <summary>
        /// 自定义报文。
        /// </summary>
        CustomData,

        /// <summary>
        /// 新建设备。数据区为4个字节的设备编号。
        /// </summary>
        DeviceCreated,

        /// <summary>
        /// 修改设备档案。数据区为4个字节的设备编号。
        /// </summary>
        DeviceUpdated,

        /// <summary>
        /// 删除设备。数据区为4个字节的设备编号。
        /// </summary>
        DeviceDeleted,
    }
}
