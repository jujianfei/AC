using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AC.Base.Exam.Exam
{
    public class Devices
    {
        /// <summary>
        /// 主键
        /// </summary>
        private int baseKey;

        public int BaseKey
        {
            get { return baseKey; }
            set { baseKey = value; }
        }


        private string deviceName;

        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }


        /// <summary>
        /// 外键。taskConfig
        /// </summary>
        private int taskConfigId;

        public int TaskConfigId
        {
            get { return taskConfigId; }
            set { taskConfigId = value; }
        }

        /// <summary>
        /// 序列号
        /// </summary>
        private String suite;

        public String Suite
        {
            get { return suite; }
            set { suite = value; }
        }


        /// <summary>
        /// 工厂型号
        /// </summary>
        private String factory;

        public String Factory
        {
            get { return factory; }
            set { factory = value; }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private String address;

        public String Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// 设备类型。
        /// </summary>
        private int deviceType;

        public int DeviceType
        {
            get { return deviceType; }
            set { deviceType = value; }
        }

        private String xmlConfig;

        public String XmlConfig
        {
            get
            {
                return xmlConfig = this.SetDeviceConfig();

            }
            set
            {
                xmlConfig = value;
                if (value != "")
                {
                    XmlDocument newXmlDocument = new XmlDocument();
                    newXmlDocument.LoadXml(value);
                    this.GetDeviceConfig(newXmlDocument);
                }
            }
        }

        private List<Devices> deviceList ;

        public List<Devices> DeviceList
        {
            get { return deviceList; }
            set { deviceList = value; }
        }




        public void GetDeviceConfig(System.Xml.XmlNode xmlRootConfig)
        {
            foreach (System.Xml.XmlNode xmlConfig in xmlRootConfig.ChildNodes)
            {
                foreach (System.Xml.XmlNode xnConfig in xmlConfig.ChildNodes)
                {
                    Devices devices = new Devices();
                    foreach (System.Xml.XmlNode node in xnConfig)
                    {

                        switch (node.Name)
                        {
                            case "suite":
                                devices.suite = node.InnerText;
                                break;
                            case "address":
                                devices.address= node.InnerText;
                                break;
                            case "deviceType":
                                devices.deviceType = Convert.ToInt32(node.InnerText);
                                break;
                            case "factory":
                                devices.factory = node.InnerText;
                                break;
                        }
                    }
                    this.deviceList.Add(devices);
                }
            }
        }



        public String SetDeviceConfig()
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.XmlNode xnRoot = xmlDoc.CreateElement("root");

            foreach (Devices devices in deviceList)
            {
                System.Xml.XmlDocument xmlDocChild = new System.Xml.XmlDocument();
                System.Xml.XmlNode xnRecord = xmlDoc.CreateElement("device");

                System.Xml.XmlNode xnSuite = xmlDoc.CreateElement("suite");
                xnSuite.InnerText = devices.suite.ToString();
                xnRecord.AppendChild(xnSuite);

                System.Xml.XmlNode xnAddress = xmlDoc.CreateElement("address");
                xnAddress.InnerText = devices.address.ToString();
                xnRecord.AppendChild(xnAddress);

                System.Xml.XmlNode xnDeviceType = xmlDoc.CreateElement("deviceType");
                xnDeviceType.InnerText = devices.deviceType.ToString();
                xnRecord.AppendChild(xnDeviceType);

                System.Xml.XmlNode xnFactory = xmlDoc.CreateElement("factory");
                xnFactory.InnerText = devices.factory.ToString();
                xnRecord.AppendChild(xnFactory);

               

            }
            return xnRoot.OuterXml;
        }

        
    }
}
