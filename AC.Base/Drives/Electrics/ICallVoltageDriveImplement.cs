using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 表示该设备可以召测当前实时电压数据。
    /// </summary>
    [DriveImplement("召测电压")]
    public interface ICallVoltageDriveImplement : IDriveImplement
    {
        /// <summary>
        /// 召测当前实时电压。如果是单相用电设备则A相数据有效，B、C相属性不使用，否则应提供三相分相位数据。
        /// 注：通讯协议有该项数据，但并不表明实际的设备一定实现了该功能，在调用该方法前应判断该设备对应的功能接口上相应属性。
        /// </summary>
        /// <returns></returns>
        PhaseValue CallVoltage();
    }
}
