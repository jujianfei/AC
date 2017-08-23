using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 分类-所有下级设备关联。该表保存各个分类与其直接关联的下级设备以及所有下级分类所关联的所有下级设备的对应关系。
    /// </summary>
    [Database.Table("分类-所有下级设备关联", Database.TableCreateRuleOptions.Only, "")]
    public struct ClassifyDeviceAll
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "ClassifyDeviceAll";

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
        /// 所属分类编号。用于排序时将同一分类的设备排放在一起。
        /// </summary>
        [Database.Column("所属分类编号", "", Database.DataTypeOptions.Int, false, true)]
        public const string OwnerClassifyId = "OwnerClassifyId";
    }
}
