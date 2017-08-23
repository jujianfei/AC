using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 某一插件所使用的图标。
    /// </summary>
    public interface IPluginTypeImage
    {
        /// <summary>
        /// 该类设备使用的 16 * 16 图标。
        /// </summary>
        System.Drawing.Image Icon16 { get; }

        /// <summary>
        /// 该类设备使用的 32 * 32 图标。
        /// </summary>
        System.Drawing.Image Icon32 { get; }
    }
}
