using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 提供实现 IDrive 接口的设备驱动的基础实现。
    /// </summary>
    public abstract class DriveBase : IDrive
    {
        #region << IDrive 成员 >>
        /// <summary>
        /// 从保存此设备驱动数据的 XML 文档节点初始化当前设备驱动。实现该接口的类应注意：该方法在设备驱动对象创建之后可能仍会多次调用，如刷新设备档案时不会新创建该对象，而是调用设备的 Reload 方法直接从数据库取出数据后赋值给设备驱动，所以对于集合一类的属性应先清空集合再进行添加操作。
        /// </summary>
        /// <param name="deviceConfig">该对象节点的数据</param>
        public virtual void SetDriveConfig(System.Xml.XmlNode deviceConfig)
        {
        }

        /// <summary>
        /// 获取当前设备驱动的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public virtual System.Xml.XmlNode GetDriveConfig(System.Xml.XmlDocument xmlDoc)
        {
            return null;
        }

        /// <summary>
        /// 通过当前驱动发送数据。发送数据时各驱动考虑硬件实际情况决定是否需要自行处理发送队列。
        /// </summary>
        /// <param name="childrenDrive">发送数据的子驱动对象。该对象应是当前驱动的下级驱动对象之一。</param>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">等待回应数据所需要的额外延时（总延时时间 = 上级设备延时时间 + 额外延时时间）。该值为 -1 时表示不等待回应，数据发出即可，此状态下回应数据返回 null；等于 0 时表示不做额外延时等待，等待时间由上级设备决定；大于 0 表示需要等待回应数据的额外延时，单位毫秒(ms)。</param>
        /// <returns>回应的数据。</returns>
        public abstract byte[] SendData(IDrive childrenDrive, byte[] sendData, int delay);

        /// <summary>
        /// 接受上报的数据。
        /// </summary>
        /// <param name="receiveData">上报的数据。</param>
        /// <returns>需要回应的数据，如无回应数据则返回 null。</returns>
        public abstract byte[] ReceiveData(byte[] receiveData);

        /// <summary>
        /// 获取上级驱动对象，如无上级驱动则返回 null。
        /// </summary>
        /// <remarks>应确保该事件必须且只能被绑定一次，以保证委托返回的驱动对象被正确传递。</remarks>
        public event GettingParentDriveEventHandler GettingParent;

        /// <summary>
        /// 获取所有下级驱动对象。
        /// </summary>
        /// <remarks>应确保该事件必须且只能被绑定一次，以保证委托返回的子驱动对象集合被正确传递。</remarks>
        public event GettingChildrenDriveEventHandler GettingChildren;

        /// <summary>
        /// 发送数据时产生的事件
        /// </summary>
        public event DriveSendingDataEventHandler SendingData;

        /// <summary>
        /// 接收到上报数据时产生的事件
        /// </summary>
        public event DriveReceivingDataEventHandler ReceivingData;

        /// <summary>
        /// 启动通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先开启后台服务以便主控软件可以与该通道连接，然后开启设备端服务允许设备接入。
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// 停止通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先关闭设备端服务断开所有设备的连接，然后通知所有已连接的后台自身即将关闭的消息并关闭后台服务。对于通道安全运行的多种形式，如主备通道或多主通道，则由具体实现完成该功能。
        /// </summary>
        public virtual void Stop()
        {
        }
        #endregion

        /// <summary>
        /// 获取上级驱动对象，如无上级驱动则返回 null。
        /// </summary>
        public IDrive Parent
        {
            get
            {
                if (this.GettingParent != null)
                {
                    return this.GettingParent();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取所有下级驱动对象。
        /// </summary>
        public IDriveCollection Children
        {
            get
            {
                if (this.GettingChildren != null)
                {
                    return this.GettingChildren();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 发送数据时，引发 SendingData 事件。
        /// </summary>
        /// <param name="drive">发送数据的驱动对象</param>
        /// <param name="sendData">发送的数据</param>
        protected internal void OnSendingData(IDrive drive, byte[] sendData)
        {
            if (this.SendingData != null)
            {
                this.SendingData(drive, sendData);
            }
        }

        /// <summary>
        /// 接收数据时，引发 ReceivingData 事件。
        /// </summary>
        /// <param name="drive">收到数据的驱动对象</param>
        /// <param name="receiveData">收到的数据</param>
        protected internal void OnReceivingData(IDrive drive, byte[] receiveData)
        {
            if (this.ReceivingData != null)
            {
                this.ReceivingData(drive, receiveData);
            }
        }
    }
}
