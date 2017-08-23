using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 任务组。
    /// </summary>
    [Database.Table("任务组", Database.TableCreateRuleOptions.Only, "")]
    public struct TaskGroup
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "TaskGroup";

        /// <summary>
        /// 任务组编号
        /// </summary>
        [Database.Column("任务组编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string TaskGroupId = "TaskGroupId";

        /// <summary>
        /// 任务组名称
        /// </summary>
        [Database.Column("任务组名称", "", 250, false, true)]
        public const string Name = "Name";

        /// <summary>
        /// 配置信息
        /// </summary>
        [Database.Column("配置信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";
    }
}
