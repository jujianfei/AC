using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 可以对设备进行遍历枚举、使用索引器获取设备、根据设备 ID 获取设备的接口。
    /// </summary>
    public interface IDeviceCollection : IReadOnlyCollection<Device>
    {
        /// <summary>
        /// 获取该集合中指定 ID 的设备。
        /// </summary>
        /// <param name="deviceId">设备 ID。</param>
        /// <returns>返回集合中指定 ID 的设备，如果未查找到该设备则返回 null。</returns>
        Device GetDevice(int deviceId);

        /// <summary>
        /// 获取该集合中符合筛选条件的设备集合。
        /// </summary>
        /// <param name="filters">设备筛选条件。</param>
        /// <returns></returns>
        DeviceCollection GetDevices(DeviceSearchs.DeviceFilterCollection filters);

        /// <summary>
        /// 获取当前集合内所有的设备编号。
        /// </summary>
        /// <returns></returns>
        int[] GetIdForArray();

        /// <summary>
        /// 获取当前集合内所有的设备编号。
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.ICollection<int> GetIdForCollection();

        /// <summary>
        /// 获取当前集合内以逗号分隔的字符串形式的设备编号。
        /// </summary>
        /// <returns>以逗号分隔的字符串形式的设备编号。</returns>
        string GetIdForString();

        /// <summary>
        /// 获取当前集合内以指定字符分隔的字符串形式的设备编号。
        /// </summary>
        /// <param name="separator">分隔各设备编号的字符。</param>
        /// <returns>以指定字符分隔的字符串形式的设备编号。</returns>
        string GetIdForString(string separator);
    }
}
