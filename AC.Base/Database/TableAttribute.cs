using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 数据表特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TableAttribute : System.Attribute
    {
        /// <summary>
        /// 数据表属性。
        /// </summary>
        /// <param name="name">数据表中文名。</param>
        /// <param name="createRule">数据表只创建一次还是按照每月、每年的规则进行创建。</param>
        /// <param name="remark">数据表备注。</param>
        public TableAttribute(string name, TableCreateRuleOptions createRule, string remark)
        {
            this.Name = name;
            this.CreateRule = createRule;
            this.Remark = remark;
        }

        /// <summary>
        /// 数据表中文名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 数据表创建规则。
        /// </summary>
        public TableCreateRuleOptions CreateRule { get; private set; }

        /// <summary>
        /// 数据表备注。
        /// </summary>
        public string Remark { get; private set; }
    }
}
