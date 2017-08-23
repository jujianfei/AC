using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// RTU接收数据返回状态
    /// </summary>
    public interface ISendReceiveStateImplement: IDriveImplement
    {
        /// <summary>
        /// 返回数据。
        /// </summary>
        byte[] ReceiveDatas { get; set; }

        /// <summary>
        /// 获取返回数据。
        /// </summary>
        /// <returns>该设备在线；false：该设备不在线。</returns>
        string getReceiveDatas();

        /// <summary>
        /// 获取设备通讯地址。
        /// </summary>
        int STATE { get; set; }

        /// <summary>
        /// 获取系统报错内容。
        /// </summary>
        string ResultString { get; set; }

        /// <summary>
        /// 获取返回状态。
        /// </summary>
        /// <returns></returns>
        string getState();

        /// <summary>
        /// 获取返回状态。
        /// </summary>
        /// <returns></returns>
        void FreeState();
    }
}
