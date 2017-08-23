using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 提供 16*16 和 32*32 两种尺寸的图标。
    /// </summary>
    public interface IIcon
    {
        /// <summary>
        /// 16 * 16 图标。
        /// </summary>
        System.Drawing.Image Icon16 { get; }

        /// <summary>
        /// 32 * 32 图标。
        /// </summary>
        System.Drawing.Image Icon32 { get; }
    }
}
