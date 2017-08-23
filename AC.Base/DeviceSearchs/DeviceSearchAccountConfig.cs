using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 用于应用程序及 WEB 程序设备搜索列表中账户默认的搜索配置。
    /// </summary>
    public class DeviceSearchAccountConfig : IAccountConfig
    {
        private ApplicationClass m_Application;
        private IAccount m_Account;

        private bool m_UseDefaultConfig = true;
        /// <summary>
        /// 是否使用系统默认的搜索配置。
        /// </summary>
        public bool UseDefaultConfig
        {
            get
            {
                return this.m_UseDefaultConfig;
            }
            set
            {
                this.m_UseDefaultConfig = value;
            }
        }


        private List<string> m_SearchFilterNames;
        /// <summary>
        /// 搜索筛选器名称及类型，名称和类型之间使用“@”分隔。
        /// </summary>
        public List<string> SearchFilterNames
        {
            get
            {
                if (this.m_SearchFilterNames == null)
                {
                    this.m_SearchFilterNames = new List<string>();
                }
                return this.m_SearchFilterNames;
            }
        }

        private List<string> m_SearchListItemNames;
        /// <summary>
        /// 列表项名称及类型，名称和类型之间使用“@”分隔。
        /// </summary>
        public List<string> SearchListItemNames
        {
            get
            {
                if (this.m_SearchListItemNames == null)
                {
                    this.m_SearchListItemNames = new List<string>();
                }
                return this.m_SearchListItemNames;
            }
        }

        private List<string> m_SearchOrderNames;
        /// <summary>
        /// 排序器类型名称。
        /// </summary>
        public List<string> SearchOrderNames
        {
            get
            {
                if (this.m_SearchOrderNames == null)
                {
                    this.m_SearchOrderNames = new List<string>();
                }
                return this.m_SearchOrderNames;
            }
        }

        private int m_PageSize = 30;
        /// <summary>
        /// 每页显示的记录数。
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.m_PageSize;
            }
            set
            {
                if (value < 5)
                {
                    this.m_PageSize = 5;
                }
                else if (value > 1000)
                {
                    this.m_PageSize = 1000;
                }
                else
                {
                    this.m_PageSize = value;
                }
            }
        }

        /// <summary>
        /// 是否显示子设备。
        /// </summary>
        public bool CanChildren { get; set; }

        #region IAccountConfig 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;
        }

        /// <summary>
        /// 设置当前操作员账号。
        /// </summary>
        /// <param name="account">操作员账号。</param>
        public void SetAccount(IAccount account)
        {
            this.m_Account = account;
        }

        /// <summary>
        /// 传入系统中保存的该账号的 XML 配置数据初始化该系统配置对象。
        /// </summary>
        /// <param name="deviceConfig"></param>
        public void SetConfig(System.Xml.XmlNode deviceConfig)
        {
            foreach (System.Xml.XmlNode xnItem in deviceConfig.ChildNodes)
            {
                switch (xnItem.Name)
                {
                    case "UseDefaultConfig":
                        this.UseDefaultConfig = Function.ToBool(xnItem.InnerText);
                        break;

                    case "SearchFilterNames":
                        foreach (System.Xml.XmlNode xnSearchFilterName in xnItem.ChildNodes)
                        {
                            this.SearchFilterNames.Add(xnSearchFilterName.InnerText);
                        }
                        break;

                    case "SearchListItemNames":
                        foreach (System.Xml.XmlNode xnSearchListItemName in xnItem.ChildNodes)
                        {
                            this.SearchListItemNames.Add(xnSearchListItemName.InnerText);
                        }
                        break;

                    case "PageSize":
                        this.PageSize = Function.ToInt(xnItem.InnerText);
                        break;

                    case "CanChildren":
                        this.CanChildren = Function.ToBool(xnItem.InnerText);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取当前系统配置对象 XML 形式的配置数据。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnUseDefaultConfig = xmlDoc.CreateElement("UseDefaultConfig");
            xnUseDefaultConfig.InnerText = Function.BoolToByte(this.UseDefaultConfig).ToString();
            xnConfig.AppendChild(xnUseDefaultConfig);

            System.Xml.XmlNode xnSearchFilterNames = xmlDoc.CreateElement("SearchFilterNames");
            foreach (string strSearchFilterName in this.SearchFilterNames)
            {
                System.Xml.XmlNode xnSearchFilterName = xmlDoc.CreateElement("SearchFilterName");
                xnSearchFilterName.InnerText = strSearchFilterName;
                xnSearchFilterNames.AppendChild(xnSearchFilterName);
            }
            xnConfig.AppendChild(xnSearchFilterNames);

            System.Xml.XmlNode xnSearchListItemNames = xmlDoc.CreateElement("SearchListItemNames");
            foreach (string strSearchListItemName in this.SearchListItemNames)
            {
                System.Xml.XmlNode xnSearchListItemName = xmlDoc.CreateElement("SearchListItemName");
                xnSearchListItemName.InnerText = strSearchListItemName;
                xnSearchListItemNames.AppendChild(xnSearchListItemName);
            }
            xnConfig.AppendChild(xnSearchListItemNames);

            System.Xml.XmlNode xnPageSize = xmlDoc.CreateElement("PageSize");
            xnPageSize.InnerText = this.PageSize.ToString();
            xnConfig.AppendChild(xnPageSize);

            System.Xml.XmlNode xnCanChildren = xmlDoc.CreateElement("CanChildren");
            xnCanChildren.InnerText = Function.BoolToByte(this.CanChildren).ToString();
            xnConfig.AppendChild(xnCanChildren);

            return xnConfig;
        }

        #endregion
    }
}
