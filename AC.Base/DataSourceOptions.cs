using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 数据来源选项。
    /// </summary>
    public enum DataSourceOptions
    {
        /// <summary>
        /// 无。
        /// </summary>
        None = 0,

        /// <summary>
        /// 从设备采集。
        /// </summary>
        Device = 1,

        /// <summary>
        /// 根据现有数据计算。
        /// </summary>
        Computing = 2,
    }

    /// <summary>
    /// 数据来源选项扩展。
    /// </summary>
    public static class DataSourceExtensions
    {
        /// <summary>
        /// 获取数据来源的文字说明。
        /// </summary>
        /// <param name="dataSource">数据来源</param>
        /// <returns>数据来源的文字说明</returns>
        public static string GetDescription(this DataSourceOptions dataSource)
        {
            switch (dataSource)
            {
                case DataSourceOptions.None:
                    return "无";

                case DataSourceOptions.Device:
                    return "采集";

                case DataSourceOptions.Computing:
                    return "计算";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
