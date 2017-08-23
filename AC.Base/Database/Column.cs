using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 可以在数据库中进行数据表创建的数据表中的字段。
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 字段名。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 数据字段中文名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据字段备注。
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 数据类型。
        /// </summary>
        public DataTypeOptions DataType { get; set; }

        /// <summary>
        /// 数据长度，该属性仅对 Decimal、VarChar 类型的数据有效。
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// 数据精度，该属性仅对 Decimal 类型的数据有效。
        /// </summary>
        public int DataPrecision { get; set; }

        /// <summary>
        /// 获取该字段的数据类型。
        /// </summary>
        /// <returns></returns>
        public string GetDataType()
        {
            switch (this.DataType)
            {
                case DataTypeOptions.Int:
                    return "int";

                case DataTypeOptions.Long:
                    return "long";

                case DataTypeOptions.Decimal:
                    return "decimal(" + this.DataLength + "," + this.DataPrecision + ")";

                case DataTypeOptions.VarChar:
                    return "varchar(" + this.DataLength + ")";

                case DataTypeOptions.Text:
                    return "text";

                case DataTypeOptions.File:
                    return "file";

                default:
                    return "--";
            }
        }

        /// <summary>
        /// 该字段是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 该字段是否不允许空。
        /// </summary>
        public bool IsNotNull { get; set; }

        /// <summary>
        /// 输出当前对象的内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Name.Length > 0)
            {
                return this.Code + " (" + this.Name + ")";
            }
            else
            {
                return this.Code;
            }
        }
    }
}
