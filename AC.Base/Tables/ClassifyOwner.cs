using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 分类所有者。表明某一分类是公共分类还是私有分类，该表仅记录根节点(ParentId=0)的分类。
    /// </summary>
    [Database.Table("分类所有者", Database.TableCreateRuleOptions.Only, "")]
    public struct ClassifyOwner
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        [Database.Column("分类编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string ClassifyId = "ClassifyId";

        /// <summary>
        /// 帐号标识
        /// </summary>
        [Database.Column("帐号标识", "", 250, false, false)]
        public const string AccountCode = "AccountCode";
    }
}
