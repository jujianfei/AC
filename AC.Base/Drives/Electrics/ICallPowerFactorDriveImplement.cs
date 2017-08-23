using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 表示该设备可以召测当前实时功率因数数据。
    /// </summary>
    [DriveImplement("召测功率因数")]
    public interface ICallPowerFactorDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测当前实时功率因数。如果是单相用电设备则只有总相位数据有效，否则应根据 allPhase 参数提供总相位及三相分相位数据。
        /// 注：通讯协议有该项数据，但并不表明实际的设备一定实现了该功能，在调用该方法前应判断该设备对应的功能接口上相应属性。
        /// </summary>
        /// <param name="allPhase">是否召测总相位及分相数据。true：需要召测总相位及A、B、C三相数据（如果当前设备为单相供电，则A、B、C三相属性可以不赋值）；false：仅召测总相位数据（如果该设备采用三相供电并且分相数据随总数据可一帧召回，则可以同时返回所有数据，否则无需单独召测分相数据）。</param>
        /// <returns></returns>
        PhaseTotalValue CallPowerFactor(bool allPhase);
    }
}
