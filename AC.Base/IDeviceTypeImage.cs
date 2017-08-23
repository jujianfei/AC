using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 某一设备类型所使用的图标。
    /// </summary>
    public interface IDeviceTypeImage : IIcon
    {
        /// <summary>
        /// 该类设备的外观照片。
        /// </summary>
        System.Drawing.Image Photo { get; }

        /// <summary>
        /// 该类设备不同状态下的图标 如果没有返回null
        /// </summary>
        System.Collections.Generic.Dictionary<DeviceStateOptions, System.Drawing.Image> stateIcon16 { get; }
    }
}
