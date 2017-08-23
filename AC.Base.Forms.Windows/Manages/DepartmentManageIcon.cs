using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;

namespace AC.Base.Forms.Windows.Manages
{
    /// <summary>
    /// 部门管理所使用的图标。
    /// </summary>
    public class DepartmentManageIcon : IIcon
    {
        #region << IIcon 成员 >>

        /// <summary>
        /// 16 * 16 图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get { return Properties.Resources.Department16; }
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
