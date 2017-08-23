using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 读取每年创建一张数据表的设备数据。
    /// </summary>
    public abstract class DeviceMonthDataReader
    {
        /// <summary>
        /// 读取每年创建一张数据表的设备数据。
        /// </summary>
        /// <param name="device">设备集合中的第一次访问的设备对象。</param>
        /// <param name="startDate">访问数据的起始日期，如果未设置过 DateRange 日期范围则以该日期作为数据筛选的起始日期。</param>
        /// <param name="endDate">访问数据的结束日期，如果未设置过 DateRange 日期范围则以该日期作为数据筛选的结束日期。</param>
        /// <param name="tableName">数据所在的数据表名。</param>
        /// <param name="deviceIdColumnName">设备编号字段名。</param>
        /// <param name="dateNumColumnName">数据日期字段名，该字段储存的日期应该是 YYYYMMDD 格式的8位整型数字。</param>
        public DeviceMonthDataReader(Device device, DateTime startDate, DateTime endDate, string tableName, string deviceIdColumnName, string dateNumColumnName)
        {
            this.m_FirstDevice = device;

            if (device.DateRange != null && device.DateRange.ContainsMonth(startDate, endDate))
            {
                this.m_DateRange = device.DateRange;
            }
            else
            {
                this.m_DateRange = new DateRange(startDate, endDate);
            }

            string strDeviceIdWhere = "";
            if (device.Source != null && device.Source.Contains(device))
            {
                strDeviceIdWhere = " IN (" + device.Source.GetIdForString() + ")";

                foreach (Device device1 in device.Source)
                {
                    if (device1.DateRange == null || device1.DateRange.UniqueId != this.m_DateRange.UniqueId)
                    {
                        device1.DateRange = this.m_DateRange;
                    }

                    this.SetReader(device1);
                }
            }
            else
            {
                strDeviceIdWhere = "=" + device.DeviceId;

                if (device.DateRange == null || device.DateRange.UniqueId != this.m_DateRange.UniqueId)
                {
                    device.DateRange = this.m_DateRange;
                }

                this.SetReader(device);
            }

            AC.Base.Database.DbConnection dbConn = device.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    foreach (DateRange.YearlyForMonthRange range in this.DateRange.GetYearRanges())
                    {
                        string strTableName = Database.DbConnection.GetYearlyName(tableName, range.Date.Year);
                        if (dbConn.TableIsExist(strTableName))
                        {
                            string strSqlDate = range.GetSqlWhere(dateNumColumnName);
                            if (strSqlDate.Length > 0)
                            {
                                strSqlDate = " AND " + strSqlDate;
                            }
                            string strSql = "SELECT * FROM " + strTableName + " WHERE " + deviceIdColumnName + strDeviceIdWhere + strSqlDate;
                            System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                            while (dr.Read())
                            {
                                this.DoDataReader(this.GetDevice(Function.ToInt(dr[deviceIdColumnName])), dr);
                            }
                            dr.Close();
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
            }
        }

        private Device m_FirstDevice;
        /// <summary>
        /// 初始化该数据读取器的设备。
        /// </summary>
        protected Device FirstDevice
        {
            get
            {
                return this.m_FirstDevice;
            }
        }

        private DateRange m_DateRange;
        /// <summary>
        /// 获取查询设备数据时所使用的日期范围。
        /// </summary>
        public DateRange DateRange
        {
            get
            {
                return this.m_DateRange;
            }
        }

        /// <summary>
        /// 获取当前数据读取器内的缓存数据是否适用于指定设备的某日数据。
        /// </summary>
        /// <param name="device"></param>
        /// <param name="startDate">起始日期。</param>
        /// <param name="endDate">结束日期。</param>
        /// <returns></returns>
        public bool Contains(Device device, DateTime startDate, DateTime endDate)
        {
            if (device.DateRange != null && this.DateRange.UniqueId == device.DateRange.UniqueId)
            {
                return this.DateRange.ContainsMonth(startDate, endDate);
            }
            return false;
        }

        /// <summary>
        /// 获取指定编号的设备。
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        protected Device GetDevice(int deviceId)
        {
            if (this.FirstDevice.DeviceId == deviceId)
            {
                return this.FirstDevice;
            }
            else if (this.FirstDevice.Source != null)
            {
                foreach (Device device in this.FirstDevice.Source)
                {
                    if (device.DeviceId == deviceId)
                    {
                        return device;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 格式化传入的日期，确保该日期的 Day 属性为 1。
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected DateTime FormatDate(DateTime date)
        {
            if (date.Day != 1)
            {
                date = date.AddDays((date.Day - 1) * -1);
            }
            return date;
        }

        /// <summary>
        /// 为指定的设备设置数据读取器。
        /// </summary>
        /// <param name="device"></param>
        protected abstract void SetReader(Device device);

        /// <summary>
        /// 处理数据库中读取到的数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        protected abstract void DoDataReader(Device device, System.Data.IDataReader dr);
    }
}
