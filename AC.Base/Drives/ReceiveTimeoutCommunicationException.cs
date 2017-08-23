using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 向设备发送数据后，在指定的时间内设备没有回应所产生的异常。
    /// </summary>
    public class ReceiveTimeoutCommunicationException : CommunicationException
    {
        private int m_Timeout;

        /// <summary>
        /// 通讯超时异常
        /// </summary>
        /// <param name="timeout"></param>
        public ReceiveTimeoutCommunicationException(int timeout)
        {
            this.m_Timeout = timeout;
        }

        /// <summary>
        /// 通讯超时异常信息。
        /// </summary>
        public override string Message
        {
            get
            {
                return "在指定的接收超时时间 " + this.m_Timeout + " 毫秒后，无响应数据。";
            }
        }
    }
}
