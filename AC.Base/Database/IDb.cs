using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 用于操作各种类型的数据库的接口。
    /// </summary>
    public interface IDb
    {
        /// <summary>
        /// 获取该数据库的配置，例如数据库连接信息等。该方法通常在配置数据库并完成数据库配置时调用。
        /// </summary>
        /// <returns>数据库的配置信息</returns>
        System.Xml.XmlNode GetConfig(System.Xml.XmlDocument doc);

        /// <summary>
        /// 设置数据库配置。
        /// </summary>
        /// <param name="config">数据库的配置信息</param>
        void SetConfig(System.Xml.XmlNode config);

        /// <summary>
        /// 获取数据库连接。
        /// </summary>
        /// <returns></returns>
        System.Data.IDbConnection GetDbConnection();

        /// <summary>
        /// 获取一个新的操作大对象数据类型的接口。
        /// </summary>
        /// <returns></returns>
        System.Data.IDbDataParameter GetNewDbDataParameter();

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="table">数据表结构</param>
        /// <param name="tableName">欲创建数据表的表名。如果不指定则使用 table.Code 作为表名。</param>
        void CreateTable(Table table, string tableName);

        /// <summary>
        /// 确定指定的数据表是否已创建。
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool TableIsExist(string tableName);

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
        System.Data.IDataReader GetDataReader(Database.DbConnection dbConnection, int pageSize, string selectColumns, string tableNames, string where, string order_Tablename);

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
        System.Data.IDataReader GetDataReader(Database.DbConnection dbConnection, int pageSize, int pageNum, string selectColumns, string tableNames, string where, string order_Tablename, string order_Reverse, string order);

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
        System.Data.IDataReader GetDataReader(Database.DbConnection dbConnection, int pageSize, int recordsetCount, string selectColumns, string tableNames, string where, string order_TablenameReverse, string order);

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
        System.Data.IDataReader GetMiddleDataReader(Database.DbConnection dbConnection, int startIndex, int endIndex, string selectColumns, string tableNames, string where, string order_Tablename, string order_Reverse, string order);

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
        System.Data.IDataReader GetBottomDataReader(Database.DbConnection dbConnection, int dataNum, string selectColumns, string tableNames, string where, string order_TablenameReverse, string order);
    }
}
