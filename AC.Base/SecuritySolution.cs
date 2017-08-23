using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Database;

namespace AC.Base
{
    /// <summary>
    /// 系统默认内置的账号登陆及密码验证安全方案。
    /// </summary>
    public class SecuritySolution : ISecuritySolution
    {
        ApplicationClass m_application;

        #region ISecuritySolution 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_application = application;
        }

        /// <summary>
        /// 账号验证，登录验证。
        /// </summary>
        /// <param name="accountCode">账号名。</param>
        /// <param name="password">密码。</param>
        /// <returns>如果成功登录则返回账号对象，否则抛出异常。</returns>
        public IAccount AccountValidate(string accountCode, string password)
        {
            DbConnection dbConn = m_application.GetDbConnection();
            if (dbConn != null)
            {
                Account account = new Account();
                try
                {
                    string strSql = string.Format("select * from AccountConfig where AccountCode = \"{0}\" and Password = \"{1}\"", accountCode, AC.Base.Function.md5(password));
                    System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                    if (dr.Read())
                    {
                        //IPAddress myIPAddress = (IPAddress)Dns.GetHostAddresses(Dns.GetHostName()).GetValue(0);
                        object objConfig = dr[Tables.AccountConfig.XMLConfig];

                        account.AccountCode = accountCode;
                        account.Popedomlist = new List<Type>();
                        account.PopedomName = string.Empty;

                        if (objConfig != null && !(objConfig is System.DBNull))
                        {
                            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                            try
                            {
                                xmlDoc.LoadXml(objConfig.ToString());
                                if (xmlDoc.ChildNodes.Count > 0)
                                {
                                     foreach (System.Xml.XmlNode xnConfig in xmlDoc.ChildNodes)
                                     {
                                         if (xnConfig.Name.Equals("account"))
                                             account.SetConfig(xnConfig);
                                     }
                                }
                            }
                            catch(Exception)
                            {
                                throw new Exception("没有找到用户正确配置");
                            }
                        }
                    }
                    else
                        throw new Exception("没有查询到当前账号、或用户密码错误");

                    account.PopedomName = "Administrator";

                    //strSql = string.Format("SELECT b.RoleName,b.Description,b.PermissionXML FROM AccountRoles a,Roles b where a.AccountCode = \"{0}\" and a.RID = b.RID", account.AccountCode);
                    //System.Data.IDataReader dr1 = dbConn.ExecuteReader(strSql);
                    //while (dr1.Read())
                    //{
                    //    account.PopedomName = string.Format("{0} , {1}", account.PopedomName, dr1["RoleName"].ToString());
                    //    System.Xml.XmlDocument xdc = new System.Xml.XmlDocument();
                    //    xdc.LoadXml(dr1["PermissionXML"].ToString());
                    //    System.Xml.XmlNode xn = xdc.SelectSingleNode("item");

                    //    if (xn.ChildNodes != null && xn.ChildNodes.Count > 0)
                    //    {
                    //        foreach (System.Xml.XmlNode sxn in xn.ChildNodes)
                    //        {
                    //            Type p = Type.GetType(sxn.InnerText);
                    //            if (!account.Popedomlist.Contains(p))
                    //                account.Popedomlist.Add(p);
                    //        }
                    //    }
                    //}
                    dr.Close();
                    //dr1.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
                return account;
            }
            else
                throw new Exception("数据库连接失败，请检查当前网络环境或联系管理员");
        }

        #endregion
    }
}
