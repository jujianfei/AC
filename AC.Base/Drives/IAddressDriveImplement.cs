using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 将设备通讯地址转为字节数组或将字节数组转为设备通讯地址。
    /// </summary>
    public interface IAddressDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 获取设备通讯地址字节数据的表现形式。
        /// </summary>
        /// <returns></returns>
        byte[] GetAddress();

        /// <summary>
        /// 设置字节数据表现形式的设备通讯地址。
        /// </summary>
        /// <param name="address">设备通讯地址。</param>
        void SetAddress(byte[] address);
    }
}
