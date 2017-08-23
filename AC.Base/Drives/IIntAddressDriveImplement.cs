using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 使用整数作为设备通讯地址。
    /// </summary>
    public interface IIntAddressDriveImplement : IAddressDriveImplement
    {
        /// <summary>
        /// 获取设备通讯地址。
        /// </summary>
        int Address { get; }
    }
}
