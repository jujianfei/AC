using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 数据字段数据类型选项。
    /// </summary>
    public enum DataTypeOptions
    {
        /// <summary>
        /// Int 型数据，可储存 -2147483648 至 2147483647 之间的 4 个字节的数据。
        /// </summary>
        Int,

        /// <summary>
        /// Long 型数据，可储存 -9223372036854775808 至 9223372036854775807 之间的 8 个字节的数据。
        /// </summary>
        Long,

        /// <summary>
        /// 带有小数的数据。
        /// </summary>
        Decimal,

        /// <summary>
        /// 可变长的字符串。
        /// </summary>
        VarChar,

        /// <summary>
        /// 最大可达 2G 的长文本。
        /// </summary>
        Text,

        /// <summary>
        /// 最大可达 2G 的文件数据。
        /// </summary>
        File,
    }
}
