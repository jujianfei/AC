using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 分类
    /// </summary>
    [Database.Table("分类", Database.TableCreateRuleOptions.Only, "")]
    public struct Classify
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "Classify";

        /// <summary>
        /// 分类编号
        /// </summary>
        [Database.Column("分类编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string ClassifyId = "ClassifyId";

        /// <summary>
        /// 分类类型
        /// </summary>
        [Database.Column("分类类型", "", 250, false, true)]
        public const string ClassifyType = "ClassifyType";

        /// <summary>
        /// 上级分类编号
        /// </summary>
        [Database.Column("上级分类编号", "", Database.DataTypeOptions.Int, false, true)]
        public const string ParentId = "ParentId";

        /// <summary>
        /// 分类名称
        /// </summary>
        [Database.Column("分类名称", "", 250, false, true)]
        public const string Name = "Name";

        /// <summary>
        /// 快捷码
        /// </summary>
        [Database.Column("快捷码", "分类名短拼音", 250, false, false)]
        public const string NameShortcut = "NameShortcut";

        /// <summary>
        /// 分类标识，用于与外部关联时的标识。
        /// </summary>
        [Database.Column("分类标识", "", 250, false, false)]
        public const string Identifier = "Identifier";

        /// <summary>
        /// 排序编号
        /// </summary>
        [Database.Column("排序编号", "", Database.DataTypeOptions.Int, false, false)]
        public const string OrdinalNumber = "OrdinalNumber";

        /// <summary>
        /// 配置信息
        /// </summary>
        [Database.Column("配置信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";
    }
}
