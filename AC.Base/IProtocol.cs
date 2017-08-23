using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 用于处理设备主动上报的数据并将处理后的数据存入数据库的规约接口。
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 规约名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 解析设备主动上报的上行数据，并将数据存入数据库。如果数据无法解析或者解析过程中发生校验错误等情况均抛出异常。
        /// </summary>
        /// <param name="receiveData">接收到的完整的数据。</param>
        void DataStorage(byte[] receiveData);
    }
}
