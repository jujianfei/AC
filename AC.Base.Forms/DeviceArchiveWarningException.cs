using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AC.Base.Forms
{
    /// <summary>
    /// 修改设备档案或者删除设备档案时，某一档案项发生一般警告信息但是又不影响正常逻辑时产生的异常。
    /// </summary>
    public class DeviceArchiveWarningException : Exception
    {
        /// <summary>
        /// 修改设备档案或者删除设备档案时，某一档案项发生一般警告信息但是又不影响正常逻辑时产生的异常。
        /// </summary>
        public DeviceArchiveWarningException()
        {
        }

        /// <summary>
        /// 修改设备档案或者删除设备档案时，某一档案项发生一般警告信息但是又不影响正常逻辑时产生的异常。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        public DeviceArchiveWarningException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 修改设备档案或者删除设备档案时，某一档案项发生一般警告信息但是又不影响正常逻辑时产生的异常。
        /// </summary>
        /// <param name="info">System.Runtime.Serialization.SerializationInfo，它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context">System.Runtime.Serialization.StreamingContext，它包含有关源或目标的上下文信息。</param>
        public DeviceArchiveWarningException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 修改设备档案或者删除设备档案时，某一档案项发生一般警告信息但是又不影响正常逻辑时产生的异常。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用。</param>
        public DeviceArchiveWarningException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
