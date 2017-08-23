using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 设备驱动功能实现接口。该接口附加在实现 IDrive 接口的设备驱动上，通常用于对某一通用功能进行规范，例如透传直抄功能的接口，设备对时功能的接口等。
    /// 实现该接口的“功能实现”如果可以向用户提供搜索实现该功能的所有设备驱动的功能，则可以添加 DriveImplementAttribute 属性。
    /// </summary>
    public interface IDriveImplement : IDrive
    {
    }
}
