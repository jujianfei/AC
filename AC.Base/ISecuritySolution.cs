using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 账号登陆及密码验证安全方案接口。
    /// SecuritySolution 及 Account 是系统内置的账号安全方案，另外可以通过实现 ISecuritySolution、IAccount 接口并配置默认的安全方案使用其它账号验证方式。
    /// </summary>
    public interface ISecuritySolution
    {
        //获取系统内所有的账号信息
        //GetAccountInfos()

        /// <summary>
        /// 账号验证，登录验证。
        /// </summary>
        /// <param name="accountCode">账号名。</param>
        /// <param name="password">密码。</param>
        /// <returns>如果成功登录则返回账号对象，否则抛出异常。</returns>
        IAccount AccountValidate(string accountCode, string password);


        /// <summary>
        /// 设置应用程序框架
        /// </summary>
        /// <param name="application">应用程序框架</param>
        void SetApplication(ApplicationClass application);
    }
}
