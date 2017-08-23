using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.General
{
    /// <summary>
    /// 表示该设备可以召测当前实时温度数据。
    /// </summary>
    [DriveImplement("召测温度")]
    public interface ICallTemperatureDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测当前实时温度，如温度数据异常则返回 null。
        /// </summary>
        /// <returns></returns>
        decimal? CallTemperature();
    }
}
