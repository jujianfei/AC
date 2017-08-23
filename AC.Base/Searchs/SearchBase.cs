using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 提供实现 ISearch 接口，可以在数据库搜索数据的基础功能。
    /// </summary>
    /// <typeparam name="C">调用 Search 方法所返回数据的集合的类型。</typeparam>
    /// <typeparam name="CO">调用 Search 方法所返回集合中的数据类型。</typeparam>
    /// <typeparam name="F">实现 IFilter 接口的筛选器类型。</typeparam>
    /// <typeparam name="O">实现 IOrder 接口的排序器类型。</typeparam>
    public abstract class SearchBase<C, CO, F, O> : ISearch<C>
        where C : System.Collections.Generic.IEnumerable<CO>
        where F : IFilter
        where O : IOrder
    {
        private List<string> m_lsTables = null;
        private string m_strTableNames = null;       //表名
        private string m_strTableRelation = null;    //表关联关系
        private string m_strSelectColumns = null;    //输出的字段

        /// <summary>
        /// 提供实现 ISearch 接口，可以在数据库搜索数据的基础功能。
        /// </summary>
        /// <param name="application"></param>
        protected SearchBase(ApplicationClass application)
        {
            this.Application = application;
        }

        private List<string> Tables
        {
            get
            {
                if (this.m_lsTables == null)
                {
                    this.DoTables();
                }
                return this.m_lsTables;
            }
        }

        private string TableNames
        {
            get
            {
                if (this.m_strTableNames == null)
                {
                    this.DoTables();
                }
                return this.m_strTableNames;
            }
        }

        private string TableRelation
        {
            get
            {
                if (this.m_strTableRelation == null)
                {
                    this.DoTables();
                }
                return this.m_strTableRelation;
            }
        }

        private string SelectColumns
        {
            get
            {
                if (this.m_strSelectColumns == null)
                {
                    this.DoTables();
                }
                return this.m_strSelectColumns;
            }
        }

        private void DoTables()
        {
            this.m_lsTables = new List<string>();
            this.m_strTableNames = "";       //表名
            this.m_strTableRelation = "";    //表关联关系
            this.m_strSelectColumns = "";    //输出的字段

            //表关联关系
            foreach (SearchTable table in this.GetTables())
            {
                if (m_lsTables.Contains(table.TableName1) == false)
                {
                    m_lsTables.Add(table.TableName1);
                }

                if (table.TableName2 != null)
                {
                    if (m_lsTables.Contains(table.TableName2) == false)
                    {
                        m_lsTables.Add(table.TableName2);
                    }
                    this.m_strTableRelation += " AND " + table.TableName1 + "." + table.ColumnName1 + "=" + table.TableName2 + "." + table.ColumnName2;
                }
            }

            foreach (SearchTable table in this.GetTables())
            {
                if (table.ColumnValue != null)
                {
                    if (m_lsTables.Count == 1)
                    {
                        this.m_strTableRelation += " AND " + table.ColumnName1 + table.ColumnValue;
                    }
                    else if (m_lsTables.Count > 1)
                    {
                        this.m_strTableRelation += " AND " + table.TableName1 + "." + table.ColumnName1 + table.ColumnValue;
                    }
                }
            }

            if (this.m_strTableRelation.Length > 0)
            {
                this.m_strTableRelation = this.m_strTableRelation.Substring(5);
            }

            //表名
            foreach (string tableName in m_lsTables)
            {
                this.m_strTableNames += "," + tableName;
            }
            this.m_strTableNames = this.m_strTableNames.Substring(1);

            //输出的字段
            foreach (SearchSelectColumn column in this.GetSelectColumn())
            {
                if (m_lsTables.Contains(column.TableName))
                {
                    if (column.ColumnNames != null && column.ColumnNames.Length > 0)
                    {
                        foreach (string columnName in column.ColumnNames)
                        {
                            if (m_lsTables.Count == 1)
                            {
                                this.m_strSelectColumns += "," + columnName;
                            }
                            else if (m_lsTables.Count > 1)
                            {
                                this.m_strSelectColumns += "," + column.TableName + "." + columnName;
                            }
                        }
                    }
                    else
                    {
                        if (m_lsTables.Count == 1)
                        {
                            this.m_strSelectColumns += ",*";
                        }
                        else if (m_lsTables.Count > 1)
                        {
                            this.m_strSelectColumns += "," + column.TableName + ".*";
                        }
                    }
                }
                else
                {
                    throw new Exception(column.TableName + " 未在 GetTables() 的表关联关系中预先定义。");
                }
            }
            if (this.m_strSelectColumns.Length > 0)
            {
                this.m_strSelectColumns = this.m_strSelectColumns.Substring(1);
            }
            else
            {
                this.m_strSelectColumns = "*";
            }
        }

        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; private set; }

        /// <summary>
        /// 获取筛选数据的筛选器。
        /// </summary>
        protected abstract FilterCollection<F> filters { get; }

        /// <summary>
        /// 获取或设置对搜索数据结果进行排序的排序器信息。
        /// </summary>
        public SearchOrderInfoCollection<O> OrderInfos { get; set; }

        #region ISearch<C> 成员

        /// <summary>
        /// 获取或设置每页显示的数据数量。当此属性设置为“0”时，则表示不进行分页，将所有符合条件的数据全部读取出。
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 获取当前是第几页的数据。
        /// </summary>
        public int PageNum { get; protected set; }

        /// <summary>
        /// 获取此搜索结果数据的总页数。
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// 获取此搜索结果数据的总数据量。
        /// </summary>
        public int RecordsetCount { get; private set; }

        /// <summary>
        /// 当前页记录集数据在整个搜索结果中的开始索引，该值从0开始。
        /// </summary>
        public int RecordsetStartIndex
        {
            get
            {
                if (this.RecordsetCount > 0)
                {
                    if (this.PageSize > 0)
                    {
                        return this.PageSize * (this.PageNum - 1);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 当前页记录集数据在整个搜索结果中的结束索引，该值从0开始。
        /// </summary>
        public int RecordsetEndIndex
        {
            get
            {
                if (this.RecordsetCount > 0)
                {
                    if (this.PageSize > 0)
                    {
                        if (this.PageNum == this.PageCount)
                        {
                            return this.PageSize * (this.PageNum - 1) + (this.RecordsetCount % this.PageSize) - 1;
                        }
                        else
                        {
                            return (this.PageSize * this.PageNum - 1);
                        }
                    }
                    else
                    {
                        return this.RecordsetCount - 1;
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取搜索器所执行SQL语句中的数据表。
        /// </summary>
        /// <returns></returns>
        public abstract SearchTable[] GetTables();

        /// <summary>
        /// 获取搜索器所执行SQL语句中所选取的字段。
        /// </summary>
        /// <returns></returns>
        public abstract SearchSelectColumn[] GetSelectColumn();

        /// <summary>
        /// 获取搜索器所执行SQL语句中默认的排序字段及排序顺序。
        /// </summary>
        /// <returns></returns>
        public abstract SearchOrderInfoCollection<O> GetDefaultOrders();

        /// <summary>
        /// 按设定的条件执行搜索。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。</param>
        /// <returns></returns>
        public abstract C Search(int pageNum);

        /// <summary>
        /// 搜索数据时所使用的数据库连接。当继承的类调用 GetDataReader() 方法后应枚举返回的 IDataReader 中的数据，并在读取完数据后关闭该数据库连接。
        /// </summary>
        protected Database.DbConnection DbConnection { get; private set; }

        /// <summary>
        /// 根据设置的筛选条件获取记录集数据。
        /// </summary>
        /// <param name="isReverse">是否对排序信息 OrderInfos 中设定的排序条件执行倒序，如果该参数为 true 将导致整个搜索结果的先后顺序发生逆转。</param>
        /// <returns></returns>
        protected System.Data.IDataReader GetDataReader(bool isReverse)
        {
            this.RecordsetCount = 0;
            this.PageCount = 0;

            this.DbConnection = this.Application.GetDbConnection();

            if (this.DbConnection == null)
            {
                throw new Exception("当前系统未配置数据库连接，无法使用搜索功能。");
            }

            string strWhere = "";
            if (this.TableRelation.Length > 0)
            {
                strWhere = this.TableRelation;
            }

            string strFilterWhere = null;
            if (this.filters.Count > 0)
            {
                strFilterWhere = this.filters.GetFilterSql(this.Tables.Count > 1);
            }
            if (strFilterWhere != null && strFilterWhere.Length > 0)
            {
                if (strWhere.Length > 0)
                {
                    strWhere += " AND (" + strFilterWhere + ")";
                }
                else
                {
                    strWhere += strFilterWhere;
                }
            }

            if (strWhere.Length > 0)
            {
                strWhere = " WHERE " + strWhere;
            }

            string strSql;
            // 查询结果记录集总数。
            strSql = "SELECT Count(*) FROM " + this.TableNames + strWhere;
            this.RecordsetCount = Function.ToInt(this.DbConnection.ExecuteScalar(strSql));

            // 计算总页数并检查当前页是否在页数范围内。
            if (this.RecordsetCount == 0)
            {
                this.PageCount = 0;
                this.PageNum = 0;
            }
            else
            {
                if (this.PageSize > 0)
                {
                    this.PageCount = this.RecordsetCount / this.PageSize;
                    if ((this.RecordsetCount % this.PageSize) > 0)
                    {
                        this.PageCount++;
                    }

                    if (this.PageNum < 1)
                    {
                        this.PageNum = 1;
                    }
                    if (this.PageNum > this.PageCount)
                    {
                        this.PageNum = this.PageCount;
                    }
                }
                else
                {
                    this.PageNum = 1;
                    this.PageCount = 1;
                }

                if (this.PageCount == 1)
                {
                    strSql = "SELECT " + this.SelectColumns + " FROM " + this.TableNames + strWhere + this.GetOrder(true, (isReverse ? true : false));
                    return this.DbConnection.ExecuteReader(strSql);
                }
                else if (this.PageCount > 1)
                {
                    if (this.PageNum == 1)
                    {
                        return this.DbConnection.GetDataReader(this.PageSize, this.SelectColumns, this.TableNames, strWhere, this.GetOrder(true, (isReverse ? true : false)));
                    }
                    else if (this.PageNum == this.PageCount)
                    {
                        return this.DbConnection.GetDataReader(this.PageSize, this.RecordsetCount, this.SelectColumns, this.TableNames, strWhere, this.GetOrder(true, (isReverse ? false : true)), this.GetOrder(false, (isReverse ? true : false)));
                    }
                    else
                    {
                        return this.DbConnection.GetDataReader(this.PageSize, this.PageNum, this.SelectColumns, this.TableNames, strWhere, this.GetOrder(true, (isReverse ? true : false)), this.GetOrder(false, (isReverse ? false : true)), this.GetOrder(false, (isReverse ? true : false)));
                    }
                }
            }

            return null;
        }

        private string GetOrder(bool enableTableName, bool isReverse)
        {
            SearchOrderInfoCollection<O> orders = this.OrderInfos;
            if ((orders == null || orders.Count == 0) && this.PageSize > 0)
            {
                // 对于不分页的查询，如果没有设置排序器，则不进行排序
                // 对于分页的查询，如果没有设置排序器，则使用默认排序器进行排序。
                orders = this.GetDefaultOrders();
            }

            String strOrder = "";
            if (orders != null)
            {
                foreach (SearchOrderInfo<O> searchOrderInfo in orders)
                {
                    bool bolDisplayTableName = this.Tables.Count > 1;
                    if (enableTableName == false)
                    {
                        bolDisplayTableName = false;
                    }

                    strOrder += "," + searchOrderInfo.GetSqlOrderBy(bolDisplayTableName, isReverse);
                }

                if (strOrder.Length > 0)
                {
                    strOrder = " ORDER BY " + strOrder.Substring(1);
                }
            }

            return strOrder;
        }
    }
}
