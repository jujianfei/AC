using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 用于搜索器排序的字段。
    /// </summary>
    public class SearchOrderColumn
    {
        /// <summary>
        /// 用于搜索器排序的字段。
        /// </summary>
        /// <param name="tableName">用于排序的表名。</param>
        /// <param name="columnName">用于排序的列名。</param>
        public SearchOrderColumn(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
        }

        /// <summary>
        /// 用于排序的表名。
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 用于排序的列名。
        /// </summary>
        public string ColumnName { get; private set; }
    }
}
