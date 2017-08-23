using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 账号接口。
    /// SecuritySolution 及 Account 是系统内置的账号安全方案，另外可以通过实现 ISecuritySolution、IAccount 接口并配置默认的安全方案使用其它账号验证方式。
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// 帐号标识
        /// </summary>
        string AccountCode { get; set; }

        /// <summary>
        /// 使用者名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 传入系统中保存的该账号的 XML 配置数据初始化该系统配置对象。
        /// </summary>
        /// <param name="AccountConfig">账号配置</param>
        void SetConfig(System.Xml.XmlNode AccountConfig);

        /// <summary>
        /// 获取该账号配置信息.
        /// </summary>
        /// <returns>账号配置信息</returns>
        System.Xml.XmlNode GetConfig(System.Xml.XmlDocument doc);

        /// <summary>
        /// 更改密码。
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        void UpdatePassword(string oldPassword, string newPassword);

        /// <summary>
        /// 验证当前账号是否有某种类型的权限。
        /// </summary>
        /// <param name="popedomType">需要验证的实现 IPopedom 接口的权限对象类型</param>
        /// <returns>如果验证为拥有部分权限，则应将 Popedom 转为具体类型进行进一步的判断。</returns>
        AccountPopedomValidate PopedomValidate(Type popedomType);
    }
}
