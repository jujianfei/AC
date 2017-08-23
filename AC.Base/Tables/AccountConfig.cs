using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 账号配置
    /// </summary>
    [Database.Table("账号配置", Database.TableCreateRuleOptions.Only, "")]
    public struct AccountConfig
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "AccountConfig";

        /// <summary>
        /// 帐号标识
        /// </summary>
        [Database.Column("帐号标识", "", 250, true, true)]
        public const string AccountCode = "AccountCode";

        /// <summary>
        /// 配置类型
        /// </summary>
        [Database.Column("配置类型", "", 250, true, true)]
        public const string ConfigType = "ConfigType";

        /// <summary>
        /// 配置信息
        /// </summary>
        [Database.Column("配置信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";
    }
}
