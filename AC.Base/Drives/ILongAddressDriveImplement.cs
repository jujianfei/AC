using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 使用长整数作为设备通讯地址。
    /// </summary>
    public interface ILongAddressDriveImplement : IAddressDriveImplement
    {
        /// <summary>
        /// 获取设备通讯地址。
        /// </summary>
        long Address { get; }
    }
}
