using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 角色权限接口。
    /// 
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// 标识
        /// </summary>
        int RID { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        string R_RoleName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        string R_Description { get; set; }

        /// <summary>
        /// xml
        /// </summary>
        string U_PermissionXML { get; set; }

    }
}
