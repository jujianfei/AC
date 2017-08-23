using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备档案删除项。实现该接口的类将作为删除档案中的一个项目在进行删除操作时执行部分删除逻辑，同时还可以提供自身的操作界面以便在删除操作前进行一些设置。
    /// 如果该档案项不提供任何界面只执行一些逻辑，则不必继承自任何类；如果希望将该项添加到桌面应用程序中则必须从 System.Windows.Forms.Control 继承，如果希望将插件添加到WEB应用中则必须从 System.Web.UI.Control 继承。
    /// 实现该接口的类必须添加 DeviceArchiveItemTypeAttribute 特性，如果需要使用账号可添加 IUseAccount 接口。
    /// </summary>
    public interface IDeviceArchiveDeleteItem
    {
        /// <summary>
        /// 设置应显示或处理的设备。
        /// </summary>
        /// <param name="device"></param>
        void SetDevice(Device device);

        /// <summary>
        /// 操作员点击“删除”按钮后由设备删除界面调用的方法。如果执行删除操作时遇到错误不允许继续删除时，该方法可以抛出 Exception 异常；如果遇到一般性错误只需要提示操作员告知其一些信息但是允许继续删除时，可以抛出 DeviceArchiveWarningException 异常。
        /// </summary>
        void Delete();
    }
}
