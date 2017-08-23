using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 提供访问通道服务的供后台使用的驱动基类。
    /// </summary>
    public abstract class ChannelDriveBase_ex : DriveBase, ISendDataDriveImplement
    {
        internal byte[] SessionId = new byte[2];            //由通道服务分配的 Session ID。
        private ushort m_ustSequenceNumber = 0;             //数据发送序号，如果通道服务有返回数据，则返回的序号与发送的序号一致。


        /// <summary>
        /// 通过当前驱动发送数据。发送数据时各驱动考虑硬件实际情况决定是否需要自行处理发送队列。
        /// </summary>
        /// <param name="childrenDrive">发送数据的子驱动对象。该对象应是当前驱动的下级驱动对象之一。</param>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">等待回应数据所需要的额外延时（总延时时间 = 上级设备延时时间 + 额外延时时间）。该值为 -1 时表示不等待回应，数据发出即可，此状态下回应数据返回 null；等于 0 时表示不做额外延时等待，等待时间由上级设备决定；大于 0 表示需要等待回应数据的额外延时，单位毫秒(ms)。</param>
        /// <returns>回应的数据。</returns>
        public override byte[] SendData(IDrive childrenDrive, byte[] sendData, int delay)
        {
            m_ustSequenceNumber++;
            return null;
        }

        /// <summary>
        /// 直接向当前驱动的设备发送数据。
        /// </summary>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">等待回应数据所需要的额外延时（总延时时间 = 上级设备延时时间 + 额外延时时间）。该值为 -1 时表示不等待回应，数据发出即可，此状态下回应数据返回 null；等于 0 时表示不做额外延时等待，等待时间由上级设备决定；大于 0 表示需要等待回应数据的额外延时，单位毫秒(ms)。</param>
        /// <returns>回应的数据。</returns>
        public byte[] SendData(byte[] sendData, int delay)
        {
            //任务队列应该在服务端做，客户端只要请求数据就发往服务端
            //Queue q;
            return null;
        }

        /// <summary>
        /// 接受上报的数据。
        /// </summary>
        /// <param name="receiveData">上报的数据。</param>
        /// <returns>需要回应的数据，如无回应数据则返回 null。</returns>
        public override byte[] ReceiveData(byte[] receiveData)
        {
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

        private ChannelServiceMemberInfoCollection m_ChannelServiceMemberInfos;
        /// <summary>
        /// 通道服务成员信息集合。
        /// </summary>
        public ChannelServiceMemberInfoCollection ChannelServiceMemberInfos
        {
            get
            {
                if (this.m_ChannelServiceMemberInfos == null)
                {
                    this.m_ChannelServiceMemberInfos = new ChannelServiceMemberInfoCollection();
                }
                return this.m_ChannelServiceMemberInfos;
            }
        }

        private ChannelDriveConnectMemberCollection m_ConnectMembers;
        /// <summary>
        /// 与各通道服务成员的 Socket 连接。
        /// </summary>
        public ChannelDriveConnectMemberCollection ConnectMembers
        {
            get
            {
                if (this.m_ConnectMembers == null)
                {
                    lock (this)
                    {
                        if (this.m_ConnectMembers == null)
                        {
                            this.m_ConnectMembers = new ChannelDriveConnectMemberCollection();

                            foreach (ChannelServiceMemberInfo memberInfo in this.ChannelServiceMemberInfos)
                            {
                                this.m_ConnectMembers.Add(new ChannelDriveConnectMember(this, memberInfo));
                            }
                        }
                    }
                }
                return this.m_ConnectMembers;
            }
        }


        /// <summary>
        /// 从保存此设备驱动数据的 XML 文档节点初始化当前设备驱动。实现该接口的类应注意：该方法在设备驱动对象创建之后可能仍会多次调用，如刷新设备档案时不会新创建该对象，而是调用设备的 Reload 方法直接从数据库取出数据后赋值给设备驱动，所以对于集合一类的属性应先清空集合再进行添加操作。
        /// </summary>
        /// <param name="deviceConfig">该对象节点的数据</param>
        public override void SetDriveConfig(System.Xml.XmlNode deviceConfig)
        {
            foreach (System.Xml.XmlNode xnItem in deviceConfig.ChildNodes)
            {
                switch (xnItem.Name)
                {
                    case "MemberInfos":
                        this.ChannelServiceMemberInfos.SetConfig(xnItem);
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
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            xnConfig.AppendChild(this.ChannelServiceMemberInfos.GetConfig(xmlDoc));

            return xnConfig;
        }
    }
}
