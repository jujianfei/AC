using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 设备发送或接收数据时，数据格式不正确所产生的异常。
    /// </summary>
    public abstract class DataCommunicationException : CommunicationException
    {
        /// <summary>
        /// 设备发送或接收数据时，数据格式不正确所产生的异常。
        /// </summary>
        /// <param name="drive">产生数据异常的设备驱动。</param>
        /// <param name="message">异常消息。</param>
        public DataCommunicationException(IDrive drive, string message)
            : base(message)
        {
            this.m_Drive = drive;
        }

        private IDrive m_Drive;
        /// <summary>
        /// 产生数据异常的设备驱动。
        /// </summary>
        public IDrive Drive
        {
            get
            {
                return this.m_Drive;
            }
        }
    }
}
