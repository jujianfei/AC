using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 设置设备时钟，为设备对时。
    /// </summary>
    public interface ISetClockDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 设置设备时钟，为设备对时。
        /// </summary>
        /// <param name="dateTime">欲设置的时间。</param>
        void SetClock(DateTime dateTime);
    }
}
