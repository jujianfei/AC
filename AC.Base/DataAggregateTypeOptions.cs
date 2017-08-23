using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 使用 GROUP BY 对数据进行聚合运算时所使用的聚合方式。
    /// </summary>
    public enum DataAggregateTypeOptions
    {
        /// <summary>
        /// 合计值。
        /// </summary>
        Sum,

        /// <summary>
        /// 平均值。
        /// </summary>
        Avg,

        /// <summary>
        /// 最大值。
        /// </summary>
        Max,

        /// <summary>
        /// 最小值。
        /// </summary>
        Min,
    }

    /// <summary>
    /// 使用 GROUP BY 对数据进行聚合运算时所使用的聚合方式。
    /// </summary>
    public static class DataAggregateExtensions
    {
        /// <summary>
        /// 获取聚合方式在 SQL 语句中的函数名。
        /// </summary>
        /// <param name="dataAggregate"></param>
        /// <returns></returns>
        public static string GetName(this DataAggregateTypeOptions dataAggregate)
        {
            switch (dataAggregate)
            {
                case DataAggregateTypeOptions.Sum:
                    return "Sum";

                case DataAggregateTypeOptions.Avg:
                    return "Avg";

                case DataAggregateTypeOptions.Max:
                    return "Max";

                case DataAggregateTypeOptions.Min:
                    return "Min";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
