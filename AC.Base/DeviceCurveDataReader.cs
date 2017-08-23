using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 设备曲线数据读取器。
    /// </summary>
    public abstract class DeviceCurveDataReader
    {
        /// <summary>
        /// 设备曲线数据读取器。
        /// </summary>
        /// <param name="device">设备集合中的第一次访问的设备对象。</param>
        /// <param name="startDate">访问数据的起始日期，如果未设置过 DateRange 日期范围则以该日期作为数据筛选的起始日期。</param>
        /// <param name="endDate">访问数据的结束日期，如果未设置过 DateRange 日期范围则以该日期作为数据筛选的结束日期。</param>
        /// <param name="curvePoint">是否将曲线数据强制转换为指定的点数。如果该值为 0 则表示不转换，设备曲线数据点数使用各设备的默认值。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        /// <param name="tableNameF">96及以下点数数据表名。</param>
        /// <param name="tableNameS">96以上点数数据表名。</param>
        public DeviceCurveDataReader(Device device, DateTime startDate, DateTime endDate, int curvePoint, CurveMergeOptions mergeOption, CurveSplitOptions splitOption, string tableNameF, string tableNameS)
        {
            this.m_CurvePoint = curvePoint;
            this.m_FirstDevice = device;

            if (device.DateRange != null && device.DateRange.ContainsDay(startDate, endDate))
            {
                this.m_DateRange = device.DateRange;
            }
            else
            {
                this.m_DateRange = new DateRange(startDate, endDate);
            }

            string strDeviceIdWhereF = "";
            string strDeviceIdWhereS = "";
            if (device.Source != null && device.Source.Contains(device))
            {
                foreach (Device device1 in device.Source)
                {
                    CurvePointOptions _CurvePoint = this.GetDeviceCurvePoint(device1);
                    if (_CurvePoint <= CurvePointOptions.Point96 || (curvePoint > 0 && curvePoint < (int)CurvePointOptions.Point96))
                    {
                        strDeviceIdWhereF += "," + device1.DeviceId;
                    }
                    else
                    {
                        strDeviceIdWhereS += "," + device1.DeviceId;
                    }

                    if (device1.DateRange == null || device1.DateRange.UniqueId != this.m_DateRange.UniqueId)
                    {
                        device1.DateRange = this.m_DateRange;
                    }

                    this.SetReader(device1);
                }

                if (strDeviceIdWhereF.Length > 0)
                {
                    strDeviceIdWhereF = " IN (" + strDeviceIdWhereF.Substring(1) + ")";
                }
                if (strDeviceIdWhereS.Length > 0)
                {
                    strDeviceIdWhereS = " IN (" + strDeviceIdWhereS.Substring(1) + ")";
                }
            }
            else
            {
                CurvePointOptions _CurvePoint = this.GetDeviceCurvePoint(device);
                if (_CurvePoint <= CurvePointOptions.Point96 || (curvePoint > 0 && curvePoint < (int)CurvePointOptions.Point96))
                {
                    strDeviceIdWhereF = "=" + device.DeviceId;
                }
                else
                {
                    strDeviceIdWhereS = "=" + device.DeviceId;
                }

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
                    foreach (DateRange.MonthlyForDayRange range in this.DateRange.GetMonthRanges())
                    {
                        if (strDeviceIdWhereF.Length > 0)
                        {
                            string strTableName = Database.DbConnection.GetMonthlyName(tableNameF, range.Date.Year, range.Date.Month);
                            if (dbConn.TableIsExist(strTableName))
                            {
                                string strSqlDate = range.GetSqlWhere("DateNum");
                                if (strSqlDate.Length > 0)
                                {
                                    strSqlDate = " AND " + strSqlDate;
                                }
                                string strSql = "SELECT * FROM " + strTableName + " WHERE DeviceId" + strDeviceIdWhereF + strSqlDate;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    Device deviceF = this.GetDevice(Function.ToInt(dr["DeviceId"]));
                                    CurvePointOptions _DeviceCurvePointF = this.GetDeviceCurvePoint(deviceF);
                                    if (_DeviceCurvePointF > CurvePointOptions.Point96)
                                    {
                                        _DeviceCurvePointF = CurvePointOptions.Point96;     //如果设备默认的曲线数据点数大于96点，则使用96点数据。
                                    }

                                    decimal?[] decValues = new decimal?[_DeviceCurvePointF.GetPointCount()];
                                    for (int intPointIndex = 0; intPointIndex < _DeviceCurvePointF.GetPointCount(); intPointIndex++)
                                    {
                                        string strColumnName = "Value" + (intPointIndex * (96 / _DeviceCurvePointF.GetPointCount()) + 1);
                                        decValues[intPointIndex] = Function.ToDecimalNull(dr[strColumnName]);
                                    }

                                    if (curvePoint != 0 && curvePoint != (int)_DeviceCurvePointF)
                                    {
                                        //需要对曲线数据进行转换
                                        decValues = CurveValue.ConvertTo(decValues, (CurvePointOptions)curvePoint, mergeOption, splitOption);
                                    }

                                    this.DoDataReaderF(deviceF, dr, decValues);
                                }
                                dr.Close();
                            }
                        }

                        if (strDeviceIdWhereS.Length > 0)
                        {
                            string strTableName = Database.DbConnection.GetMonthlyName(tableNameS, range.Date.Year, range.Date.Month);
                            if (dbConn.TableIsExist(strTableName))
                            {
                                string strSqlDate = range.GetSqlWhere("DateNum");
                                if (strSqlDate.Length > 0)
                                {
                                    strSqlDate = " AND " + strSqlDate;
                                }
                                string strSql = "SELECT * FROM " + strTableName + " WHERE DeviceId" + strDeviceIdWhereS + strSqlDate;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    this.DoDataReaderS(this.GetDevice(Function.ToInt(dr["DeviceId"])), dr);
                                }
                                dr.Close();
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

                if (curvePoint > 0 && strDeviceIdWhereS.Length > 0)
                {
                    //通知继承的曲线读取器转换分秒数据点数。
                    this.DoConvertS();
                }
            }
        }

        private int m_CurvePoint;
        /// <summary>
        /// 是否将曲线数据强制转换为指定的点数。如果该值为 0 则表示不转换，设备曲线数据点数使用各设备的默认值。
        /// </summary>
        public int CurvePoint
        {
            get
            {
                return this.m_CurvePoint;
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
                return this.DateRange.ContainsDay(startDate, endDate);
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
        /// 获取指定设备默认的曲线数据点数。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        protected abstract CurvePointOptions GetDeviceCurvePoint(Device device);

        /// <summary>
        /// 为指定的设备设置数据读取器。
        /// </summary>
        /// <param name="device"></param>
        protected abstract void SetReader(Device device);

        /// <summary>
        /// 处理数据库中读取到的96点曲线数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        /// <param name="values">经计算转换过的数据值集合。</param>
        protected abstract void DoDataReaderF(Device device, System.Data.IDataReader dr, decimal?[] values);

        /// <summary>
        /// 处理数据库中读取到的按时间储存的数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        protected abstract void DoDataReaderS(Device device, System.Data.IDataReader dr);

        /// <summary>
        /// 开始将分秒曲线数据转换为指定的点数。
        /// </summary>
        protected abstract void DoConvertS();

        /// <summary>
        /// 保存设备曲线数据。
        /// </summary>
        /// <param name="tableTypeF">96及以下点数数据表类型声明。</param>
        /// <param name="tableNameF">96及以下点数数据表名。</param>
        /// <param name="tableTypeS">96以上点数数据表类型声明。</param>
        /// <param name="tableNameS">96以上点数数据表名。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        /// <param name="dbConnection">活动的数据库连接。</param>
        /// <param name="deviceId">设备编号。</param>
        /// <param name="date">数据日期。只使用其中的年、月、日。</param>
        /// <param name="values">曲线数据值，该数组长度必须是 CurvePointOptions 中指定的长度之一。</param>
        /// <param name="keyColumns">该数据所属数据表的额外主键字段，及这些主键字段的数值。</param>
        public static void Save(Type tableTypeF, string tableNameF, Type tableTypeS, string tableNameS, CurveMergeOptions mergeOption, CurveSplitOptions splitOption, Database.DbConnection dbConnection, int deviceId, DateTime date, decimal?[] values, params KeyValuePair<string, string>[] keyColumns)
        {
            CurveValue _CurveValue = new CurveValue(values);
            string strSql;
            string strTableName;

            if (_CurveValue.CurvePoint > CurvePointOptions.Point96)
            {
                //储存分秒数据表
                strTableName = Database.DbConnection.GetMonthlyName(tableNameS, date.Year, date.Month);
                if (dbConnection.TableIsExist(strTableName) == false)
                {
                    dbConnection.CreateTable(tableTypeS, strTableName);
                }

                string strWhere = "";
                foreach (KeyValuePair<string, string> kvp in keyColumns)
                {
                    strWhere += " AND " + kvp.Key + "=" + kvp.Value;
                }
                strSql = "Delete From " + strTableName + " Where DateNum=" + Function.ToIntDate(date) + " AND DeviceId=" + deviceId + strWhere;
                dbConnection.ExecuteNonQuery(strSql);

                string strValue = "";
                string strColumn = "";
                foreach (KeyValuePair<string, string> kvp in keyColumns)
                {
                    strValue += "," + kvp.Value;
                    strColumn += "," + kvp.Key;
                }
                for (int intIndex = 0; intIndex < _CurveValue.Values.Length; intIndex++)
                {
                    try
                    {
                        strSql = Function.ToIntDate(date) + "," + _CurveValue.CurvePoint.GetTimeNum(intIndex) + "," + deviceId + strValue + "," + Function.SqlDecimal(values[intIndex]);
                        strSql = "INSERT INTO " + strTableName + " (DateNum,TimeNum,DeviceId" + strColumn + ",DataValue) VALUES (" + strSql + ")";
                        dbConnection.ExecuteNonQuery(strSql);
                    }
                    catch { }
                }
            }

            //96点数据储存
            strTableName = Database.DbConnection.GetMonthlyName(tableNameF, date.Year, date.Month);
            if (dbConnection.TableIsExist(strTableName) == false)
            {
                dbConnection.CreateTable(tableTypeF, strTableName);
            }

            if (_CurveValue.CurvePoint > CurvePointOptions.Point96)
            {
                _CurveValue.Convert(CurvePointOptions.Point96, mergeOption, splitOption);
            }

            try
            {
                string strValue = "";
                string strColumn = "";
                foreach (KeyValuePair<string, string> kvp in keyColumns)
                {
                    strValue += "," + kvp.Value;
                    strColumn += "," + kvp.Key;
                }

                string strColumnValues = "";
                string strColumnNames = "";
                for (int intIndex = 0; intIndex < _CurveValue.Values.Length; intIndex++)
                {
                    strColumnValues += "," + Function.SqlDecimal(_CurveValue.Values[intIndex]);
                    strColumnNames += ",Value" + ((intIndex * (96 / _CurveValue.CurvePoint.GetPointCount())) + 1);
                }
                strSql = Function.ToIntDate(date) + "," + deviceId + strValue + strColumnValues;
                strSql = "INSERT INTO " + strTableName + " (DateNum,DeviceId" + strColumn + strColumnNames + ") VALUES (" + strSql + ")";
                dbConnection.ExecuteNonQuery(strSql);
            }
            catch
            {
                string strWhere = "";
                foreach (KeyValuePair<string, string> kvp in keyColumns)
                {
                    strWhere += " AND " + kvp.Key + "=" + kvp.Value;
                }

                string strColumns = "";
                for (int intIndex = 0; intIndex < _CurveValue.Values.Length; intIndex++)
                {
                    strColumns += ",Value" + ((intIndex * (96 / _CurveValue.CurvePoint.GetPointCount())) + 1) + "=" + Function.SqlDecimal(_CurveValue.Values[intIndex]);
                }
                strSql = "UPDATE " + strTableName + " Set " + strColumns.Substring(1) + " Where DateNum=" + Function.ToIntDate(date) + " AND DeviceId=" + deviceId + strWhere;
                dbConnection.ExecuteNonQuery(strSql);
            }
        }
    }
}
