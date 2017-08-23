using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.DLT645
{
    /// <summary>
    /// 使用 DL/T 645 电力行业标准通信规约的设备驱动。
    /// </summary>
    public abstract class DLT645Drive : DriveBase
    {
        /// <summary>
        /// 电能表通讯地址。由6 个字节构成，每字节2 位BCD 码。地址长度可达12 位十进制数，可以为表号、资产号、用户号、设备号等。具体使用可由用户自行决定。当使用的地址码长度不足6 字节时，用十六进制AAH 补足6 字节。低地址位在先，高地址位在后。当地址为999999999999H 时，为广播地址。
        /// </summary>
        public long Address { get; set; }

        /// <summary>
        /// 从保存此设备驱动数据的 XML 文档节点初始化当前设备驱动。实现该接口的类应注意：该方法在设备驱动对象创建之后可能仍会多次调用，如刷新设备档案时不会新创建该对象，而是调用设备的 Reload 方法直接从数据库取出数据后赋值给设备驱动，所以对于集合一类的属性应先清空集合再进行添加操作。
        /// </summary>
        /// <param name="deviceConfig">该对象节点的数据</param>
        public override void SetDriveConfig(System.Xml.XmlNode deviceConfig)
        {
        
        }

        /// <summary>
        /// 获取当前设备驱动的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public override System.Xml.XmlNode GetDriveConfig(System.Xml.XmlDocument xmlDoc)
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
        public override byte[] SendData(IDrive childrenDrive, byte[] sendData, int delay)
        {
            throw new Exception("该设备不支持转发下级设备的数据。");
        }

        /// <summary>
        /// 接受上报的数据。
        /// </summary>
        /// <param name="receiveData">上报的数据。</param>
        /// <returns>需要回应的数据，如无回应数据则返回 null。</returns>
        public override byte[] ReceiveData(byte[] receiveData)
        {
            return null;
        }
    }
}
