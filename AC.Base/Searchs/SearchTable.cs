using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索器中使用的表关联关系，如果该搜索器仅对一张表进行操作则仅设置表名即可。
    /// </summary>
    public class SearchTable
    {
        /// <summary>
        /// 搜索器中所使用的表名，并且需要输出该表的所有字段。 例如 SELECT * FROM tableName1 中的 tableName1
        /// </summary>
        /// <param name="tableName">表名。</param>
        public SearchTable(string tableName)
        {
            this.TableName1 = tableName;
        }

        /// <summary>
        /// 搜索器中所使用的表名以及表与表之间的关联关系。例如 SELECT * FROM tableName1,tableName2 WHERE tableName1.columnName1=tableName2.columnName2
        /// </summary>
        /// <param name="tableName1">表1。</param>
        /// <param name="columnName1">表1中与表2关联的字段。</param>
        /// <param name="tableName2">表2。</param>
        /// <param name="columnName2">表2中与表1关联的字段。</param>
        public SearchTable(string tableName1, string columnName1, string tableName2, string columnName2)
        {
            this.TableName1 = tableName1;
            this.ColumnName1 = columnName1;
            this.TableName2 = tableName2;
            this.ColumnName2 = columnName2;
        }

        /// <summary>
        /// 搜索器中所使用的表名以及设定的某字段的默认值。例如 SELECT * FROM tableName WHERE columnName=8
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="columnName">字段名。</param>
        /// <param name="columnValue">该字段的默认值。如果是字符串，需要加单引号后传入。例如 SELECT * FROM tableName WHERE columnName='abc' 中的 ='abc'。</param>
        public SearchTable(string tableName, string columnName, string columnValue)
        {
            this.TableName1 = tableName;
            this.ColumnName1 = columnName;
            this.ColumnValue = columnValue;
        }

        /// <summary>
        /// 表1。
        /// </summary>
        public string TableName1 { get; private set; }

        /// <summary>
        /// 表2。
        /// </summary>
        public string TableName2 { get; private set; }

        /// <summary>
        /// 表1中与表2关联的字段。
        /// </summary>
        public string ColumnName1 { get; private set; }

        /// <summary>
        /// 表2中与表1关联的字段。
        /// </summary>
        public string ColumnName2 { get; private set; }

        /// <summary>
        /// 字段的默认值。
        /// </summary>
        public string ColumnValue { get; private set; }

        /// <summary>
        /// 获取用于 SQL 查询语句中的表关联条件。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。</param>
        /// <returns></returns>
        public string GetTableSql(bool enableTableName)
        {
            if (this.ColumnValue != null)
            {
                if (enableTableName)
                {
                    return this.TableName1 + "." + this.ColumnName1 + "=" + this.ColumnValue;
                }
                else
                {
                    return this.ColumnName1 + "=" + this.ColumnValue;
                }
            }
            else if (this.TableName2 != null)
            {
                if (enableTableName)
                {
                    return this.TableName1 + "." + this.ColumnName1 + "=" + this.TableName2 + "." + this.ColumnName2;
                }
                else
                {
                    return this.ColumnName1 + "=" + this.ColumnName2;
                }
            }
            else
            {
                return "";
            }
        }
    }
}
