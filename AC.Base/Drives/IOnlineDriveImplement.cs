using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 查询指定的设备是否在线
    /// </summary>
    public interface IOnlineDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 获取指定设备是否在线。
        /// </summary>
        /// <param name="drive">查询该设备的在线状态。drive 应是通道下的设备驱动对象，并且使用 TCP 等有状态的通讯方式与通道通讯。</param>
        /// <returns>true：该设备在线；false：该设备不在线。</returns>
        bool GetOnlineState(IDrive drive);

        /// <summary>
        /// 获取指定设备是否在线。
        /// </summary>
        /// <param name="drives">查询该设备的在线状态。drive 应是通道下的设备驱动对象，并且使用 TCP 等有状态的通讯方式与通道通讯。</param>
        /// <returns>返回的数组长度与 drives 长度一致。true：该设备在线；false：该设备不在线。</returns>
        bool[] GetOnlineState(IDrive[] drives);
    }
}
