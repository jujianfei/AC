using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 筛选器逻辑运算选项。控制 SQL 查询条件语句是以 AND 关系进行运算还是以 OR 关系进行运算。
    /// </summary>
    public enum FilterLogicOperatorOptions
    {
        /// <summary>
        /// And 运算符。
        /// </summary>
        And,

        /// <summary>
        /// Or 运算符。
        /// </summary>
        Or,
    }

    /// <summary>
    /// 筛选器逻辑运算选项扩展。
    /// </summary>
    public static class FilterLogicOperatorExtensions
    {
        /// <summary>
        /// 获取筛选器逻辑运算选项的文字说明。
        /// </summary>
        /// <param name="logicOperator"></param>
        /// <returns></returns>
        public static string GetFilterSql(this FilterLogicOperatorOptions logicOperator)
        {
            switch (logicOperator)
            {
                case FilterLogicOperatorOptions.And:
                    return "AND";

                case FilterLogicOperatorOptions.Or:
                    return "OR";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }

        /// <summary>
        /// 获取筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <param name="logicOperator"></param>
        /// <returns></returns>
        public static string GetFilterDescription(this FilterLogicOperatorOptions logicOperator)
        {
            switch (logicOperator)
            {
                case FilterLogicOperatorOptions.And:
                    return "并且";

                case FilterLogicOperatorOptions.Or:
                    return "或者";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
