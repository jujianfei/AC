using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 操作员账号。
    /// </summary>
    public class Account : IAccount
    {
        #region IAccount 成员
        /// <summary>
        /// 账号标识
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// 使用者名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// 英文名
        /// </summary>
        public string EName { get; set; }

        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }

        
        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }
        
        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LastIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string LastDateTime { get; set; }


        /// <summary>
        /// 权限名称
        /// </summary>
        public string PopedomName { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<Type> Popedomlist { get; set; }

        /// <summary>
        /// 获取账号配置信息。
        /// </summary>
        /// <returns>账号配置信息</returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument doc)
        {
            System.Xml.XmlNode xn = doc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnFirstName = doc.CreateElement("FirstName");
            xnFirstName.InnerText = this.FirstName;
            xn.AppendChild(xnFirstName);

            System.Xml.XmlNode xnLastName = doc.CreateElement("LastName");
            xnLastName.InnerText = this.LastName;
            xn.AppendChild(xnLastName);


            System.Xml.XmlNode xnEName = doc.CreateElement("EName");
            xnEName.InnerText = this.EName;
            xn.AppendChild(xnEName);

            System.Xml.XmlNode xnMobile = doc.CreateElement("Mobile");
            xnMobile.InnerText = this.Mobile;
            xn.AppendChild(xnMobile);

            System.Xml.XmlNode xnLastIP = doc.CreateElement("LastIP");
            xnLastIP.InnerText = this.LastIP;
            xn.AppendChild(xnLastIP);

            System.Xml.XmlNode xnLastDateTime = doc.CreateElement("LastDateTime");
            xnLastDateTime.InnerText = this.LastDateTime;
            xn.AppendChild(xnLastDateTime);

            return xn;
        }

          /// <summary>
        /// 设置数据库配置。
        /// </summary>
        /// <param name="config">数据库的配置信息</param>
        public void SetConfig(System.Xml.XmlNode config)
        {
            foreach (System.Xml.XmlNode xnItem in config.ChildNodes)
            {
                switch (xnItem.Name)
                {
                    case "AccountCode":
                        this.AccountCode = xnItem.InnerText;
                        break;

                    case "FirstName":
                        this.FirstName = xnItem.InnerText;
                        break;

                    case "LastName":
                        this.LastName = xnItem.InnerText;
                        break;

                    case "EName":
                        this.EName = xnItem.InnerText;
                        break;

                    case "Email":
                        this.Email = xnItem.InnerText;
                        break;

                    case "Mobile":
                        this.Mobile = xnItem.InnerText;
                        break;

                    case "LastIP":
                        this.LastIP = xnItem.InnerText;
                        break;

                    case "LastDateTime":
                        this.LastDateTime = xnItem.InnerText;
                        break;
                }
            }
        }

        /// <summary>
        /// 更改密码。
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        public void UpdatePassword(string oldPassword, string newPassword)
        {
        }

        /// <summary>
        /// 验证当前账号是否有某种类型的权限。
        /// </summary>
        /// <param name="popedomType">需要验证的实现 IPopedom 接口的权限对象类型</param>
        /// <returns>如果验证为拥有部分权限，则应将 Popedom 转为具体类型进行进一步的判断。</returns>
        public AccountPopedomValidate PopedomValidate(Type popedomType)
        {
            //简易的系统,菜单不需要权限验证
            return new AccountPopedomValidate(AccountPopedomValidateOptions.Succeed, null);

            //如果需要权限验证，那么角色登录时请添加角色权限
            //if (this.Popedomlist.Contains(popedomType) || this.PopedomName.Contains("Administrator"))
            //    return new AccountPopedomValidate(AccountPopedomValidateOptions.Succeed, null);
            //else
            //    return new AccountPopedomValidate(AccountPopedomValidateOptions.Loser, null); 
        }

        #endregion

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.AccountCode;
        }
    }
}
