using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 搜索设备日数据，并按照指定列对数据排序。
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public abstract class DeviceDayDataOrder<V> : Searchs.ISearch<IList<V>>
    {
        private string m_DataTableName;
        private string[] m_TableNames;

        /// <summary>
        /// 搜索设备日数据，并按照指定列对数据排序。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="dataTableName">储存日数据的数据表名。</param>
        /// <param name="tableNames">其它需要关联的数据表名。</param>
        protected DeviceDayDataOrder(ApplicationClass application, string dataTableName, params string[] tableNames)
        {
            this.Application = application;
            this.m_DataTableName = dataTableName;
            this.m_TableNames = tableNames;
        }

        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; private set; }

        private DateTime m_StartDate;
        /// <summary>
        /// 数据起始日期(必须)，使用该属性中的年、月、日部分。起始日期与结束日期必须为同年同月，且起始日期小于等于结束日期。
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.m_StartDate;
            }
            set
            {
                this.m_StartDate = value;
                if (this.m_StartDate > this.m_EndDate)
                {
                    this.m_EndDate = this.m_StartDate;
                }
                else if ((this.m_StartDate.Year * 12 + this.m_StartDate.Month) != (this.m_EndDate.Year * 12 + this.m_EndDate.Month))
                {
                    throw new Exception("起始日期与结束日期必须是同年同月。");
                }
            }
        }

        private DateTime m_EndDate;
        /// <summary>
        /// 数据结束日期(必须)，使用该属性中的年、月、日部分。结束日期与起始日期必须为同年同月，且结束日期大于等于起始日期。
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return this.m_EndDate;
            }
            set
            {
                this.m_EndDate = value;
                if (this.m_EndDate < this.m_StartDate)
                {
                    this.m_EndDate = this.m_StartDate;
                }
                else if ((this.m_StartDate.Year * 12 + this.m_StartDate.Month) != (this.m_EndDate.Year * 12 + this.m_EndDate.Month))
                {
                    throw new Exception("起始日期与结束日期必须是同年同月。");
                }
            }
        }

        /// <summary>
        /// 获取带有年月标记的数据表名。
        /// </summary>
        /// <returns></returns>
        protected string GetDataTableName()
        {
            return Database.DbConnection.GetMonthlyName(this.m_DataTableName, this.StartDate.Year, this.StartDate.Month);
        }

        /// <summary>
        /// 是否按照升序顺序对 OrderColumn 指定的字段排序。
        /// </summary>
        public bool IsAscend { get; set; }

        private AC.Base.DeviceSearchs.DeviceFilterCollection m_Filters;
        /// <summary>
        /// 获取或设置设备筛选器，用以对排序数据的设备范围进行筛选。
        /// </summary>
        public AC.Base.DeviceSearchs.DeviceFilterCollection Filters
        {
            get
            {
                if (this.m_Filters == null)
                {
                    this.m_Filters = new DeviceSearchs.DeviceFilterCollection();
                    this.m_Filters.SetApplication(this.Application);
                }
                return this.m_Filters;
            }
            set
            {
                this.m_Filters = value;
            }
        }

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

        /// <summary>
        /// 获取额外的筛选条件。
        /// </summary>
        /// <returns></returns>
        protected abstract string GetSqlWhere();

        /// <summary>
        /// 获取排序的字段。
        /// </summary>
        protected abstract string GetOrderColumn();

        /// <summary>
        /// 处理数据库中读取到的日数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr">记录集。</param>
        /// <returns></returns>
        protected abstract V DoDataReader(Device device, System.Data.IDataReader dr);

        /// <summary>
        /// 搜索指定页数的日数据。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。</param>
        /// <returns></returns>
        public IList<V> Search(int pageNum)
        {
            List<V> lstValues = new List<V>();

            this.PageNum = pageNum;
            this.RecordsetCount = 0;
            this.PageCount = 0;

            Database.DbConnection dbConn = this.Application.GetDbConnection();

            if (dbConn == null)
            {
                throw new Exception("当前系统未配置数据库连接，无法使用搜索功能。");
            }

            try
            {
                if (dbConn.TableIsExist(this.GetDataTableName()))
                {
                    string strWhere = Tables.Device.TableName + "." + Tables.Device.DeviceId + "=" + this.GetDataTableName() + ".DeviceId";
                    if (this.StartDate == this.EndDate)
                    {
                        strWhere += " AND " + this.GetDataTableName() + ".DateNum=" + Function.ToIntDate(this.StartDate);
                    }
                    else
                    {
                        strWhere += " AND " + this.GetDataTableName() + ".DateNum>=" + Function.ToIntDate(this.StartDate) + " AND " + this.GetDataTableName() + ".DateNum<=" + Function.ToIntDate(this.EndDate);
                    }
                    strWhere += this.GetSqlWhere();

                    if (this.Filters.Count > 0)
                    {
                        string strFilterWhere = this.Filters.GetFilterSql(true);
                        if (strFilterWhere != null && strFilterWhere.Length > 0)
                        {
                            strWhere += " AND (" + strFilterWhere + ")";
                        }
                    }

                    if (strWhere.Length > 0)
                    {
                        strWhere = " WHERE " + strWhere;
                    }

                    string strTableNames = "";
                    if (this.m_TableNames != null && this.m_TableNames.Length > 0)
                    {
                        foreach (string name in this.m_TableNames)
                        {
                            strTableNames += "," + name;
                        }
                    }

                    string strSql;
                    // 查询结果记录集总数。
                    strSql = "SELECT COUNT(" + this.GetDataTableName() + ".DeviceId) FROM " + Tables.Device.TableName + strTableNames + "," + this.GetDataTableName() + strWhere;
                    this.RecordsetCount = Function.ToInt(dbConn.ExecuteScalar(strSql));

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

                        System.Data.IDataReader dr = null;
                        if (this.PageCount == 1)
                        {
                            strSql = "SELECT " + this.GetSelectColumn() + " FROM " + Tables.Device.TableName + strTableNames + "," + this.GetDataTableName() + strWhere + this.GetOrder(true, this.IsAscend ? false : true);
                            dr = dbConn.ExecuteReader(strSql);
                        }
                        else if (this.PageCount > 1)
                        {
                            if (this.PageNum == 1)
                            {
                                dr = dbConn.GetDataReader(this.PageSize, this.GetSelectColumn(), Tables.Device.TableName + strTableNames + "," + this.GetDataTableName(), strWhere, this.GetOrder(true, this.IsAscend ? false : true));
                            }
                            else if (this.PageNum == this.PageCount)
                            {
                                dr = dbConn.GetDataReader(this.PageSize, this.RecordsetCount, this.GetSelectColumn(), Tables.Device.TableName + strTableNames + "," + this.GetDataTableName(), strWhere, this.GetOrder(true, this.IsAscend ? true : false), this.GetOrder(false, this.IsAscend ? false : true));
                            }
                            else
                            {
                                dr = dbConn.GetDataReader(this.PageSize, this.PageNum, this.GetSelectColumn(), Tables.Device.TableName + strTableNames + "," + this.GetDataTableName(), strWhere, this.GetOrder(true, this.IsAscend ? false : true), this.GetOrder(false, this.IsAscend ? true : false), this.GetOrder(false, this.IsAscend ? false : true));
                            }
                        }

                        if (dr != null)
                        {
                            DeviceCollection devices = new DeviceCollection(true);

                            while (dr.Read())
                            {
                                int intDeviceId = Function.ToInt(dr[Tables.Device.DeviceId]);
                                Device device = this.Application.GetDeviceInstance(intDeviceId);
                                if (device != null)
                                {
                                    device.SetDataReader(dr);
                                }
                                else
                                {
                                    DeviceType deviceType = this.Application.DeviceTypeSort.GetDeviceType(Function.ToString(dr[Tables.Device.DeviceType]));
                                    if (deviceType != null)
                                    {
                                        device = deviceType.CreateDevice();
                                        device.SetApplication(this.Application);
                                        device.SetDataReader(dr);

                                        this.Application.SetDeviceInstance(device);
                                    }
                                }
                                if (device != null)
                                {
                                    devices.Add(device);

                                    lstValues.Add(this.DoDataReader(device, dr));
                                }
                            }
                            dr.Close();
                        }
                    }
                }
                else
                {
                    this.RecordsetCount = 0;
                    this.PageCount = 0;
                    this.PageNum = 0;
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

            return lstValues;
        }

        #endregion

        private string GetSelectColumn()
        {
            return Tables.Device.TableName + "." + Tables.Device.DeviceType + "," +
                Tables.Device.TableName + "." + Tables.Device.ParentId + "," +
                Tables.Device.TableName + "." + Tables.Device.Name + "," +
                Tables.Device.TableName + "." + Tables.Device.NameShortcut + "," +
                Tables.Device.TableName + "." + Tables.Device.DeviceAddress + "," +
                Tables.Device.TableName + "." + Tables.Device.Identifier + "," +
                Tables.Device.TableName + "." + Tables.Device.StateOption + "," +
                Tables.Device.TableName + "." + Tables.Device.StateDescription + "," +
                Tables.Device.TableName + "." + Tables.Device.OrdinalNumber + "," +
                Tables.Device.TableName + "." + Tables.Device.Longitude + "," +
                Tables.Device.TableName + "." + Tables.Device.Latitude + "," +
                Tables.Device.TableName + "." + Tables.Device.XMLConfig + "," +
                this.GetDataTableName() + ".*";
        }

        private string GetOrder(bool enableTableName, bool isReverse)
        {
            if (isReverse)
            {
                return " ORDER BY " + (enableTableName ? this.GetDataTableName() + "." : "") + this.GetOrderColumn() + " DESC";
            }
            else
            {
                return " ORDER BY " + (enableTableName ? this.GetDataTableName() + "." : "") + this.GetOrderColumn();
            }
        }
    }
}
