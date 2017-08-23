using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 需要当前操作员账号接口。该接口通常配合其它功能一起使用，表示当前功能需要使用操作员账号对象，实现该接口后系统框架会调用 SetAccount 方法传递账号对象。
    /// 可以与 IPlugin、IParameter、IFilter 等接口一起使用。
    /// </summary>
    public interface IUseAccount
    {
        /// <summary>
        /// 设置当前操作员账号。
        /// </summary>
        /// <param name="account"></param>
        void SetAccount(IAccount account);
    }
}
