using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 直接向设备发送数据。
    /// </summary>
    public interface ISendDataDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 直接向当前驱动的设备发送数据。
        /// </summary>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">等待回应数据所需要的额外延时（总延时时间 = 上级设备延时时间 + 额外延时时间）。该值为 -1 时表示不等待回应，数据发出即可，此状态下回应数据返回 null；等于 0 时表示不做额外延时等待，等待时间由上级设备决定；大于 0 表示需要等待回应数据的额外延时，单位毫秒(ms)。</param>
        /// <returns>回应的数据。</returns>
        byte[] SendData(byte[] sendData, int delay);
    }
}
