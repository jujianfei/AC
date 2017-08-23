using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 微软 Access 数据库图标。
    /// </summary>
    public class AccessDbIcon : IIcon
    {
        #region IIcon 成员

        /// <summary>
        /// 16 * 16 图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get { return Properties.Resources.Access16; }
        }

        /// <summary>
        /// 32 * 32 图标。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get { return Properties.Resources.Access32; }
        }

        #endregion
    }
}
