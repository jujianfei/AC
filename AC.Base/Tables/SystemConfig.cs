using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [Database.Table("系统配置", Database.TableCreateRuleOptions.Only, "")]
    public struct SystemConfig
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "SystemConfig";

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
