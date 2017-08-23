using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.General
{
    /// <summary>
    /// 表示该设备可以召测当前实时湿度数据。
    /// </summary>
    [DriveImplement("召测湿度")]
    public interface ICallHumidityDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测当前实时湿度，如湿度数据异常则返回 null。
        /// </summary>
        /// <returns></returns>
        decimal? CallHumidity();
    }
}
