using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 微软 SQL Server 数据库
    /// </summary>
    [DbType("SQL Server", "微软公司 SQL Server 数据库", typeof(SQLServerDbIcon))]
    public class SQLServerDb : IDb
    {
        /// <summary>
        /// 初始化 SQL Server 数据库连接。
        /// </summary>
        public SQLServerDb()
        {
        }

        /// <summary>
        /// 使用 Windows 身份验证并指定数据库实例名及数据库名初始化 SQL Server 数据库连接。
        /// </summary>
        /// <param name="instanceName">服务器实例名</param>
        /// <param name="databaseName"></param>
        public SQLServerDb(string instanceName, string databaseName)
        {
            this.InstanceName = instanceName;
            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// 使用 SQL Server 身份验证并指定数据库实例名及数据库名初始化 SQL Server 数据库连接。
        /// </summary>
        /// <param name="instanceName">服务器实例名</param>
        /// <param name="userName">登录数据库的帐号</param>
        /// <param name="password">登录数据库的密码</param>
        /// <param name="databaseName">数据库名</param>
        public SQLServerDb(string instanceName, string userName, string password, string databaseName)
        {
            this.InstanceName = instanceName;
            this.UserName = userName;
            this.Password = password;
            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// 服务器实例名
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// 是否使用 Windows 身份验证登录
        /// </summary>
        public bool LoginSecure { get; set; }

        /// <summary>
        /// 登录数据库的帐号。只有在 LoginSecure 为 False 的情况下才可以使用 SQL Server 身份验证。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 登录数据库的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DatabaseName { get; set; }

        #region IDb 成员

        /// <summary>
        /// 获取该数据库的配置，例如数据库连接信息等。该方法通常在配置数据库并完成数据库配置时调用。
        /// </summary>
        /// <returns>数据库的配置信息</returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument doc)
        {
            System.Xml.XmlNode xn = doc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnInstanceName = doc.CreateElement("InstanceName");
            xnInstanceName.InnerText = this.InstanceName;
            xn.AppendChild(xnInstanceName);

            System.Xml.XmlNode xnLoginSecure = doc.CreateElement("LoginSecure");
            xnLoginSecure.InnerText = (this.LoginSecure ? "1" : "0");
            xn.AppendChild(xnLoginSecure);

            if (this.LoginSecure == false)
            {
                System.Xml.XmlNode xnUserName = doc.CreateElement("UserName");
                xnUserName.InnerText = this.UserName;
                xn.AppendChild(xnUserName);

                if (this.Password != null && this.Password.Length > 0)
                {
                    System.Xml.XmlNode xnPassword = doc.CreateElement("Password");
                    xnPassword.InnerText = this.Password;
                    xn.AppendChild(xnPassword);
                }
            }

            System.Xml.XmlNode xnDatabaseName = doc.CreateElement("DatabaseName");
            xnDatabaseName.InnerText = this.DatabaseName;
            xn.AppendChild(xnDatabaseName);

            return xn;
        }

        /// <summary>
        /// 设置数据库配置。
        /// </summary>
        /// <param name="config">数据库的配置信息</param>
        public void SetConfig(System.Xml.XmlNode config)
        {
            foreach (System.Xml.XmlNode xnChildren in config.ChildNodes)
            {
                if (xnChildren.Name.Equals("InstanceName"))
                {
                    this.InstanceName = xnChildren.InnerText;
                }
                else if (xnChildren.Name.Equals("LoginSecure"))
                {
                    if (xnChildren.InnerText.Equals("1"))
                    {
                        this.LoginSecure = true;
                    }
                    else
                    {
                        this.LoginSecure = false;
                    }
                }
                else if (xnChildren.Name.Equals("UserName"))
                {
                    this.UserName = xnChildren.InnerText;
                }
                else if (xnChildren.Name.Equals("Password"))
                {
                    this.Password = xnChildren.InnerText;
                }
                else if (xnChildren.Name.Equals("DatabaseName"))
                {
                    this.DatabaseName = xnChildren.InnerText;
                }
            }
        }

        /// <summary>
        /// 获取数据库连接。
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbConnection GetDbConnection()
        {
            if (this.InstanceName == null || this.InstanceName.Length == 0)
            {
                throw new Exception("未设置 SQL Server 数据库实例名。");
            }
            if (this.DatabaseName == null || this.DatabaseName.Length == 0)
            {
                throw new Exception("未设置 SQL Server 数据库名。");
            }

            if (this.LoginSecure)
            {
                return new System.Data.SqlClient.SqlConnection("Server=" + this.InstanceName + ";Integrated Security=SSPI;database=" + this.DatabaseName);
            }
            else
            {
                return new System.Data.SqlClient.SqlConnection("Server=" + this.InstanceName + ";uid=" + this.UserName + ";pwd=" + this.Password + ";database=" + this.DatabaseName);
            }
        }

        /// <summary>
        /// 获取一个新的操作大对象数据类型的接口。
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbDataParameter GetNewDbDataParameter()
        {
            return new System.Data.SqlClient.SqlParameter();
        }

        /// <summary>
        /// 分页显示第1页的数据。
        /// </summary>
        /// <param name="dbConnection">已打开的数据库连接</param>
        /// <param name="pageSize">每页显示的数据数量</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_Tablename">含有表名的排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetDataReader(Database.DbConnection dbConnection, int pageSize, string selectColumns, string tableNames, string where, string order_Tablename)
        {
            string strSql = "SELECT TOP " + pageSize + " " + selectColumns + " FROM " + tableNames + where + order_Tablename;
            return dbConnection.ExecuteReader(strSql);
        }

        /// <summary>
        /// 分页显示从第2页至倒数第2页的数据。
        /// </summary>
        /// <param name="dbConnection">已打开的数据库连接</param>
        /// <param name="pageSize">每页显示的数据数量</param>
        /// <param name="pageNum">获取第几页的数据</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_Tablename">含有表名的排序条件</param>
        /// <param name="order_Reverse">倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetDataReader(Database.DbConnection dbConnection, int pageSize, int pageNum, string selectColumns, string tableNames, string where, string order_Tablename, string order_Reverse, string order)
        {
            string strSql = "SELECT * FROM (SELECT TOP " + pageSize + " * FROM (SELECT TOP " + (pageSize * pageNum) + " " + selectColumns + " FROM " + tableNames + where + order_Tablename + ") AS T1" + order_Reverse + ") AS T2" + order;
            return dbConnection.ExecuteReader(strSql);
        }

        /// <summary>
        /// 分页显示最后一页的数据。
        /// </summary>
        /// <param name="dbConnection">已打开的数据库连接</param>
        /// <param name="pageSize">每页显示的数据数量</param>
        /// <param name="recordsetCount">数据总数量</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_TablenameReverse">含有表名的倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetDataReader(Database.DbConnection dbConnection, int pageSize, int recordsetCount, string selectColumns, string tableNames, string where, string order_TablenameReverse, string order)
        {
            int intTopCount = pageSize;
            if ((recordsetCount % pageSize) > 0)
            {
                intTopCount = recordsetCount % pageSize;
            }
            string strSql = "SELECT * FROM (SELECT TOP " + intTopCount + " " + selectColumns + " FROM " + tableNames + where + order_TablenameReverse + ") AS T2" + order;
            return dbConnection.ExecuteReader(strSql);
        }

        /// <summary>
        /// 获取查询数据集中间指定索引的一部分连续数据。
        /// </summary>
        /// <param name="dbConnection">已打开的数据库连接</param>
        /// <param name="startIndex">该数据的起始索引，索引从 0 开始。</param>
        /// <param name="endIndex">该数据的结束索引，endIndex - startIndex + 1 = 需要获取数据的数量。</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_Tablename">含有表名的排序条件</param>
        /// <param name="order_Reverse">倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetMiddleDataReader(DbConnection dbConnection, int startIndex, int endIndex, string selectColumns, string tableNames, string where, string order_Tablename, string order_Reverse, string order)
        {
            string strSql = "SELECT * FROM (SELECT TOP " + (endIndex - startIndex + 1) + " * FROM (SELECT TOP " + (endIndex + 1) + " " + selectColumns + " FROM " + tableNames + where + order_Tablename + ") AS T1" + order_Reverse + ") AS T2" + order;
            return dbConnection.ExecuteReader(strSql);
        }

        /// <summary>
        /// 获取查询数据集后部的数据。
        /// </summary>
        /// <param name="dbConnection">已打开的数据库连接</param>
        /// <param name="dataNum">需要从数据集后部获取的数据数量。</param>
        /// <param name="selectColumns">需要输出的字段</param>
        /// <param name="tableNames">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="order_TablenameReverse">含有表名的倒序排序条件</param>
        /// <param name="order">排序条件</param>
        /// <returns></returns>
        public System.Data.IDataReader GetBottomDataReader(DbConnection dbConnection, int dataNum, string selectColumns, string tableNames, string where, string order_TablenameReverse, string order)
        {
            string strSql = "SELECT * FROM (SELECT TOP " + dataNum + " " + selectColumns + " FROM " + tableNames + where + order_TablenameReverse + ") AS T2" + order;
            return dbConnection.ExecuteReader(strSql);
        }

        #endregion

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="table">数据表结构</param>
        /// <param name="tableName">欲创建数据表的表名。如果不指定则使用 table.Code 作为表名。</param>
        public void CreateTable(Table table, string tableName)
        {
            if (tableName == null || tableName.Length == 0)
            {
                tableName = table.Code;
            }

            System.Data.IDbCommand cmd;
            System.Data.IDataReader dr;
            System.Data.IDbConnection conn = this.GetDbConnection();
            conn.Open();

            //查询该表是否存在
            bool bolIsExist = false;
            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id,name FROM sysobjects WHERE xtype='U' AND status>=0 AND name='" + tableName + "'";
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bolIsExist = true;
            }
            dr.Close();

            //删除已存在的表。
            if (bolIsExist)
            {
                foreach (Index _Index in table.Indexs)
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText = "if exists (select 1 from sysindexes where id = object_id('" + tableName + "') and name = '" + _Index.Code + "' and indid > 0 and indid < 255) drop index " + tableName + "." + _Index.Code;
                    cmd.ExecuteNonQuery();
                }

                cmd = conn.CreateCommand();
                cmd.CommandText = "if exists (select 1 from sysobjects where id = object_id('" + tableName + "') and type = 'U') drop table " + tableName;
                cmd.ExecuteNonQuery();
            }

            //字段
            string strPrimaryKeyFields = "";
            string strCreateTable = "CREATE TABLE " + tableName + " (";
            foreach (Column _Column in table.Columns)
            {
                if (_Column.IsPrimaryKey)
                {
                    strPrimaryKeyFields += "," + _Column.Code;
                }

                strCreateTable += _Column.Code + " ";

                switch (_Column.DataType)
                {
                    case DataTypeOptions.Int:
                        strCreateTable += "int ";
                        break;

                    case DataTypeOptions.Long:
                        strCreateTable += "bigint ";
                        break;

                    case DataTypeOptions.Decimal:
                        if (_Column.DataLength > 0 && _Column.DataPrecision > 0)
                        {
                            strCreateTable += "decimal(" + _Column.DataLength + "," + _Column.DataPrecision + ") ";
                        }
                        else if (_Column.DataLength > 0)
                        {
                            strCreateTable += "decimal(" + _Column.DataLength + ") ";
                        }
                        else
                        {
                            strCreateTable += "decimal ";
                        }
                        break;

                    case DataTypeOptions.VarChar:
                        if (_Column.DataLength > 0)
                        {
                            strCreateTable += "varchar(" + _Column.DataLength + ") ";
                        }
                        else
                        {
                            strCreateTable += "varchar ";
                        }
                        break;

                    case DataTypeOptions.Text:
                        strCreateTable += "text ";
                        break;

                    case DataTypeOptions.File:
                        strCreateTable += "image ";
                        break;

                    default:
                        throw new Exception("尚未实现的数据类型 " + _Column.DataType);
                }

                if (_Column.IsNotNull)
                {
                    strCreateTable += "not null,";
                }
                else
                {
                    strCreateTable += "null,";
                }
            }

            //主键
            if (strPrimaryKeyFields.Length > 0)
            {
                strCreateTable += "CONSTRAINT PK_" + tableName + " PRIMARY KEY (" + strPrimaryKeyFields.Substring(1) + ")";
            }
            strCreateTable += ")";

            cmd = conn.CreateCommand();
            cmd.CommandText = strCreateTable;
            cmd.ExecuteNonQuery();

            //创建索引
            foreach (Index _Index in table.Indexs)
            {
                string strIndexColumn = "";
                foreach (Column _IndexColumn in _Index.Columns)
                {
                    strIndexColumn += _IndexColumn.Code + ",";
                }

                if (strIndexColumn.Length > 0)
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText = "CREATE INDEX " + _Index.Code + " ON " + tableName + " (" + strIndexColumn.Substring(0, strIndexColumn.Length - 1) + ")";
                    cmd.ExecuteNonQuery();
                }
            }

            conn.Close();
        }

        /// <summary>
        /// 确定指定的数据表是否已创建。
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableIsExist(string tableName)
        {
            System.Data.IDbCommand cmd;
            System.Data.IDataReader dr;
            System.Data.IDbConnection conn = this.GetDbConnection();
            conn.Open();

            //查询该表是否存在
            bool bolIsExist = false;
            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id,name FROM sysobjects WHERE xtype='U' AND status>=0 AND name='" + tableName + "'";
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bolIsExist = true;
            }
            dr.Close();

            return bolIsExist;
        }

        /// <summary>
        /// 获得网络中可用的 SQL Server 实例集合。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetSqlServerInstances()
        {
            List<string> lsNames = new List<string>();

            try
            {
                System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
                System.Data.DataTable dt = instance.GetDataSources();

                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    if (dr["InstanceName"].ToString().Length > 0)
                    {
                        lsNames.Add(dr["ServerName"].ToString() + "\\" + dr["InstanceName"].ToString());
                    }
                    else
                    {
                        lsNames.Add(dr["ServerName"].ToString());
                    }
                }
            }
            catch { }

            return lsNames;
        }

        /// <summary>
        /// 获取指定实例内的所有数据库名。
        /// </summary>
        /// <param name="instanceName">实例名。如 SQL Server 所在计算机的IP地址</param>
        /// <param name="loginSecure">是否使用 Windows 身份验证方式。如不使用此方式则需输入登录名与密码</param>
        /// <param name="username">使用 SQL Server 身份验证方式时的登录用户名</param>
        /// <param name="password">使用 SQL Server 身份验证方式时的登录密码</param>
        /// <returns></returns>
        public static IEnumerable<string> GetSqlServerDatabases(string instanceName, bool loginSecure, string username, string password)
        {
            List<string> lsNames = new List<string>();

            SQLServerDb db = new SQLServerDb();
            db.InstanceName = instanceName;
            db.LoginSecure = loginSecure;
            if (loginSecure == false)
            {
                db.UserName = username;
                db.Password = password;
            }
            db.DatabaseName = "master";

            System.Data.IDbConnection conn = db.GetDbConnection();
            conn.Open();

            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sysdatabases";
            cmd.Connection = conn;

            System.Data.IDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lsNames.Add(Function.ToTrim(dr["name"]));
            }
            dr.Close();

            conn.Close();

            return lsNames;
        }

        /// <summary>
        /// 创建 SQL Server 数据库。
        /// </summary>
        /// <param name="instanceName">数据库实例名。</param>
        /// <param name="loginSecure">是否使用 Windows 身份验证登录。</param>
        /// <param name="username">使用 SQL Server 身份验证时所使用的登录帐号。</param>
        /// <param name="password">使用 SQL Server 身份验证时所使用的密码。</param>
        /// <param name="databaseName">欲创建数据库的数据库名。</param>
        /// <param name="filePath">指定数据库文件路径。如传入 null 则使用默认路径。</param>
        /// <param name="dataFileSize">指定数据库的初始大小(MB)。</param>
        public static void CreateDatabase(string instanceName, bool loginSecure, string username, string password, string databaseName, string filePath, int dataFileSize)
        {
            SQLServerDb db = new SQLServerDb();
            db.InstanceName = instanceName;
            db.LoginSecure = loginSecure;
            if (loginSecure == false)
            {
                db.UserName = username;
                db.Password = password;
            }
            db.DatabaseName = "master";

            System.Data.IDbConnection conn = db.GetDbConnection();
            conn.Open();

            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            string strSql;

            strSql = "CREATE DATABASE " + databaseName;

            if (filePath != null && filePath.Length > 0)
            {
                if (filePath.Substring(filePath.Length - 1).Equals("\\") == false)
                {
                    filePath += "\\";
                }
                strSql += " ON (NAME=" + databaseName + ",FILENAME='" + filePath + databaseName + ".mdf',";

                if (dataFileSize > 0)
                {
                    strSql += "SIZE=" + dataFileSize.ToString() + ",";
                }
                strSql += "FILEGROWTH=10%)";
            }
            cmd.CommandText = strSql;
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
