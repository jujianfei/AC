using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows.Manages
{
    /// <summary>
    /// 机构管理所使用的图标。
    /// </summary>
    public class OrganizationConfigIcon : IIcon
    {
        #region IIcon 成员

        /// <summary>
        /// 16 * 16 图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get { return Properties.Resources.Organization16; }
        }

        /// <summary>
        /// 32 * 32 图标。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get { return Properties.Resources.Organization32; }
        }

        #endregion
    }
}
