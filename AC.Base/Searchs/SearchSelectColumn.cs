using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索器中需要记录集输出的字段。
    /// </summary>
    public class SearchSelectColumn
    {
        /// <summary>
        /// 搜索器中需要记录集输出的指定表的所有字段。
        /// </summary>
        /// <param name="tableName">需要输出所有字段的表名。</param>
        public SearchSelectColumn(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// 搜索器中需要记录集输出的字段。
        /// </summary>
        /// <param name="tableName">需要输出字段的表名。</param>
        /// <param name="columnNames">需要输出的字段名。如果该字段名可能与其它表字段名重复，可以添加 AS 指定别名，例如：SELECT table.column AS column1 FROM table</param>
        public SearchSelectColumn(string tableName, params string[] columnNames)
        {
            this.TableName = tableName;
            this.ColumnNames = columnNames;
        }

        /// <summary>
        /// 需要输出字段的表名。
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 需要输出的字段名。
        /// </summary>
        public string[] ColumnNames { get; private set; }
    }
}
