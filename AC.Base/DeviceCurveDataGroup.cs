using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 对设备曲线数据进行分组聚合统计。
    /// </summary>
    public abstract class DeviceCurveDataGroup
    {
        private string m_DataTableName;
        private string[] m_TableNames;

        /// <summary>
        /// 对设备曲线数据进行分组聚合统计。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="dataTableName">储存曲线数据的数据表名。</param>
        /// <param name="tableNames">其它关联的表名</param>
        protected DeviceCurveDataGroup(ApplicationClass application, string dataTableName, params string[] tableNames)
        {
            this.Application = application;
            this.m_DataTableName = dataTableName;
            if (tableNames != null)
            {
                this.m_TableNames = tableNames;
            }
            else
            {
                this.m_TableNames = new string[] { };
            }
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

        private AC.Base.DeviceSearchs.DeviceFilterCollection m_Filters;
        /// <summary>
        /// 获取或设置设备筛选器，用以对统计数据的设备范围进行筛选。
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

        /// <summary>
        /// 获取聚合统计的类型。
        /// </summary>
        /// <returns></returns>
        protected abstract DataAggregateTypeOptions GetAggregateType();

        /// <summary>
        /// 获取分组的字段名。格式为“表名.字段名1,表名.字段名2”
        /// </summary>
        /// <returns></returns>
        protected abstract string GetGroupColumnName();

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
        /// 搜索数据时所使用的数据库连接。当继承的类调用 GetDataReader() 方法后应枚举返回的 IDataReader 中的数据，并在读取完数据后关闭该数据库连接。
        /// </summary>
        protected Database.DbConnection DbConnection { get; private set; }

        /// <summary>
        /// 获取分组数据记录集。
        /// </summary>
        /// <returns></returns>
        protected System.Data.IDataReader GetDataReader()
        {
            this.DbConnection = this.Application.GetDbConnection();

            if (this.DbConnection == null)
            {
                throw new Exception("当前系统未配置数据库连接，无法使用分组统计功能。");
            }

            if (this.DbConnection.TableIsExist(this.GetDataTableName()))
            {
                string strSumColumn = "";
                for (int intIndex = 1; intIndex <= 96; intIndex++)
                {
                    strSumColumn += "," + this.GetAggregateType().GetName() + "(" + this.GetDataTableName() + ".Value" + intIndex + ") AS V" + intIndex;
                }

                string strFrom = Tables.Device.TableName + "," + this.GetDataTableName();
                foreach (string tName in this.m_TableNames)
                {
                    strFrom += "," + tName;
                }

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
                strWhere = " WHERE " + strWhere;

                string strSql = "SELECT " + this.GetGroupColumnName() + strSumColumn + " FROM " + strFrom + strWhere + " GROUP BY " + this.GetGroupColumnName();
                System.Data.IDataReader dr = this.DbConnection.ExecuteReader(strSql);
                return dr;
            }

            return null;
        }
    }
}
