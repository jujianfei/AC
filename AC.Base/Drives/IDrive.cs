using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 获取上层驱动对象时产生的事件所调用的委托。
    /// </summary>
    /// <returns>上层驱动对象。如无上层驱动则返回 null。</returns>
    public delegate IDrive GettingParentDriveEventHandler();

    /// <summary>
    /// 获取所有下层驱动对象时产生的事件所调用的委托。
    /// </summary>
    /// <returns>下层驱动对象集合。</returns>
    public delegate IDriveCollection GettingChildrenDriveEventHandler();

    /// <summary>
    /// 发送数据时产生的事件所调用的委托。
    /// </summary>
    /// <param name="drive">发送数据的驱动对象</param>
    /// <param name="sendData">发送的数据</param>
    public delegate void DriveSendingDataEventHandler(IDrive drive, byte[] sendData);

    /// <summary>
    /// 接收到上报数据时产生的事件所调用的委托。
    /// </summary>
    /// <param name="drive">收到数据的驱动对象</param>
    /// <param name="receiveData">收到的数据</param>
    public delegate void DriveReceivingDataEventHandler(IDrive drive, byte[] receiveData);

    /// <summary>
    /// 设备驱动接口。表示一个具有通讯、数据运算、数据处理等能力的设备驱动。实现该接口的类主要完成通讯中下行报文的组帧，上行报文的解析通常由实现 IProtocol 接口的规约类进行处理。实现该接口的实体类必须提供一个无参数的构造函数。
    /// </summary>
    public interface IDrive
    {
        /// <summary>
        /// 从保存此设备驱动数据的 XML 文档节点初始化当前设备驱动。实现该接口的类应注意：该方法在设备驱动对象创建之后可能仍会多次调用，如刷新设备档案时不会新创建该对象，而是调用设备的 Reload 方法直接从数据库取出数据后赋值给设备驱动，所以对于集合一类的属性应先清空集合再进行添加操作。
        /// </summary>
        /// <param name="deviceConfig">该对象节点的数据</param>
        void SetDriveConfig(System.Xml.XmlNode deviceConfig);

        /// <summary>
        /// 获取当前设备驱动的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        System.Xml.XmlNode GetDriveConfig(System.Xml.XmlDocument xmlDoc);

        /// <summary>
        /// 获取上级驱动对象，如无上级驱动则返回 null。
        /// </summary>
        /// <remarks>应确保该事件必须且只能被绑定一次，以保证委托返回的驱动对象被正确传递。</remarks>
        event GettingParentDriveEventHandler GettingParent;

        /// <summary>
        /// 获取所有下级驱动对象。
        /// </summary>
        /// <remarks>应确保该事件必须且只能被绑定一次，以保证委托返回的子驱动对象集合被正确传递。</remarks>
        event GettingChildrenDriveEventHandler GettingChildren;


        /// <summary>
        /// 通过当前驱动发送数据。发送数据时各驱动考虑硬件实际情况决定是否需要自行处理发送队列。
        /// </summary>
        /// 
        /// <param name="childrenDrive">发送数据的子驱动对象。该对象应是当前驱动的下级驱动对象之一。</param>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">等待回应数据所需要的额外延时（总延时时间 = 上级设备延时时间 + 额外延时时间）。该值为 -1 时表示不等待回应，数据发出即可，此状态下回应数据返回 null；等于 0 时表示不做额外延时等待，等待时间由上级设备决定；大于 0 表示需要等待回应数据的额外延时，单位毫秒(ms)。</param>
        /// <returns>回应的数据。</returns>

        /// <summary>
        /// 通过当前驱动发送数据。发送数据时各驱动考虑硬件实际情况决定是否需要自行处理发送队列。
        /// </summary>
        /// <param name="childrenDrive">发送数据的子驱动对象。该对象应是当前驱动的下级驱动对象之一。</param>
        /// <param name="sendData">需要发送的数据</param>
        /// <param name="delay">延迟</param>
        /// <returns>回应的数据</returns>
        byte[] SendData(IDrive childrenDrive, byte[] sendData,int delay);

        /// <summary>
        /// 发送数据时产生的事件。
        /// </summary>
        event DriveSendingDataEventHandler SendingData;

        /// <summary>
        /// 接受上报的数据。
        /// </summary>
        /// <param name="receiveData">上报的数据。</param>
        /// <returns>需要回应的数据，如无回应数据则返回 null。</returns>
        byte[] ReceiveData(byte[] receiveData);

        /// <summary>
        /// 接收到上报数据时产生的事件。
        /// </summary>
        event DriveReceivingDataEventHandler ReceivingData;
    }
}
