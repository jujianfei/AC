using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 接收到设备返回的数据后，返回的数据格式不正确时所产生的异常。
    /// </summary>
    public class ReceiveDataCommunicationException : DataCommunicationException
    {
        /// <summary>
        /// 接收到设备返回的数据后，返回的数据格式不正确时所产生的异常。
        /// </summary>
        /// <param name="drive">产生数据异常的设备驱动。</param>
        /// <param name="receiveData">产生异常的数据。</param>
        /// <param name="message">异常消息。</param>
        public ReceiveDataCommunicationException(IDrive drive, byte[] receiveData, string message)
            : base(drive, message + " 接收的数据: " + Function.OutBytes(receiveData))
        {
            this.m_ReceiveData = receiveData;
        }

        private byte[] m_ReceiveData;
        /// <summary>
        /// 产生异常的数据。
        /// </summary>
        public byte[] ReceiveData
        {
            get
            {
                return this.m_ReceiveData;
            }
        }

    }
}
