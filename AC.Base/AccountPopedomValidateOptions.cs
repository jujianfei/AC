using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 帐号权限验证选项。
    /// </summary>
    public enum AccountPopedomValidateOptions
    {
        /// <summary>
        /// 权限验证成功。Popedom 属性为 null。
        /// </summary>
        Succeed,

        /// <summary>
        /// 拥有部分权限。可使用此权限但有限制，具体设置在 Popedom 的属性中。
        /// </summary>
        Partially,

        /// <summary>
        /// 权限验证未通过。Popedom 属性为 null。
        /// </summary>
        Loser,
    }
}
