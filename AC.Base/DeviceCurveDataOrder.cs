using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 搜索设备曲线数据，并按照指定时刻对数据排序。
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public abstract class DeviceCurveDataOrder<V> : Searchs.ISearch<IList<V>>
    {
        private string m_DataTableName;
        private string[] m_TableNames;

        /// <summary>
        /// 搜索设备曲线数据，并按照指定时刻对数据排序。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="dataTableName">储存曲线数据的数据表名。</param>
        /// <param name="tableNames">其它需要关联的数据表名。</param>
        protected DeviceCurveDataOrder(ApplicationClass application, string dataTableName, params string[] tableNames)
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

        private CurvePointOptions m_CurvePoint = CurvePointOptions.Point96;
        /// <summary>
        /// 曲线点数。查询出的数据统一按照指定点数输出，该属性默认为 96 点，且该属性不能大于 96 点。
        /// </summary>
        public CurvePointOptions CurvePoint
        {
            get
            {
                return this.m_CurvePoint;
            }
            set
            {
                if (value <= CurvePointOptions.Point96)
                {
                    this.m_CurvePoint = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置按照哪个时间对数据进行排序(hhmmss)。
        /// </summary>
        public int OrderTimeNum { get; set; }

        /// <summary>
        /// 是否按照升序顺序对 OrderTimeNum 指定的时间排序。默认为按从大到小的降序顺序排序。
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
        /// 当指定的曲线小于 96 点时，如何对曲线数据进行合并。
        /// </summary>
        protected abstract CurveMergeOptions CurveMerge { get; }

        /// <summary>
        /// 处理数据库中读取到的96点曲线数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr">记录集。</param>
        /// <param name="values">经计算转换过的数据值集合。</param>
        /// <returns></returns>
        protected abstract V DoDataReader(Device device, System.Data.IDataReader dr, decimal?[] values);

        /// <summary>
        /// 搜索指定页数的曲线数据。
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

                                    decimal?[] decValues = new decimal?[CurvePointOptions.Point96.GetPointCount()];
                                    for (int intPointIndex = 0; intPointIndex < CurvePointOptions.Point96.GetPointCount(); intPointIndex++)
                                    {
                                        string strColumnName = "Value" + (intPointIndex + 1);
                                        decValues[intPointIndex] = Function.ToDecimalNull(dr[strColumnName]);
                                    }

                                    if (this.CurvePoint != CurvePointOptions.Point96)
                                    {
                                        //需要对曲线数据进行转换
                                        decValues = CurveValue.ConvertTo(decValues, (CurvePointOptions)this.CurvePoint, this.CurveMerge, CurveSplitOptions.Copy);
                                    }

                                    lstValues.Add(this.DoDataReader(device, dr, decValues));
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
                return " ORDER BY " + (enableTableName ? this.GetDataTableName() + "." : "") + "Value" + (CurvePointOptions.Point96.GetPointIndex(this.OrderTimeNum) + 1) + " DESC";
            }
            else
            {
                return " ORDER BY " + (enableTableName ? this.GetDataTableName() + "." : "") + "Value" + (CurvePointOptions.Point96.GetPointIndex(this.OrderTimeNum) + 1);
            }
        }
    }
}
