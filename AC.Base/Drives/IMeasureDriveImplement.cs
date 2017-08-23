using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 模拟量召测
    /// </summary>
    [DriveImplement("模拟量")]
    public interface IMeasureDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测RTU模拟量
        /// </summary>
        /// <returns>返回数组的长度值等于 CallMeasure() </returns>
        Boolean CallMeasure();
    }
}
