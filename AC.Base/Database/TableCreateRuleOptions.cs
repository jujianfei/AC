using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 数据表创建规则。
    /// </summary>
    public enum TableCreateRuleOptions
    {
        /// <summary>
        /// 单一表。该表在数据库中仅存在一份。
        /// </summary>
        Only,

        /// <summary>
        /// 每月一张表。
        /// </summary>
        Monthly,

        /// <summary>
        /// 每年一张表。
        /// </summary>
        Yearly,
    }


    /// <summary>
    /// 数据表创建规则扩展。
    /// </summary>
    public static class TableCreateRuleExtensions
    {
        /// <summary>
        /// 获取数据表创建规则的文字说明。
        /// </summary>
        /// <param name="createRule">数据表创建规则</param>
        /// <returns>数据表创建规则的文字说明</returns>
        public static string GetDescription(this TableCreateRuleOptions createRule)
        {
            switch (createRule)
            {
                case TableCreateRuleOptions.Only:
                    return "单一";

                case TableCreateRuleOptions.Monthly:
                    return "每月";

                case TableCreateRuleOptions.Yearly:
                    return "每年";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
