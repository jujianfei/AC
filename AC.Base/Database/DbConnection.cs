using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AC.Base.Database
{
    /// <summary>
    /// 应用程序数据库连接对象。
    /// </summary>
    public class DbConnection : System.Data.IDbConnection
    {
        /// <summary>
        /// 默认的数据库连接配置文件名。
        /// </summary>
        public const string DbConnectionConfigFileName = "DbConnection.xml";

        /// <summary>
        /// 保存数据库连接配置文件。
        /// </summary>
        /// <param name="configFileName"></param>
        /// <param name="db"></param>
        public static void SaveDbConnectionConfig(string configFileName, IDb db)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement("DbConnectionConfig");
            xmlDoc.AppendChild(xnConfig);

            System.Xml.XmlNode xnType = xmlDoc.CreateElement("Type");
            xnType.InnerText = db.GetType().FullName;
            xnConfig.AppendChild(xnType);

            xnConfig.AppendChild(db.GetConfig(xmlDoc));

            xmlDoc.Save(configFileName);
        }

        /// <summary>
        /// 载入数据库连接配置文件。
        /// </summary>
        /// <param name="configFileName"></param>
        /// <returns></returns>
        public static IDb LoadDbConnectionConfig(string configFileName)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(configFileName);

            if (xmlDoc.ChildNodes.Count > 0)
            {
                string strType = null;
                foreach (System.Xml.XmlNode xnItem in xmlDoc.ChildNodes[0].ChildNodes)
                {
                    if (xnItem.Name.Equals("Type"))
                    {
                        strType = xnItem.InnerText;
                    }
                    else
                    {
                        if (strType != null)
                        {
                            Type typDb = Function.GetType(strType);
                            if (typDb != null)
                            {
                                System.Reflection.ConstructorInfo ciObject = typDb.GetConstructor(new System.Type[] { });
                                object obj = ciObject.Invoke(new object[] { });

                                IDb db = obj as IDb;
                                db.SetConfig(xnItem);
                                return db;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 是否关闭使用 System.Diagnostics.Debug.WriteLine 输出执行的 SQL 语句。
        /// </summary>
        public bool IsCloseOutputSQL { get; set; }

        private IDb m_Db;
        private System.Data.IDbConnection m_Conn;

        internal DbConnection(IDb db)
        {
            this.m_Db = db;
            this.m_Conn = this.m_Db.GetDbConnection();
            this.m_Conn.Open();
        }

        /// <summary>
        /// 获取一个新的操作大对象数据类型的接口。
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbDataParameter GetNewDbDataParameter()
        {
            return this.m_Db.GetNewDbDataParameter();
        }

        #region ExecuteReader

        /// <summary>
        /// 以 SQL 文本命令方式执行无参数无事务的查询，最大超时为 30 秒，并提供从数据源读取数据行的只进流的方法。
        /// </summary>
        /// <param name="sql">要对数据源执行的 Transact-SQL 语句或存储过程。</param>
        /// <returns>通过在数据源执行命令所获得的只进结果集流</returns>
        public System.Data.IDataReader ExecuteReader(string sql)
        {
            return this.ExecuteReader(sql, -1, System.Data.CommandType.Text, null);
        }

        /// <summary>
        /// 执行查询，并提供从数据源读取数据行的只进流的方法。
        /// </summary>
        /// <param name="sql">要对数据源执行的 Transact-SQL 语句或存储过程。</param>
        /// <param name="commandTimeout">在终止执行命令的尝试并生成错误之前的等待时间。-1 表示使用默认值 30 秒。</param>
        /// <param name="commandType">该值指示如何解释 CommandText 属性。</param>
        /// <param name="dataParameters">与 Command 对象有关的参数及其到 DataSet 列的映射。如无参数可为 null。</param>
        /// <returns>通过在数据源执行命令所获得的只进结果集流</returns>
        public System.Data.IDataReader ExecuteReader(string sql, int commandTimeout, System.Data.CommandType commandType, params System.Data.IDbDataParameter[] dataParameters)
        {
#if DEBUG
            if (this.IsCloseOutputSQL == false)
            {
                System.Diagnostics.Debug.WriteLine(sql);
            }
#endif

            System.Data.IDbCommand cmd = this.m_Conn.CreateCommand();
            cmd.CommandText = sql;

            if (commandTimeout != -1)
            {
                cmd.CommandTimeout = commandTimeout;
            }

            cmd.CommandType = commandType;

            if (dataParameters != null)
            {
                foreach (System.Data.IDbDataParameter dataParameter in dataParameters)
                {
                    if (dataParameter != null)
                    {
                        cmd.Parameters.Add(dataParameter);
                    }
                }
            }

            return cmd.ExecuteReader();
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 以 SQL 文本命令方式执行无参数无事务的查询，最大超时为 30 秒，并返回查询所返回的结果集中第一行的第一列。
        /// </summary>
        /// <param name="sql">要对数据源执行的 Transact-SQL 语句或存储过程。</param>
        /// <returns>从数据库中检索的第一行、第一列的单个值（例如一个聚合值）。</returns>
        public object ExecuteScalar(string sql)
        {
            return this.ExecuteScalar(sql, -1, System.Data.CommandType.Text, null);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。
        /// </summary>
        /// <param name="sql">要对数据源执行的 Transact-SQL 语句或存储过程。</param>
        /// <param name="commandTimeout">在终止执行命令的尝试并生成错误之前的等待时间。-1 表示使用默认值 30 秒。</param>
        /// <param name="commandType">该值指示如何解释 CommandText 属性。</param>
        /// <param name="dataParameters">与 Command 对象有关的参数及其到 DataSet 列的映射。如无参数可为 null。</param>
        /// <returns>从数据库中检索的第一行、第一列的单个值（例如一个聚合值）。</returns>
        public object ExecuteScalar(string sql, int commandTimeout, System.Data.CommandType commandType, params System.Data.IDbDataParameter[] dataParameters)
        {
#if DEBUG
            if (this.IsCloseOutputSQL == false)
            {
                System.Diagnostics.Debug.WriteLine(sql);
            }
#endif

            System.Data.IDbCommand cmd = this.m_Conn.CreateCommand();
            cmd.CommandText = sql;

            if (commandTimeout != -1)
            {
                cmd.CommandTimeout = commandTimeout;
            }

            cmd.CommandType = commandType;

            if (dataParameters != null)
            {
                foreach (System.Data.IDbDataParameter dataParameter in dataParameters)
                {
                    if (dataParameter != null)
                    {
                        cmd.Parameters.Add(dataParameter);
                    }
                }
            }

            return cmd.ExecuteScalar();
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 以 SQL 文本命令方式执行无参数无事务的查询，最大超时为 30 秒，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">要对数据源执行的 Transact-SQL 语句或存储过程。</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(string sql)
        {
            return this.ExecuteNonQuery(sql, -1, System.Data.CommandType.Text, null);
        }

        /// <summary>
        /// 执行查询，并返回受影响的行数。
        /// </summary>
        /// <param name="sql">要对数据源执行的 Transact-SQL 语句或存储过程。</param>
        /// <param name="commandTimeout">在终止执行命令的尝试并生成错误之前的等待时间。-1 表示使用默认值 30 秒。</param>
        /// <param name="commandType">该值指示如何解释 CommandText 属性。</param>
        /// <param name="dataParameters">与 Command 对象有关的参数及其到 DataSet 列的映射。如无参数可为 null。</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(string sql, int commandTimeout, System.Data.CommandType commandType, params System.Data.IDbDataParameter[] dataParameters)
        {
#if DEBUG
            if (this.IsCloseOutputSQL == false)
            {
                System.Diagnostics.Debug.WriteLine(sql);
            }
#endif

            System.Data.IDbCommand cmd = this.m_Conn.CreateCommand();
            cmd.CommandText = sql;

            if (commandTimeout != -1)
            {
                cmd.CommandTimeout = commandTimeout;
            }

            cmd.CommandType = commandType;

            if (dataParameters != null)
            {
                foreach (System.Data.IDbDataParameter dataParameter in dataParameters)
                {
                    if (dataParameter != null)
                    {
                        cmd.Parameters.Add(dataParameter);
                    }
                }
            }

            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region IDbConnection 成员

        /// <summary>
        /// 以指定的 System.Data.IsolationLevel 值开始一个数据库事务。
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            return this.m_Conn.BeginTransaction(il);
        }

        /// <summary>
        /// 开始数据库事务。
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbTransaction BeginTransaction()
        {
            return this.m_Conn.BeginTransaction();
        }

        /// <summary>
        /// 为打开的 Connection 对象更改当前数据库。
        /// </summary>
        /// <param name="databaseName"></param>
        public void ChangeDatabase(string databaseName)
        {
            this.m_Conn.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 关闭数据库连接。
        /// </summary>
        public void Close()
        {
            //*** 将连接放入数据库空闲连接池

            this.m_Conn.Close();
        }

        /// <summary>
        /// 获取或设置用于打开数据库的字符串。
        /// </summary>
        public string ConnectionString
        {
            get { return this.m_Conn.ConnectionString; }
            set { this.m_Conn.ConnectionString = value; }
        }

        /// <summary>
        /// 获取在尝试建立连接时终止尝试并生成错误之前所等待的时间。
        /// </summary>
        public int ConnectionTimeout
        {
            get { return this.m_Conn.ConnectionTimeout; }
        }

        /// <summary>
        /// 创建并返回一个与该连接相关联的 Command 对象。
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbCommand CreateCommand()
        {
            return this.m_Conn.CreateCommand();
        }

        /// <summary>
        /// 获取当前数据库或连接打开后要使用的数据库的名称。
        /// </summary>
        public string Database
        {
            get { return this.m_Conn.Database; }
        }

        /// <summary>
        /// 该方法无任何作用，在调用 application.GetDbConnection() 时已经将该数据库连接打开。
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// 获取连接的当前状态。
        /// </summary>
        public System.Data.ConnectionState State
        {
            get { return this.m_Conn.State; }
        }

        #endregion

        #region 分页显示

        /// <summary>
        /// 分页显示第1页的数据。
        /// </summary>
        /// <param name="pageSize">每页显示的数据数量</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_Tablename">含有表名的排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetDataReader(int pageSize, string selectColumns, string tableNames, string where, string order_Tablename)
        {
            return this.m_Db.GetDataReader(this, pageSize, selectColumns, tableNames, where, order_Tablename);
        }

        /// <summary>
        /// 分页显示从第2页至倒数第2页的数据。
        /// </summary>
        /// <param name="pageSize">每页显示的数据数量</param>
        /// <param name="pageNum">获取第几页的数据</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_Tablename">含有表名的排序条件</param>
        /// <param name="order_Reverse">倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetDataReader(int pageSize, int pageNum, string selectColumns, string tableNames, string where, string order_Tablename, string order_Reverse, string order)
        {
            return this.m_Db.GetDataReader(this, pageSize, pageNum, selectColumns, tableNames, where, order_Tablename, order_Reverse, order);
        }

        /// <summary>
        /// 分页显示最后一页的数据。
        /// </summary>
        /// <param name="pageSize">每页显示的数据数量</param>
        /// <param name="recordsetCount">数据总数量</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_TablenameReverse">含有表名的倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetDataReader(int pageSize, int recordsetCount, string selectColumns, string tableNames, string where, string order_TablenameReverse, string order)
        {
            return this.m_Db.GetDataReader(this, pageSize, recordsetCount, selectColumns, tableNames, where, order_TablenameReverse, order);
        }

        /// <summary>
        /// 获取查询数据集中间指定索引的一部分连续数据。
        /// </summary>
        /// <param name="startIndex">该数据的起始索引，索引从 0 开始。</param>
        /// <param name="endIndex">该数据的结束索引，endIndex - startIndex + 1 = 需要获取数据的数量。</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_Tablename">含有表名的排序条件</param>
        /// <param name="order_Reverse">倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetMiddleDataReader(int startIndex, int endIndex, string selectColumns, string tableNames, string where, string order_Tablename, string order_Reverse, string order)
        {
            return this.m_Db.GetMiddleDataReader(this, startIndex, endIndex, selectColumns, tableNames, where, order_Tablename, order_Reverse, order);
        }

        /// <summary>
        /// 获取查询数据集后部的数据。
        /// </summary>
        /// <param name="dataNum">需要从数据集后部获取的数据数量。</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_TablenameReverse">含有表名的倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetBottomDataReader(int dataNum, string selectColumns, string tableNames, string where, string order_TablenameReverse, string order)
        {
            return this.m_Db.GetBottomDataReader(this, dataNum, selectColumns, tableNames, where, order_TablenameReverse, order);
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 释放连接对象。
        /// </summary>
        public void Dispose()
        {
            this.m_Conn.Dispose();
        }

        #endregion

        private static SortedList<string, bool> st_TableName = new SortedList<string, bool>();          //保存历史访问过的数据表是否存在的信息。

        /// <summary>
        /// 获取指定的数据表是否已创建。
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableIsExist(string tableName)
        {
            if (st_TableName.ContainsKey(tableName) == false)
                st_TableName.Add(tableName, this.m_Db.TableIsExist(tableName));
            
            return st_TableName[tableName];
        }

        //public bool TableIsExist(string tableName, int year)
        //{
        //    return TableIsExist(GetYearlyName(tableName, year));
        //}

        //public bool TableIsExist(string tableName, int year, int month)
        //{
        //    return TableIsExist(GetMonthlyName(tableName, year, month));
        //}

        /// <summary>
        /// 创建数据表。
        /// </summary>
        /// <param name="tableType">欲创建的数据表的类型，该类必须已添加 TableAttribute 特性，并且各字段添加了 ColumnAttribute 特性。</param>
        /// <param name="tableName">欲创建的数据表的名称。</param>
        public void CreateTable(Type tableType, string tableName)
        {
            AC.Base.Database.Table table = new Table(tableType);
            this.m_Db.CreateTable(table, tableName);

            if (st_TableName.ContainsKey(tableName))
            {
                st_TableName[tableName] = true;
            }
            else
            {
                st_TableName.Add(tableName, true);
            }

            //*** 需要增加发送广播消息代码，通知其它模块系统内新增了数据表
        }

        /// <summary>
        /// 获取每年生成一张数据表的带有年份标记的数据表名。
        /// </summary>
        /// <param name="tableName">数据表。</param>
        /// <param name="year">年份。</param>
        /// <returns></returns>
        public static string GetYearlyName(string tableName, int year)
        {
            return tableName + Function.StringInterceptFill((year % 100).ToString(), 2, "0", false);
        }

        /// <summary>
        /// 获取每月生成一张数据表的带有年月标记的数据表名。
        /// </summary>
        /// <param name="tableName">数据表。</param>
        /// <param name="year">年份。</param>
        /// <param name="month">月份。</param>
        /// <returns></returns>
        public static string GetMonthlyName(string tableName, int year, int month)
        {
            string strMonth = null;
            if (month < 10)
            {
                strMonth = month.ToString();
            }
            else if (month == 10)
            {
                strMonth = "A";
            }
            else if (month == 11)
            {
                strMonth = "B";
            }
            else
            {
                strMonth = "C";
            }
            return GetYearlyName(tableName, year) + strMonth;
        }
    }
}
