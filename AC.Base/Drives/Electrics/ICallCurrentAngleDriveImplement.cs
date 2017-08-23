using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 表示该设备可以召测当前实时电流相位角数据。
    /// </summary>
    [DriveImplement("召测电流相位角")]
    public interface ICallCurrentAngleDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测当前实时电流相位角。
        /// 注：通讯协议有该项数据，但并不表明实际的设备一定实现了该功能，在调用该方法前应判断该设备对应的功能接口上相应属性。
        /// </summary>
        /// <param name="allPhase">是否召测各分相及零相数据。true：需要召测A、B、C三相及零相数据（如果当前设备为单相供电，则A相及零相有值，B、C相属性可以不赋值）；false：仅召测三相数据（如果该设备采用三相供电并且零相数据可一帧召回，则可以同时返回所有数据，否则无需单独召测零相数据）。</param>
        /// <returns></returns>
        PhaseZeroValue CallCurrentAngle(bool allPhase);
    }
}
