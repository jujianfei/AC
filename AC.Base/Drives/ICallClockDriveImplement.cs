using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 召测设备时钟。
    /// </summary>
    [DriveImplement("召测时钟")]
    public interface ICallClockDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测设备时钟。
        /// </summary>
        /// <returns>设备当前时钟。</returns>
        DateTime CallClock();
    }
}
