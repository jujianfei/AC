using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 数据字段特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : System.Attribute
    {
        /// <summary>
        /// Int、Text、File 数据类型，数据字段属性。
        /// </summary>
        /// <param name="name">数据字段中文名。</param>
        /// <param name="remark">数据字段备注。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="isPrimaryKey">该字段是否为主键</param>
        /// <param name="isNotNull">该字段是否不允许空</param>
        /// <param name="indexCodes">该字段所属的索引名。一个字段可以建立多个索引，如无需建立索引则不使用该参数。</param>
        public ColumnAttribute(string name, string remark, DataTypeOptions dataType, bool isPrimaryKey, bool isNotNull, params string[] indexCodes)
        {
            this.Name = name;
            this.Remark = remark;
            this.DataType = dataType;
            this.IsPrimaryKey = isPrimaryKey;
            this.IsNotNull = isNotNull;
            this.IndexCodes = indexCodes;
        }

        /// <summary>
        /// 字符数据类型，数据字段属性。
        /// </summary>
        /// <param name="name">数据字段中文名。</param>
        /// <param name="remark">数据字段备注。</param>
        /// <param name="dataLength">最大字符长度。</param>
        /// <param name="isPrimaryKey">该字段是否为主键</param>
        /// <param name="isNotNull">该字段是否不允许空</param>
        /// <param name="indexCodes">该字段所属的索引名。一个字段可以建立多个索引，如无需建立索引则不使用该参数。</param>
        public ColumnAttribute(string name, string remark, int dataLength, bool isPrimaryKey, bool isNotNull, params string[] indexCodes)
        {
            this.Name = name;
            this.Remark = remark;
            this.DataType = DataTypeOptions.VarChar;
            this.DataLength = dataLength;
            this.IsPrimaryKey = isPrimaryKey;
            this.IsNotNull = isNotNull;
            this.IndexCodes = indexCodes;
        }

        /// <summary>
        /// 双精度数据类型，数据字段属性。
        /// </summary>
        /// <param name="name">数据字段中文名。</param>
        /// <param name="remark">数据字段备注。</param>
        /// <param name="dataLength">数据总有效位数。</param>
        /// <param name="dataPrecision">小数位数。</param>
        /// <param name="isPrimaryKey">该字段是否为主键</param>
        /// <param name="isNotNull">该字段是否不允许空</param>
        /// <param name="indexCodes">该字段所属的索引名。一个字段可以建立多个索引，如无需建立索引则不使用该参数。</param>
        public ColumnAttribute(string name, string remark, int dataLength, int dataPrecision, bool isPrimaryKey, bool isNotNull, params string[] indexCodes)
        {
            this.Name = name;
            this.Remark = remark;
            this.DataType = DataTypeOptions.Decimal;
            this.DataLength = dataLength;
            this.DataPrecision = dataPrecision;
            this.IsPrimaryKey = isPrimaryKey;
            this.IsNotNull = isNotNull;
            this.IndexCodes = indexCodes;
        }

        /// <summary>
        /// 数据字段中文名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 数据字段备注。
        /// </summary>
        public string Remark { get; private set; }

        /// <summary>
        /// 数据类型。
        /// </summary>
        public DataTypeOptions DataType { get; private set; }

        /// <summary>
        /// 数据长度，该属性仅对 Decimal、VarChar 类型的数据有效。
        /// </summary>
        public int DataLength { get; private set; }

        /// <summary>
        /// 数据精度，该属性仅对 Decimal 类型的数据有效。
        /// </summary>
        public int DataPrecision { get; private set; }

        /// <summary>
        /// 该字段是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// 该字段是否不允许空。
        /// </summary>
        public bool IsNotNull { get; private set; }

        /// <summary>
        /// 该字段所属的索引名
        /// </summary>
        public string[] IndexCodes { get; private set; }
    }
}
