using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 微软 SQL Server 数据库图标。
    /// </summary>
    public class SQLServerDbIcon : IIcon
    {
        #region IIcon 成员

        /// <summary>
        /// 16 * 16 图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get { return Properties.Resources.SQLServer16; }
        }

        /// <summary>
        /// 32 * 32 图标。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get { return Properties.Resources.SQLServer32; }
        }

        #endregion
    }
}
