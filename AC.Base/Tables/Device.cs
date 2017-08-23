using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Database;

namespace AC.Base.Tables
{
    /// <summary>
    /// 设备
    /// </summary>
    [Database.Table("设备", Database.TableCreateRuleOptions.Only, "")]
    public struct Device
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "Device";

        /// <summary>
        /// 设备编号
        /// </summary>
        [Database.Column("设备编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string DeviceId = "DeviceId";

        /// <summary>
        /// 设备类型
        /// </summary>
        [Database.Column("设备类型", "", 250, false, true, new string[] { "Device_DeviceType" })]
        public const string DeviceType = "DeviceType";

        /// <summary>
        /// 上级设备编号
        /// </summary>
        [Database.Column("上级设备编号", "", Database.DataTypeOptions.Int, false, true)]
        public const string ParentId = "ParentId";

        /// <summary>
        /// 设备名称
        /// </summary>
        [Database.Column("设备名称", "", 250, false, true)]
        public const string Name = "Name";

        /// <summary>
        /// 快捷码
        /// </summary>
        [Database.Column("快捷码", "设备名短拼音", 250, false, false, new string[] { "Device_NameShortcut" })]
        public const string NameShortcut = "NameShortcut";

        /// <summary>
        /// 通讯地址
        /// </summary>
        [Database.Column("通讯地址", "与该设备通讯时所使用的通讯地址。", 250, false, false, new string[] { "Device_DeviceAddress" })]
        public const string DeviceAddress = "DeviceAddress";

        /// <summary>
        /// 设备标识
        /// </summary>
        [Database.Column("设备标识", "设备标识码、条形码、资产编号等。", 250, false, false, new string[] { "Device_Identifier" })]
        public const string Identifier = "Identifier";

        /// <summary>
        /// 设备状态
        /// </summary>
        [Database.Column("设备状态", "", Database.DataTypeOptions.Int, false, false, new string[] { "Device_StateOption" })]
        public const string StateOption = "StateOption";

        /// <summary>
        /// 状态描述
        /// </summary>
        [Database.Column("状态描述", "", 250, false, false)]
        public const string StateDescription = "StateDescription";

        /// <summary>
        /// 排序编号
        /// </summary>
        [Database.Column("排序编号", "", Database.DataTypeOptions.Int, false, false)]
        public const string OrdinalNumber = "OrdinalNumber";

        /// <summary>
        /// 设备安装的经度
        /// </summary>
        [Database.Column("经度", "设备安装的经度", Database.DataTypeOptions.Decimal, false, false, new string[] { "Device_Coordinate" })]
        public const string Longitude = "Longitude";

        /// <summary>
        /// 设备安装的纬度
        /// </summary>
        [Database.Column("纬度", "设备安装的纬度", Database.DataTypeOptions.Decimal, false, false, new string[] { "Device_Coordinate" })]
        public const string Latitude = "Latitude";

        /// <summary>
        /// 配置信息
        /// </summary>
        [Database.Column("配置信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";
    }
}
