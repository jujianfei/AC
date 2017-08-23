using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 微软 Access 数据库。
    /// </summary>
    [DbType("Access", "微软公司 Access 数据库", typeof(AccessDbIcon))]
    public class AccessDb : IDb
    {
        /// <summary>
        /// 初始化 Access 数据库连接。
        /// </summary>
        public AccessDb()
        {
        }

        /// <summary>
        /// 使用 MDB 数据库文件路径初始化 Access 数据库连接。
        /// </summary>
        /// <param name="fileName">MDB 数据库文件路径</param>
        public AccessDb(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// 使用 MDB 数据库文件路径及数据库访问密码初始化 Access 数据库连接。
        /// </summary>
        /// <param name="fileName">MDB 数据库文件路径</param>
        /// <param name="password">数据库访问密码</param>
        public AccessDb(string fileName, string password)
        {
            this.FileName = fileName;
            this.Password = password;
        }

        /// <summary>
        /// MDB 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 打开 Access 数据库的密码
        /// </summary>
        public string Password { get; set; }

        private static DateTime CreateTableTime = DateTime.MinValue;    //最后一次创建表的时间

        #region IDb 成员

        public void CreateTable(Table table, string tableName)
        {
            CreateTableTime = DateTime.Now;
            if (tableName == null || tableName.Length == 0)
            {
                tableName = table.Code;
            }

            DAO.DBEngine daoDBE = new DAO.DBEngine();
            DAO.Database daoDB = null;
            daoDB = daoDBE.OpenDatabase(this.FileName, false, false, "MS ACCESS;PWD=" + this.Password);
            DAO.TableDefs daoTables = daoDB.TableDefs;

            foreach (DAO.TableDef daoT in daoDB.TableDefs)
            {
                if (daoT.Name.Equals(tableName))
                {
                    daoTables.Delete(tableName);                          //删除现存的表
                }
            }

            DAO.TableDef daoTable = daoDB.CreateTableDef(tableName, 0, "", "");
            string strPrimaryKeyFields = "";

            foreach (Column _Column in table.Columns)
            {
                DAO.Field daoField = new DAO.Field();
                daoField.Name = _Column.Code;

                switch (_Column.DataType)
                {
                    case DataTypeOptions.Int:
                        daoField.Type = Convert.ToInt16(DAO.DataTypeEnum.dbLong);
                        break;

                    case DataTypeOptions.Long:
                    case DataTypeOptions.Decimal:
                        daoField.Type = Convert.ToInt16(DAO.DataTypeEnum.dbDouble);
                        break;

                    case DataTypeOptions.VarChar:
                        if (_Column.DataLength > 255)
                        {
                            daoField.Type = Convert.ToInt16(DAO.DataTypeEnum.dbMemo);
                            daoField.AllowZeroLength = true;
                        }
                        else
                        {
                            daoField.Type = Convert.ToInt16(DAO.DataTypeEnum.dbText);
                            daoField.Size = _Column.DataLength;
                            daoField.AllowZeroLength = true;
                        }
                        break;

                    case DataTypeOptions.Text:
                        daoField.Type = Convert.ToInt16(DAO.DataTypeEnum.dbMemo);
                        daoField.AllowZeroLength = true;
                        break;

                    case DataTypeOptions.File:
                        daoField.Type = Convert.ToInt16(DAO.DataTypeEnum.dbLongBinary);
                        break;

                    default:
                        throw new Exception("尚未实现的数据类型 " + _Column.DataType);
                }
                daoField.Required = _Column.IsNotNull;

                if (_Column.IsPrimaryKey)
                {
                    strPrimaryKeyFields += _Column.Code + ";";
                }

                daoTable.Fields.Append(daoField);
            }

            daoDB.TableDefs.Append(daoTable);

            if (table.Name != null && table.Name.Length > 0)
            {
                DAO.Property daoTableProperty = daoTable.CreateProperty("Description", DAO.DataTypeEnum.dbText, table.Name, 0);
                daoTable.Properties.Append(daoTableProperty);
            }

            foreach (Column _Column in table.Columns)
            {
                if (_Column.Name != null && _Column.Name.Length > 0)
                {
                    DAO.Field daoField = daoTable.Fields[_Column.Code];
                    DAO.Property daoColumnProperty = daoField.CreateProperty("Description", DAO.DataTypeEnum.dbText, _Column.Name, 0);
                    daoField.Properties.Append(daoColumnProperty);
                }
            }

            if (strPrimaryKeyFields.Length > 0)
            {
                DAO.Index daoIndex = daoTable.CreateIndex("PK_" + tableName);
                daoIndex.Fields = strPrimaryKeyFields;
                daoIndex.Primary = true;
                daoIndex.Unique = true;
                daoTable.Indexes.Append(daoIndex);
            }

            foreach (Index _Index in table.Indexs)
            {
                string strKeyFields = "";
                foreach (Column _KeyColumn in _Index.Columns)
                {
                    strKeyFields += "+" + _KeyColumn.Code + ";";
                }

                if (strKeyFields.Length > 0)
                {
                    DAO.Index daoIndex = daoTable.CreateIndex(_Index.Code);
                    daoIndex.Fields = strKeyFields;
                    daoIndex.Primary = false;
                    daoIndex.Unique = false;

                    daoTable.Indexes.Append(daoIndex);
                }
            }

            daoDB.Close();

            TimeSpan ts = DateTime.Now - CreateTableTime;
            if (ts.Seconds < 2)
            {
                System.Threading.Thread.Sleep(10000);    //Access 数据表创建后需要一段时间才能访问。
            }
        }

        public bool TableIsExist(string tableName)
        {
            DAO.DBEngine daoDBE = new DAO.DBEngine();
            DAO.Database daoDB = null;
            daoDB = daoDBE.OpenDatabase(this.FileName, false, false, "MS ACCESS;PWD=" + this.Password);
            DAO.TableDefs daoTables = daoDB.TableDefs;

            foreach (DAO.TableDef daoTable in daoDB.TableDefs)
            {
                if (daoTable.Name.Equals(tableName))
                {
                    daoDB.Close();
                    return true;
                }
            }

            daoDB.Close();
            return false;
        }

        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument doc)
        {
            System.Xml.XmlNode xn = doc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnFileName = doc.CreateElement("FileName");
            xnFileName.InnerText = this.FileName;
            xn.AppendChild(xnFileName);

            if (this.Password != null && this.Password.Length > 0)
            {
                System.Xml.XmlNode xnPassword = doc.CreateElement("Password");
                xnPassword.InnerText = this.Password;
                xn.AppendChild(xnPassword);
            }

            return xn;
        }

        public void SetConfig(System.Xml.XmlNode config)
        {
            foreach (System.Xml.XmlNode xnChildren in config.ChildNodes)
            {
                if (xnChildren.Name.Equals("FileName"))
                {
                    this.FileName = xnChildren.InnerText;
                }
                else if (xnChildren.Name.Equals("Password"))
                {
                    this.Password = xnChildren.InnerText;
                }
            }
        }

        public System.Data.IDbConnection GetDbConnection()
        {
            if (this.FileName == null || this.FileName.Length == 0)
            {
                throw new Exception("未设置 Access 数据库文件路径。");
            }

            string strConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.FileName;
            if (this.Password != null && this.Password.Length > 0)
            {
                strConnection += ";Jet OLEDB:database password=" + this.Password;
            }

            return new System.Data.OleDb.OleDbConnection(strConnection);
        }

        /// <summary>
        /// 获取一个新的操作大对象数据类型的接口。
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbDataParameter GetNewDbDataParameter()
        {
            return new System.Data.OleDb.OleDbParameter();
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
        /// 创建一个 Access 数据库实例（创建 mdb 文件）
        /// </summary>
        /// <param name="fileName">欲创建的数据库文件名</param>
        /// <returns>一个 Access 实例数据库</returns>
        public static AccessDb CreateDbInstance(string fileName)
        {
            DAO.DBEngine daoDBE = new DAO.DBEngine();
            DAO.Database daoDB = daoDBE.CreateDatabase(fileName, ";LANGID=0x0804;CP=936;COUNTRY=0;", DAO.DatabaseTypeEnum.dbVersion40);
            daoDB.Close();

            return new AccessDb(fileName);
        }
    }
}
