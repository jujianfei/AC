using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 不存在的设备。如果数据库中保存的某一设备类型不存在，则将使用此类型进行替换。
    /// </summary>
    public class NonexistentDevice : Device
    {
    }
}
