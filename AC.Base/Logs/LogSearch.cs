using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 搜索指定时间段的日志。
    /// </summary>
    public class LogSearch : Searchs.ISearch<LogCollection>
    {
        private bool m_OrderIsDesc;

        /// <summary>
        /// 搜索指定时间段的日志。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="startTime">日志起始时间。将使用该日期的 Year、Month、Day、Hour、Minute、Second 属性对数据进行时间范围的筛选。</param>
        /// <param name="endTime">日志结束时间。将使用该日期的 Year、Month、Day、Hour、Minute、Second 属性对数据进行时间范围的筛选。</param>
        /// <param name="orderIsDesc">是否按照时间从后往前顺序排列，true:按时间从后往前排序；false:按时间从前往后排序。</param>
        public LogSearch(ApplicationClass application, DateTime startTime, DateTime endTime, bool orderIsDesc)
            : this(application, new DateRange(startTime, endTime), orderIsDesc)
        {
        }

        /// <summary>
        /// 搜索多个时间段的日志。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="dateRange">日期范围。将使用日期的 Year、Month、Day、Hour、Minute、Second 属性对数据进行时间范围的筛选。</param>
        /// <param name="orderIsDesc">是否按照时间从后往前顺序排列，true:按时间从后往前排序；false:按时间从前往后排序。</param>
        public LogSearch(ApplicationClass application, DateRange dateRange, bool orderIsDesc)
        {
            this.m_Application = application;
            this.m_DateRange = dateRange;
            this.m_OrderIsDesc = orderIsDesc;
        }

        private ApplicationClass m_Application;
        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application
        {
            get
            {
                return this.m_Application;
            }
        }

        private DateRange m_DateRange;
        /// <summary>
        /// 日志数据的时间范围。
        /// </summary>
        public DateRange DateRange
        {
            get
            {
                return this.m_DateRange;
            }
        }

        private LogFilterCollection m_Filters;
        /// <summary>
        /// 获取筛选数据的筛选器。
        /// </summary>
        public LogFilterCollection Filters
        {
            get
            {
                if (this.m_Filters == null)
                {
                    this.m_Filters = new LogFilterCollection();
                    this.m_Filters.SetApplication(this.Application);
                }
                return this.m_Filters;
            }
        }


        #region ISearch<LogCollection> 成员

        private int m_PageSize;
        /// <summary>
        /// 获取或设置每页显示的数据数量。当此属性设置为“0”时，则表示不进行分页，将所有符合条件的数据全部读取出。
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.m_PageSize;
            }
            set
            {
                if (value >= 0)
                {
                    this.m_PageSize = value;
                }
            }
        }

        private string GetSqlWhere(AC.Base.DateRange.MonthlyForTickRange range)
        {
            string strSql = " WHERE (" + range.GetSqlWhere(Tables.Log.LogId) + ")";

            string strFilter = this.Filters.GetFilterSql(false, range.Date);
            if (strFilter.Length > 0)
            {
                strSql += " AND (" + strFilter + ")";
            }

            return strSql;
        }

        private string GetOrderColumn(bool isReverse)
        {
            if (isReverse)
            {
                return " ORDER BY " + Tables.Log.LogId + " DESC";
            }
            else
            {
                return " ORDER BY " + Tables.Log.LogId;
            }
        }

        private int CountReadIndex = 0; //已读取的 Count 最后的索引位置。

        /// <summary>
        /// 开始搜索指定页数的数据。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。当此参数为“0”时，则表示不进行分页，将所有符合条件的数据全部读取出。</param>
        /// <returns>搜索结果集合</returns>
        public LogCollection Search(int pageNum)
        {
            this.CountReadIndex = 0;
            this.m_PageNum = pageNum;
            LogCollection logs = new LogCollection();
            IList<AC.Base.DateRange.MonthlyForTickRange> lstTickRanges = this.DateRange.GetMonthTickRanges();
            int[] intCounts = new int[lstTickRanges.Count];
            Database.DbConnection dbConn = this.Application.GetDbConnection();

            try
            {
                if (this.PageSize > 0)
                {
                    this.m_RecordsetCount = 0;

                    for (int intIndex = 0; intIndex < lstTickRanges.Count; intIndex++)
                    {
                        AC.Base.DateRange.MonthlyForTickRange range = lstTickRanges[intIndex];
                        string strTableName = Tables.Log.GetTableName(range.Date);
                        if (dbConn.TableIsExist(strTableName))
                        {
                            string strSql = "SELECT COUNT(*) FROM " + strTableName + this.GetSqlWhere(range);
                            System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                            if (dr.Read())
                            {
                                intCounts[intIndex] = Function.ToInt(dr[0]);
                                this.m_RecordsetCount += intCounts[intIndex];
                            }
                        }
                    }

                    this.m_PageCount = this.RecordsetCount / this.PageSize;
                    if ((this.RecordsetCount % this.PageSize) > 0)
                    {
                        this.m_PageCount++;
                    }

                    if (this.PageNum < 1)
                    {
                        this.m_PageNum = 1;
                    }
                    if (this.PageNum > this.PageCount)
                    {
                        this.m_PageNum = this.PageCount;
                    }
                }

                if (this.PageSize == 0 || this.PageCount == 1)
                {
                    this.m_RecordsetCount = 0;
                    if (this.m_OrderIsDesc)
                    {
                        for (int intCountIndex = lstTickRanges.Count - 1; intCountIndex >= 0; intCountIndex--)
                        {
                            AC.Base.DateRange.MonthlyForTickRange tickRange = lstTickRanges[intCountIndex];
                            string strTableName = Tables.Log.GetTableName(tickRange.Date);
                            if (dbConn.TableIsExist(strTableName))
                            {
                                string strSql = "SELECT * FROM " + strTableName + this.GetSqlWhere(tickRange) + this.GetOrderColumn(this.m_OrderIsDesc);
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    this.m_RecordsetCount++;
                                    logs.Add(DrToLog(dr));
                                }
                                dr.Close();
                            }
                        }
                    }
                    else
                    {
                        for (int intCountIndex = 0; intCountIndex < lstTickRanges.Count; intCountIndex++)
                        {
                            AC.Base.DateRange.MonthlyForTickRange tickRange = lstTickRanges[intCountIndex];
                            string strTableName = Tables.Log.GetTableName(tickRange.Date);
                            if (dbConn.TableIsExist(strTableName))
                            {
                                string strSql = "SELECT * FROM " + strTableName + this.GetSqlWhere(tickRange) + this.GetOrderColumn(this.m_OrderIsDesc);
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    this.m_RecordsetCount++;
                                    logs.Add(DrToLog(dr));
                                }
                                dr.Close();
                            }
                        }
                    }

                    if (this.m_RecordsetCount == 0)
                    {
                        this.m_PageCount = 0;
                        this.m_PageNum = 0;
                    }
                    else
                    {
                        this.m_PageCount = 1;
                        this.m_PageNum = 1;
                    }
                }
                else
                {
                    int intRsStartIndex = this.RecordsetStartIndex;
                    int intRsEndIndex = this.RecordsetEndIndex;

                    if (this.m_OrderIsDesc)
                    {
                        for (int intCountIndex = intCounts.Length - 1; intCountIndex >= 0; intCountIndex--)
                        {
                            this.Search(dbConn, logs, lstTickRanges, intCounts, intCountIndex, intRsStartIndex, intRsEndIndex);
                        }
                    }
                    else
                    {
                        for (int intCountIndex = 0; intCountIndex < intCounts.Length; intCountIndex++)
                        {
                            this.Search(dbConn, logs, lstTickRanges, intCounts, intCountIndex, intRsStartIndex, intRsEndIndex);
                        }
                    }
                }

                for (int intIndex = 0; intIndex < lstTickRanges.Count; intIndex++)
                {
                    string strLogIds = "";
                    AC.Base.DateRange.MonthlyForTickRange range = lstTickRanges[intIndex];
                    foreach (Log log in logs)
                    {
                        if (range.Contains(log.LogId))
                        {
                            strLogIds += "," + log.LogId.ToString();
                        }
                    }

                    if (strLogIds.Length > 0)
                    {
                        strLogIds = strLogIds.Substring(1);

                        string strTableName;

                        strTableName = Tables.LogInt.GetTableName(range.Date);
                        if (dbConn.TableIsExist(strTableName))
                        {
                            string strSql = "SELECT * FROM " + strTableName + " WHERE " + Tables.LogInt.LogId + " IN (" + strLogIds + ")";
                            System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                            while (dr.Read())
                            {
                                long lngLogId = Function.ToLong(dr[Tables.LogInt.LogId]);
                                foreach (Log log in logs)
                                {
                                    if (log.LogId == lngLogId)
                                    {
                                        log.SetDataValue(Function.ToString(dr[Tables.LogInt.Code]), Function.ToInt(dr[Tables.LogInt.OrdinalNumber]), Function.ToIntNull(dr[Tables.LogInt.DataValue]));
                                        break;
                                    }
                                }
                            }
                        }

                        strTableName = Tables.LogLong.GetTableName(range.Date);
                        if (dbConn.TableIsExist(strTableName))
                        {
                            string strSql = "SELECT * FROM " + strTableName + " WHERE " + Tables.LogLong.LogId + " IN (" + strLogIds + ")";
                            System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                            while (dr.Read())
                            {
                                long lngLogId = Function.ToLong(dr[Tables.LogLong.LogId]);
                                foreach (Log log in logs)
                                {
                                    if (log.LogId == lngLogId)
                                    {
                                        log.SetDataValue(Function.ToString(dr[Tables.LogLong.Code]), Function.ToInt(dr[Tables.LogLong.OrdinalNumber]), Function.ToLongNull(dr[Tables.LogLong.DataValue]));
                                        break;
                                    }
                                }
                            }
                        }

                        strTableName = Tables.LogDecimal.GetTableName(range.Date);
                        if (dbConn.TableIsExist(strTableName))
                        {
                            string strSql = "SELECT * FROM " + strTableName + " WHERE " + Tables.LogDecimal.LogId + " IN (" + strLogIds + ")";
                            System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                            while (dr.Read())
                            {
                                long lngLogId = Function.ToLong(dr[Tables.LogDecimal.LogId]);
                                foreach (Log log in logs)
                                {
                                    if (log.LogId == lngLogId)
                                    {
                                        log.SetDataValue(Function.ToString(dr[Tables.LogDecimal.Code]), Function.ToInt(dr[Tables.LogDecimal.OrdinalNumber]), Function.ToDecimalNull(dr[Tables.LogDecimal.DataValue]));
                                        break;
                                    }
                                }
                            }
                        }

                        strTableName = Tables.LogString.GetTableName(range.Date);
                        if (dbConn.TableIsExist(strTableName))
                        {
                            string strSql = "SELECT * FROM " + strTableName + " WHERE " + Tables.LogString.LogId + " IN (" + strLogIds + ")";
                            System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                            while (dr.Read())
                            {
                                long lngLogId = Function.ToLong(dr[Tables.LogString.LogId]);
                                foreach (Log log in logs)
                                {
                                    if (log.LogId == lngLogId)
                                    {
                                        log.SetDataValue(Function.ToString(dr[Tables.LogString.Code]), Function.ToInt(dr[Tables.LogString.OrdinalNumber]), Function.ToString(dr[Tables.LogString.DataValue]));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbConn.Close();
            }
            return logs;
        }

        private void Search(Database.DbConnection dbConn, LogCollection logs, IList<AC.Base.DateRange.MonthlyForTickRange> tickRanges, int[] counts, int countIndex, int startIndex, int endIndex)
        {
            if (counts[countIndex] > 0 && this.CountReadIndex <= endIndex)
            {
                AC.Base.DateRange.MonthlyForTickRange tickRange = tickRanges[countIndex];
                System.Data.IDataReader dr = null;

                if (startIndex <= this.CountReadIndex && (this.CountReadIndex + counts[countIndex] - 1) <= endIndex)
                {
                    //读所有数据
                    string strSql = "SELECT * FROM " + Tables.Log.GetTableName(tickRange.Date) + this.GetSqlWhere(tickRange) + this.GetOrderColumn(this.m_OrderIsDesc);
                    dr = dbConn.ExecuteReader(strSql);
                }
                else if (startIndex <= this.CountReadIndex && (this.CountReadIndex + counts[countIndex] - 1) > endIndex)
                {
                    //读前部数据
                    dr = dbConn.GetDataReader(endIndex - this.CountReadIndex + 1, "*", Tables.Log.GetTableName(tickRange.Date), this.GetSqlWhere(tickRange), this.GetOrderColumn(this.m_OrderIsDesc));
                }
                else if (startIndex > this.CountReadIndex && (this.CountReadIndex + counts[countIndex] - 1) <= endIndex)
                {
                    //读后部数据
                    if ((this.CountReadIndex + counts[countIndex] - startIndex) > 0)
                    {
                        dr = dbConn.GetBottomDataReader((this.CountReadIndex + counts[countIndex] - startIndex), "*", Tables.Log.GetTableName(tickRange.Date), this.GetSqlWhere(tickRange), this.GetOrderColumn(!this.m_OrderIsDesc), this.GetOrderColumn(this.m_OrderIsDesc));
                    }
                }
                else if (startIndex > this.CountReadIndex && (this.CountReadIndex + counts[countIndex] - 1) > endIndex)
                {
                    //读中间数据
                    dr = dbConn.GetMiddleDataReader(startIndex - this.CountReadIndex, endIndex - this.CountReadIndex, "*", Tables.Log.GetTableName(tickRange.Date), this.GetSqlWhere(tickRange), this.GetOrderColumn(this.m_OrderIsDesc), this.GetOrderColumn(!this.m_OrderIsDesc), this.GetOrderColumn(this.m_OrderIsDesc));
                }

                if (dr != null)
                {
                    while (dr.Read())
                    {
                        logs.Add(DrToLog(dr));
                    }
                    dr.Close();
                }

                this.CountReadIndex += counts[countIndex];
            }
        }

        private Log DrToLog(System.Data.IDataReader dr)
        {
            LogType logType = this.Application.LogTypeSort.GetLogType(Function.ToString(dr[Tables.Log.LogType]));
            return logType.CreateLog(dr);
        }

        private int m_PageNum;
        /// <summary>
        /// 获取当前是第几页的数据。
        /// </summary>
        public int PageNum
        {
            get { return this.m_PageNum; }
        }

        private int m_PageCount;
        /// <summary>
        /// 获取此搜索结果数据的总页数。
        /// </summary>
        public int PageCount
        {
            get { return this.m_PageCount; }
        }

        private int m_RecordsetCount;
        /// <summary>
        /// 获取此搜索结果数据的总数据量。
        /// </summary>
        public int RecordsetCount
        {
            get { return this.m_RecordsetCount; }
        }

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
                        if (this.PageNum == this.PageCount && (this.RecordsetCount % this.PageSize) != 0)
                        {
                            return (this.PageSize * (this.PageNum - 1) - 1) + (this.PageNum == this.PageCount ? (this.RecordsetCount % this.PageSize) : 0);
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
    }
}
