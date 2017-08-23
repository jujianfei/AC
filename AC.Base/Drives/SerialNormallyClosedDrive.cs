using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace AC.Base.Drives
{
    /// <summary>
    /// 常闭串行通讯驱动。每次通讯前打开串口，通讯结束关闭串口。
    /// </summary>
    public class SerialNormallyClosedDrive : DriveBase
    {
        private string m_PortName = "COM1";
        /// <summary>
        /// 获取或设置通信端口。
        /// </summary>
        public string PortName
        {
            get
            {
                return this.m_PortName;
            }
            set
            {
                this.m_PortName = value;
            }
        }

        private int m_BaudRate = 9600;
        /// <summary>
        /// 获取或设置串行波特率，默认为9600bps。
        /// </summary>
        public int BaudRate
        {
            get
            {
                return this.m_BaudRate;
            }
            set
            {
                this.m_BaudRate = value;
            }
        }

        private Parity m_Parity = Parity.None;
        /// <summary>
        /// 获取或设置奇偶校验检查协议，默认为无奇偶校验。
        /// </summary>
        public Parity Parity
        {
            get
            {
                return this.m_Parity;
            }
            set
            {
                this.m_Parity = value;
            }
        }

        private int m_DataBits = 8;
        /// <summary>
        /// 获取或设置每个字节的标准数据位长度，默认为8。
        /// </summary>
        public int DataBits
        {
            get
            {
                return this.m_DataBits;
            }
            set
            {
                this.m_DataBits = value;
            }
        }

        private StopBits m_StopBits = StopBits.One;
        /// <summary>
        /// 获取或设置每个字节的标准停止位数，默认为1位。
        /// </summary>
        public StopBits StopBits
        {
            get
            {
                return this.m_StopBits;
            }
            set
            {
                this.m_StopBits = value;
            }
        }

        private int m_ReceiveTimeout = 2000;
        /// <summary>
        /// 接收超时（毫秒）。等待设备响应请求的时间。
        /// </summary>
        public int ReceiveTimeout
        {
            get { return m_ReceiveTimeout; }
            set { m_ReceiveTimeout = value; }
        }

        private int m_ReceiveInterval = 100;
        /// <summary>
        /// 接收间隔（毫秒）。小于此间隔的数据认为是同一帧数据，超出此间隔的后续数据将被丢弃。该值默认为 200ms，最小不能小于 30ms。
        /// </summary>
        public int ReceiveInterval
        {
            get { return m_ReceiveInterval; }
            set
            {
                if (value < 30)
                {
                    m_ReceiveInterval = 30;
                }
                else
                {
                    m_ReceiveInterval = value;
                }
            }
        }


        /// <summary>
        /// 从保存此设备驱动数据的 XML 文档节点初始化当前设备驱动。实现该接口的类应注意：该方法在设备驱动对象创建之后可能仍会多次调用，如刷新设备档案时不会新创建该对象，而是调用设备的 Reload 方法直接从数据库取出数据后赋值给设备驱动，所以对于集合一类的属性应先清空集合再进行添加操作。
        /// </summary>
        /// <param name="deviceConfig">该对象节点的数据</param>
        public override void SetDriveConfig(System.Xml.XmlNode deviceConfig)
        {
            foreach (System.Xml.XmlNode xnConfig in deviceConfig.ChildNodes)
            {
                switch (xnConfig.Name)
                {
                    case "PortName":
                        this.PortName = xnConfig.InnerText;
                        break;

                    case "BaudRate":
                        this.BaudRate = Function.ToInt(xnConfig.InnerText);
                        break;

                    case "Parity":
                        this.Parity = (System.IO.Ports.Parity)Function.ToInt(xnConfig.InnerText);
                        break;

                    case "DataBits":
                        this.DataBits = Function.ToInt(xnConfig.InnerText);
                        break;

                    case "StopBits":
                        this.StopBits = (System.IO.Ports.StopBits)Function.ToInt(xnConfig.InnerText);
                        break;

                    case "ReceiveTimeout":
                        this.ReceiveTimeout = Function.ToInt(xnConfig.InnerText);
                        break;

                    case "ReceiveInterval":
                        this.ReceiveInterval = Function.ToInt(xnConfig.InnerText);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取当前设备驱动的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public override System.Xml.XmlNode GetDriveConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnDrive = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnPortName = xmlDoc.CreateElement("PortName");
            xnPortName.InnerText = this.PortName;
            xnDrive.AppendChild(xnPortName);

            System.Xml.XmlNode xnBaudRate = xmlDoc.CreateElement("BaudRate");
            xnBaudRate.InnerText = this.BaudRate.ToString();
            xnDrive.AppendChild(xnBaudRate);

            System.Xml.XmlNode xnParity = xmlDoc.CreateElement("Parity");
            xnParity.InnerText = ((int)this.Parity).ToString();
            xnDrive.AppendChild(xnParity);

            System.Xml.XmlNode xnDataBits = xmlDoc.CreateElement("DataBits");
            xnDataBits.InnerText = this.DataBits.ToString();
            xnDrive.AppendChild(xnDataBits);

            System.Xml.XmlNode xnStopBits = xmlDoc.CreateElement("StopBits");
            xnStopBits.InnerText = ((int)this.StopBits).ToString();
            xnDrive.AppendChild(xnStopBits);

            System.Xml.XmlNode xnReceiveTimeout = xmlDoc.CreateElement("ReceiveTimeout");
            xnReceiveTimeout.InnerText = this.ReceiveTimeout.ToString();
            xnDrive.AppendChild(xnReceiveTimeout);

            System.Xml.XmlNode xnReceiveInterval = xmlDoc.CreateElement("ReceiveInterval");
            xnReceiveInterval.InnerText = this.ReceiveInterval.ToString();
            xnDrive.AppendChild(xnReceiveInterval);

            return xnDrive;
        }

        /// <summary>
        /// 通过当前驱动发送数据。发送数据时各驱动考虑硬件实际情况决定是否需要自行处理发送队列。
        /// </summary>
        /// <param name="childrenDrive">发送数据的子驱动对象。该对象应是当前驱动的下级驱动对象之一。</param>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">等待回应数据所需要的额外延时（总延时时间 = 上级设备延时时间 + 额外延时时间）。该值为 -1 时表示不等待回应，数据发出即可，此状态下回应数据返回 null；等于 0 时表示不做额外延时等待，等待时间由上级设备决定；大于 0 表示需要等待回应数据的额外延时，单位毫秒(ms)。</param>
        /// <returns>回应的数据。</returns>
        public override byte[] SendData(IDrive childrenDrive, byte[] sendData, int delay)
        {
            lock (this)
            {
                SerialPort port = new SerialPort(this.PortName, this.BaudRate, this.Parity, this.DataBits, this.StopBits);
                port.Open();

                try
                {
                    base.OnSendingData(childrenDrive, sendData);
                    port.Write(sendData, 0, sendData.Length);

#if DEBUG
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss ffff") + " TX: " + Function.OutBytes(sendData));
#endif

                    if (delay >= 0)
                    {
                        byte[] bytReceiveData = new byte[0];
                        int intTimeout = this.m_ReceiveTimeout + delay;
                        int intWaitTime = 0;                //总等待时间
                        int intWaitIntervalTime = 0;        //数据接收时的数据包间隔等待时间

                        do
                        {
                            System.Threading.Thread.Sleep(30);

                            int intReadLength = port.BytesToRead;

                            if (intReadLength > 0)
                            {
                                //将数据复制到临时数组中
                                byte[] bytTemp = new byte[bytReceiveData.Length];
                                for (int intIndex = 0; intIndex < bytReceiveData.Length; intIndex++)
                                {
                                    bytTemp[intIndex] = bytReceiveData[intIndex];
                                }

                                //扩大数据接收数组容量
                                bytReceiveData = new byte[bytTemp.Length + intReadLength];

                                //将临时数组中的数据复制到接收数组中
                                for (int intIndex = 0; intIndex < bytTemp.Length; intIndex++)
                                {
                                    bytReceiveData[intIndex] = bytTemp[intIndex];
                                }

                                port.Read(bytReceiveData, bytTemp.Length, intReadLength);

                                intWaitIntervalTime = 0;
                            }
                            else
                            {
                                intWaitTime += 30;                        //累计数据接收时间间隔

                                if (bytReceiveData.Length == 0)
                                {
                                    if (intWaitTime >= intTimeout)
                                    {
                                        //接收数据超时
                                        throw new ReceiveTimeoutCommunicationException(intTimeout);
                                    }
                                }
                                else
                                {
                                    intWaitIntervalTime += 30;

                                    if (intWaitIntervalTime >= this.ReceiveInterval)
                                    {
                                        //已收到数据，并且在一个接收间隔时间后无新的数据

                                        base.OnReceivingData(this, bytReceiveData);
#if DEBUG
                                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss ffff") + " RX: " + Function.OutBytes(bytReceiveData));
#endif
                                        return bytReceiveData;
                                    }
                                }
                            }
                        } while (true);
                    }
                    return null;
                }
                finally
                {
                    port.Close();
                }
            }
        }

        /// <summary>
        /// 接受上报的数据。
        /// </summary>
        /// <param name="receiveData">上报的数据。</param>
        /// <returns>需要回应的数据，如无回应数据则返回 null。</returns>
        public override byte[] ReceiveData(byte[] receiveData)
        {
            //该串口设备不对规约进行解析判断，上报的数据交由下级设备进行处理，如无任何设备处理该数据则返回 null。
            foreach (IDrive drive in base.Children)
            {
                byte[] bytData = drive.ReceiveData(receiveData);
                if (bytData != null)
                {
                    return bytData;
                }
            }
            return null;
        }
    }
}
