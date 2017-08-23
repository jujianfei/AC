using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace AC.Base.Drives
{
    /// <summary>
    /// 串行通讯扩展。
    /// </summary>
    public static class SerialExtensions
    {
        /// <summary>
        /// 串口波特率可用的值。
        /// </summary>
        public static int[] BaudRates = new int[] { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000, 512000 };

        /// <summary>
        /// 串口数据位可用的值。
        /// </summary>
        public static int[] DataBits = new int[] { 5, 6, 7, 8 };

        /// <summary>
        /// 获取奇偶校验的文字描述。
        /// </summary>
        /// <param name="parity">奇偶校验</param>
        /// <returns></returns>
        public static string GetDescription(this Parity parity)
        {
            switch (parity)
            {
                case Parity.None:
                    return "无";

                case Parity.Odd:
                    return "奇";

                case Parity.Even:
                    return "偶";

                case Parity.Mark:
                    return "标志";

                case Parity.Space:
                    return "空位";

                default:
                    return "";
            }
        }

        /// <summary>
        /// 获取停止位的文字描述。
        /// </summary>
        /// <param name="stopBits">停止位。</param>
        /// <returns></returns>
        public static string GetDescription(this StopBits stopBits)
        {
            switch (stopBits)
            {
                case StopBits.None:
                    return "无";

                case StopBits.One:
                    return "1位";

                case StopBits.Two:
                    return "2位";

                case StopBits.OnePointFive:
                    return "1.5位";

                default:
                    return "";
            }
        }
    }
}
