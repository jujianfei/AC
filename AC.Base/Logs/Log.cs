using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 表示为系统内的一种日志。继承该基类的实体类必须提供一个具有 ApplicationClass 参数的构造函数，且必须添加 LogTypeAttribute 特性。
    /// 使用日志时可以直接使用 new 并传入 application 对象初始化日志类。
    /// </summary>
    public abstract class Log
    {
        /// <summary>
        /// 表示为系统内的一种日志。继承该基类的实体类必须提供一个具有 ApplicationClass 参数的构造函数，且必须添加 LogTypeAttribute 特性。
        /// 使用日志时可以直接使用 new 并传入 application 对象初始化日志类。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public Log(ApplicationClass application)
        {
            this.Application = application;
        }

        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; private set; }

        /// <summary>
        /// 获取当前日志的对象来源集合。该集合内保存了搜索日志时同一批搜索的日志，保留该集合的引用为后续读取日志数据时能够以高效的批量方式将该集合内的数据一次性读取。
        /// </summary>
        internal LogCollection Source { get; set; }

        private LogType m_LogType;
        /// <summary>
        /// 当前日志的类型。
        /// </summary>
        public LogType LogType
        {
            get
            {
                if (this.m_LogType != null)
                {
                    return this.m_LogType;
                }
                else
                {
                    return this.Application.LogTypeSort.GetLogType(this.GetType());
                }
            }
            internal set
            {
                this.m_LogType = value;
            }
        }

        internal virtual void SetDataReader(System.Data.IDataReader dr)
        {
            this.m_LogId = Function.ToLong(dr[Tables.Log.LogId]);

            object objConfig = dr[Tables.Device.XMLConfig];
            if (objConfig != null && !(objConfig is System.DBNull))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                try
                {
                    xmlDoc.LoadXml(objConfig.ToString());

                    if (xmlDoc.ChildNodes.Count > 0)
                    {
                        this.SetLogConfig(xmlDoc.ChildNodes[0]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private long m_LogId;
        /// <summary>
        /// 日志 ID。该 ID 同时表示此日志的日期和时间的计时周期数，表示自 0001 年 1 月 1 日午夜 12:00:00 以来经过的以 100 纳秒为间隔的间隔数。每个计时周期表示一百纳秒，即一千万分之一秒。1 毫秒内有 10,000 个计时周期。
        /// </summary>
        public long LogId
        {
            get
            {
                return this.m_LogId;
            }
        }

        /// <summary>
        /// 获取该日志的产生时间。只有当日志成功保存后此属性方有效。
        /// </summary>
        public DateTime LogTime
        {
            get { return new DateTime(this.m_LogId); }
        }

        /// <summary>
        /// 保存当前日志。
        /// </summary>
        public virtual void Save()
        {
            this.Save(0);
        }

        /// <summary>
        /// 保存当前日志及相关的对象编号。
        /// </summary>
        /// <param name="objId">设备编号、设备分类编号，与对象无关的日志此字段存 0。</param>
        protected virtual void Save(int objId)
        {
            this.Save(objId, 0);
        }

        private static LogIdLock st_LogIdLock = new LogIdLock();

        private void Save(int objId, int tryCount)
        {
            if (this.LogId > 0)
            {
                throw new Exception("日志 " + this.LogId + " 已保存过，不能再次保存");
            }

            DateTime dtmLodId;

            lock (st_LogIdLock)
            {
                dtmLodId = st_LogIdLock.GetLogId();
            }

            string strTableName = Tables.Log.GetTableName(dtmLodId);
            Database.DbConnection dbConnection = this.Application.GetDbConnection();

            if (dbConnection.TableIsExist(strTableName) == false)
            {
                dbConnection.CreateTable(typeof(Tables.Log), strTableName);
            }

            string strConfig = null;
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

            System.Xml.XmlNode xnLogConfig = this.GetLogConfig(xmlDoc);
            if (xnLogConfig != null)
            {
                xmlDoc.AppendChild(xnLogConfig);
                strConfig = xmlDoc.OuterXml;
            }

            try
            {
                string strSql = dtmLodId.Ticks.ToString() + "," + Function.SqlStr(this.GetType().FullName, 250) + "," + objId + "," + Function.SqlStr(strConfig);
                strSql = "INSERT INTO " + strTableName + " (" + Tables.Log.LogId + "," + Tables.Log.LogType + "," + Tables.Log.ObjId + "," + Tables.Log.XMLConfig + ") VALUES (" + strSql + ")";
                dbConnection.ExecuteNonQuery(strSql);

                this.m_LogId = dtmLodId.Ticks;
            }
            catch
            {
                if (tryCount > 3)
                {
                    throw new Exception("保存日志 " + this.GetType().FullName + " 时发生错误，已重试3次。");
                }
                else
                {
                    this.Save(objId, tryCount + 1);
                }
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// 保存日志附加的整型值。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        protected void Save(string code, int ordinalNumber, int? dataValue)
        {
            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                string strTableName = Tables.LogInt.GetTableName(this.LogTime);
                if (dbConn.TableIsExist(strTableName) == false)
                {
                    dbConn.CreateTable(typeof(Tables.LogInt), strTableName);
                }

                try
                {
                    string strSql = this.LogId + "," + Function.SqlStr(code, 250) + "," + ordinalNumber + "," + Function.SqlInt(dataValue);
                    strSql = "INSERT INTO " + strTableName + " (" + Tables.LogInt.LogId + "," + Tables.LogInt.Code + "," + Tables.LogInt.OrdinalNumber + "," + Tables.LogInt.DataValue + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 保存日志附加的长整型值。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        protected void Save(string code, int ordinalNumber, long? dataValue)
        {
            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                string strTableName = Tables.LogLong.GetTableName(this.LogTime);
                if (dbConn.TableIsExist(strTableName) == false)
                {
                    dbConn.CreateTable(typeof(Tables.LogLong), strTableName);
                }

                try
                {
                    string strSql = this.LogId + "," + Function.SqlStr(code, 250) + "," + ordinalNumber + "," + Function.SqlLong(dataValue);
                    strSql = "INSERT INTO " + strTableName + " (" + Tables.LogLong.LogId + "," + Tables.LogLong.Code + "," + Tables.LogLong.OrdinalNumber + "," + Tables.LogLong.DataValue + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 保存日志附加的带小数的数值。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值，12位整数部分，4位小数部分。</param>
        protected void Save(string code, int ordinalNumber, decimal? dataValue)
        {
            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                string strTableName = Tables.LogDecimal.GetTableName(this.LogTime);
                if (dbConn.TableIsExist(strTableName) == false)
                {
                    dbConn.CreateTable(typeof(Tables.LogDecimal), strTableName);
                }

                try
                {
                    string strSql = this.LogId + "," + Function.SqlStr(code, 250) + "," + ordinalNumber + "," + Function.SqlDecimal(dataValue);
                    strSql = "INSERT INTO " + strTableName + " (" + Tables.LogDecimal.LogId + "," + Tables.LogDecimal.Code + "," + Tables.LogDecimal.OrdinalNumber + "," + Tables.LogDecimal.DataValue + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 保存日志附加的字符串值。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值，长度在 250 个字符之内。</param>
        protected void Save(string code, int ordinalNumber, string dataValue)
        {
            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                string strTableName = Tables.LogString.GetTableName(this.LogTime);
                if (dbConn.TableIsExist(strTableName) == false)
                {
                    dbConn.CreateTable(typeof(Tables.LogString), strTableName);
                }

                try
                {
                    string strSql = this.LogId + "," + Function.SqlStr(code, 250) + "," + ordinalNumber + "," + Function.SqlStr(dataValue, 250);
                    strSql = "INSERT INTO " + strTableName + " (" + Tables.LogString.LogId + "," + Tables.LogString.Code + "," + Tables.LogString.OrdinalNumber + "," + Tables.LogString.DataValue + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 获取当前日志的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns>如无配置内容则返回 null。</returns>
        public abstract System.Xml.XmlNode GetLogConfig(System.Xml.XmlDocument xmlDoc);

        /// <summary>
        /// 从保存此日志数据的 XML 文档节点初始化当前日志。
        /// </summary>
        /// <param name="logConfig">该日志对象的数据</param>
        public abstract void SetLogConfig(System.Xml.XmlNode logConfig);

        /// <summary>
        /// 从日志附加数值中初始化整型属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public abstract void SetDataValue(string code, int ordinalNumber, int? dataValue);

        /// <summary>
        /// 从日志附加数值中初始化长整型属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public abstract void SetDataValue(string code, int ordinalNumber, long? dataValue);

        /// <summary>
        /// 从日志附加数值中初始化带小数的数字属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public abstract void SetDataValue(string code, int ordinalNumber, decimal? dataValue);

        /// <summary>
        /// 从日志附加数值中初始化字符串属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public abstract void SetDataValue(string code, int ordinalNumber, string dataValue);

        /// <summary>
        /// 获取当前日志简要，该摘要返回不换行的简短日志描述。
        /// </summary>
        /// <returns></returns>
        public abstract string GetContent();

        /// <summary>
        /// 向页面输出 HTML 格式的日志内容，使用 UTF-8 编码格式。
        /// </summary>
        /// <param name="output">字符输出对象，调用 WriteLine 方法向界面输出 HTML 内容。</param>
        /// <param name="temporaryDirectory">可写入临时文件的临时目录。</param>
        /// <param name="relativePath">当前页面相对于临时目录的相对路径。</param>
        public abstract void WriterHtml(System.IO.TextWriter output, string temporaryDirectory, string relativePath);

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.LogTime.ToString("yyyy-MM-dd hh:mm:ss") + " [" + this.LogType.Name + "] " + this.GetContent();
        }

        private class LogIdLock
        {
            private int Second;
            private int Num;

            internal DateTime GetLogId()
            {
                DateTime dtm = DateTime.Now;
                if (dtm.Second == this.Second)
                {
                    this.Num++;
                    return dtm.AddMilliseconds(this.Num);
                }
                else
                {
                    this.Second = dtm.Second;
                    this.Num = 0;
                    return dtm;
                }
            }
        }
    }

    /// <summary>
    /// 日志集合。
    /// </summary>
    public class LogCollection : System.Collections.Generic.IList<Log>
    {
        private System.Collections.Generic.List<Log> Items = new List<Log>();

        #region IList<Log> 成员

        /// <summary>
        /// 搜索指定的[元素]，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Log item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// 将[元素]插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Log item)
        {
            item.Source = this;
            this.Items.Insert(index, item);
        }

        /// <summary>
        /// 移除集合的指定索引处的[元素]。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.Items[index].Source = null;
            this.Items.RemoveAt(index);
        }

        /// <summary>
        /// 获取或设置指定索引处的[元素]。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Log this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                if (this.Items[index] != null)
                {
                    this.Items[index].Source = null;
                }

                this.Items[index] = value;

                value.Source = this;
            }
        }

        #endregion

        #region ICollection<Log> 成员

        /// <summary>
        /// 将[元素]添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        public void Add(Log item)
        {
            item.Source = this;
            this.Items.Add(item);
        }

        /// <summary>
        /// 从集合中移除所有[元素]。
        /// </summary>
        public void Clear()
        {
            foreach (Log log in this)
            {
                log.Source = null;
            }
            this.Items.Clear();
        }

        /// <summary>
        /// 确定某[元素]是否在集合中。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Log item)
        {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// 将整个[元素]集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Log[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中实际包含的[元素]数。
        /// </summary>
        public int Count
        {
            get { return this.Items.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示[元素]集合是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从集合中移除特定[元素]的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Log item)
        {
            item.Source = null;
            return this.Items.Remove(item);
        }

        #endregion

        #region IEnumerable<Log> 成员

        /// <summary>
        /// 返回循环访问[元素]集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Log> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问[元素]集合的枚举数。
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }
}
