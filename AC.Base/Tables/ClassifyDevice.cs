using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 分类-设备关联
    /// </summary>
    [Database.Table("分类-设备关联", Database.TableCreateRuleOptions.Only, "")]
    public struct ClassifyDevice
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "ClassifyDevice";

        /// <summary>
        /// 分类编号
        /// </summary>
        [Database.Column("分类编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string ClassifyId = "ClassifyId";

        /// <summary>
        /// 设备编号
        /// </summary>
        [Database.Column("设备编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string DeviceId = "DeviceId";

        /// <summary>
        /// 排序编号
        /// </summary>
        [Database.Column("排序编号", "", Database.DataTypeOptions.Int, false, true)]
        public const string OrdinalNumber = "OrdinalNumber";
    }
}
