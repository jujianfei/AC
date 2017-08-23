using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 任务配置。
    /// </summary>
    [Database.Table("任务配置", Database.TableCreateRuleOptions.Only, "")]
    public struct TaskConfig
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "TaskConfig";

        /// <summary>
        /// 任务配置编号
        /// </summary>
        [Database.Column("任务配置编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string TaskConfigId = "TaskConfigId";

        /// <summary>
        /// 任务类型
        /// </summary>
        [Database.Column("任务类型", "", 250, false, true)]
        public const string TaskType = "TaskType";

        /// <summary>
        /// 任务名称
        /// </summary>
        [Database.Column("任务名称", "", 250, false, true)]
        public const string Name = "Name";

        /// <summary>
        /// 所属任务组
        /// </summary>
        [Database.Column("所属任务组", "", Database.DataTypeOptions.Int, false, true)]
        public const string TaskGroupId = "TaskGroupId";

        /// <summary>
        /// 允许自动运行
        /// </summary>
        [Database.Column("允许自动运行", "", Database.DataTypeOptions.Int, false, true)]
        public const string EnableAuto = "EnableAuto";

        /// <summary>
        /// 配置信息
        /// </summary>
        [Database.Column("配置信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";
    }
}
