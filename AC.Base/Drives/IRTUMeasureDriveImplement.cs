using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// RTU模拟量召测
    /// </summary>
    [DriveImplement("模拟量")]
    public interface IRTUMeasureDriveImplement : IMeasureDriveImplement
    {
        /// <summary>
        /// 定时任务中用来调用召测结果类型值BranchMessage
        /// </summary>
        /// <returns></returns>
        Object returnResult();
    }
}
