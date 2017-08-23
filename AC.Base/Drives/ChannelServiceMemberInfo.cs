using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace AC.Base.Drives
{
    /// <summary>
    /// 主备通道组或多主通道组中某一个通道组成员的地址、端口、连接密码信息。
    /// </summary>
    public class ChannelServiceMemberInfo
    {
        /// <summary>
        /// 通道服务成员所在计算机的 IP 地址(单个主机可以有多个 IP 地址)。
        /// </summary>
        public IPAddress[] Addresses { get; set; }

        /// <summary>
        /// 通道服务成员所监听的端口。
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// 与该通道服务成员连接时所需要使用的密码。
        /// </summary>
        public string Password { get; set; }

        private int m_MaxConnection = 200;
        /// <summary>
        /// 该通道服务允许同一后台并发连接的最大数量。
        /// </summary>
        public int MaxConnection
        {
            get
            {
                return this.m_MaxConnection;
            }
            set
            {
                this.m_MaxConnection = value;
            }
        }

        private int m_Delay;
        /// <summary>
        /// 该通道服务网络及处理数据的总延时时间。
        /// </summary>
        public int Delay
        {
            get
            {
                return this.m_Delay;
            }
            set
            {
                this.m_Delay = value;
            }
        }

        /// <summary>
        /// 设置通道服务成员配置信息。
        /// </summary>
        /// <param name="deviceConfig"></param>
        public void SetConfig(System.Xml.XmlNode deviceConfig)
        {
            foreach (System.Xml.XmlNode xnItem in deviceConfig.ChildNodes)
            {
                switch (xnItem.Name)
                {
                    case "Addresses":
                        this.Addresses = new IPAddress[xnItem.ChildNodes.Count];
                        for (int intIndex = 0; intIndex < xnItem.ChildNodes.Count; intIndex++)
                        {
                            this.Addresses[intIndex] = IPAddress.Parse(xnItem.ChildNodes[intIndex].InnerText);
                        }
                        break;

                    case "Port":
                        this.Port = ushort.Parse(xnItem.InnerText);
                        break;

                    case "Password":
                        this.Password = xnItem.InnerText;
                        break;

                    case "MaxConnection":
                        this.MaxConnection = Function.ToInt(xnItem.InnerText);
                        break;

                    case "Delay":
                        this.Delay = Function.ToInt(xnItem.InnerText);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取通道服务成员配置信息。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnAddresses = xmlDoc.CreateElement("Addresses");
            xnConfig.AppendChild(xnAddresses);

            foreach (IPAddress address in this.Addresses)
            {
                System.Xml.XmlNode xnAddress = xmlDoc.CreateElement("Address");
                xnAddress.InnerText = address.ToString();
                xnAddresses.AppendChild(xnAddress);
            }

            System.Xml.XmlNode xnPort = xmlDoc.CreateElement("Port");
            xnPort.InnerText = this.Port.ToString();
            xnConfig.AppendChild(xnPort);

            System.Xml.XmlNode xnPassword = xmlDoc.CreateElement("Password");
            xnPassword.InnerText = this.Password;
            xnConfig.AppendChild(xnPassword);

            System.Xml.XmlNode xnMaxConnection = xmlDoc.CreateElement("MaxConnection");
            xnMaxConnection.InnerText = this.MaxConnection.ToString();
            xnConfig.AppendChild(xnMaxConnection);

            System.Xml.XmlNode xnDelay = xmlDoc.CreateElement("Delay");
            xnDelay.InnerText = this.Delay.ToString();
            xnConfig.AppendChild(xnDelay);

            return xnConfig;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Addresses != null && this.Addresses.Length > 0)
            {
                string strIp = "";
                foreach (IPAddress ipAddress in this.Addresses)
                {
                    strIp += "、" + ipAddress.ToString();
                }
                return strIp.Substring(1) + ":" + this.Port.ToString();
            }
            else
            {
                return "未配置通道服务成员的 IP 地址。";
            }
        }
    }

    /// <summary>
    /// 通道组成员信息集合。
    /// </summary>
    public class ChannelServiceMemberInfoCollection : List<ChannelServiceMemberInfo>
    {
        /// <summary>
        /// 设置通道服务成员集合配置信息。
        /// </summary>
        /// <param name="deviceConfig"></param>
        public void SetConfig(System.Xml.XmlNode deviceConfig)
        {
            this.Clear();

            foreach (System.Xml.XmlNode xnMemberInfo in deviceConfig.ChildNodes)
            {
                ChannelServiceMemberInfo memberInfo = new ChannelServiceMemberInfo();
                memberInfo.SetConfig(xnMemberInfo);
                this.Add(memberInfo);
            }
        }

        /// <summary>
        /// 获取通道服务成员集合配置信息。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement("MemberInfos");

            foreach (ChannelServiceMemberInfo memberInfo in this)
            {
                xnConfig.AppendChild(memberInfo.GetConfig(xmlDoc));
            }

            return xnConfig;
        }
    }
}
