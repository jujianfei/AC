using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备档案查看项。实现该接口的类将作为查看设备档案中的一项，在查看设备档案时将该项输出的 HTML 加载在界面上。
    /// 实现该接口的类必须添加 DeviceArchiveItemTypeAttribute 特性，如果需要使用账号可添加 IUseAccount 接口。
    /// </summary>
    public interface IDeviceArchiveItem
    {
        /// <summary>
        /// 设置应显示或处理的设备。
        /// </summary>
        /// <param name="device"></param>
        void SetDevice(Device device);

        /// <summary>
        /// 向页面输出HTML。
        /// </summary>
        /// <param name="output">字符输出对象，调用 WriteLine 方法向界面输出 HTML 内容。</param>
        void WriterHtml(System.IO.TextWriter output);
    }
}
