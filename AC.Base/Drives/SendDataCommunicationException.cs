using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 向设备发送数据时，发送的数据格式不正确时产生的异常。
    /// </summary>
    public class SendDataCommunicationException : DataCommunicationException
    {
        /// <summary>
        /// 向设备发送数据时，发送的数据格式不正确时产生的异常。
        /// </summary>
        /// <param name="drive">产生数据异常的设备驱动。</param>
        /// <param name="sendData">产生异常的数据。</param>
        /// <param name="message">异常消息。</param>
        public SendDataCommunicationException(IDrive drive, byte[] sendData, string message)
            : base(drive, message)
        {
            this.m_SendData = sendData;
        }

        private byte[] m_SendData;
        /// <summary>
        /// 产生异常的数据。
        /// </summary>
        public byte[] SendData
        {
            get
            {
                return this.m_SendData;
            }
        }

    }
}
