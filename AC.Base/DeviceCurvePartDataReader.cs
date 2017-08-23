using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 设备曲线中一段曲线数据读取器。
    /// </summary>
    public abstract class DeviceCurvePartDataReader
    {
        /// <summary>
        /// 设备曲线数据读取器。
        /// </summary>
        /// <param name="device">设备集合中的第一次访问的设备对象。</param>
        /// <param name="startDate">访问数据的起始日期，如果未设置过 DateRange 日期范围则以该日期作为数据筛选的起始日期。</param>
        /// <param name="endDate">访问数据的结束日期，如果未设置过 DateRange 日期范围则以该日期作为数据筛选的结束日期。</param>
        /// <param name="startTimeNum">曲线数据段的起始时间。</param>
        /// <param name="endTimeNum">曲线数据段的结束时间。</param>
        /// <param name="curvePoint">是否将曲线数据强制转换为指定的点数。如果该值为 0 则表示不转换，设备曲线数据点数使用各设备的默认值。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        /// <param name="tableNameF">96及以下点数数据表名。</param>
        /// <param name="selectColumnNameF">96及以下点数数据表需要额外输出的字段，如无额外字段可传 null。</param>
        /// <param name="tableNameS">96以上点数数据表名。</param>
        public DeviceCurvePartDataReader(Device device, DateTime startDate, DateTime endDate, int startTimeNum, int endTimeNum, int curvePoint, CurveMergeOptions mergeOption, CurveSplitOptions splitOption, string tableNameF, string selectColumnNameF, string tableNameS)
        {
            this.m_CurvePoint = curvePoint;
            this.m_FirstDevice = device;
            this.StartTimeNum = startTimeNum;
            this.EndTimeNum = endTimeNum;

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
                    int intStartPointF = CurvePointOptions.Point96.GetPointIndex(startTimeNum) + 1;
                    int intEndPointF = CurvePointOptions.Point96.GetPointIndex(endTimeNum) + 1;
                    string strSelectF = "DateNum,DeviceId" + (selectColumnNameF != null && selectColumnNameF.Length > 0 ? "," + selectColumnNameF : "");
                    for (int intPointIndex = intStartPointF; intPointIndex <= intEndPointF; intPointIndex++)
                    {
                        strSelectF += ",Value" + intPointIndex;
                    }

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

                                string strSql = "SELECT " + strSelectF + " FROM " + strTableName + " WHERE DeviceId" + strDeviceIdWhereF + strSqlDate;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    Device deviceF = this.GetDevice(Function.ToInt(dr["DeviceId"]));
                                    CurvePointOptions _DeviceCurvePointF = this.GetDeviceCurvePoint(deviceF);
                                    if (_DeviceCurvePointF > CurvePointOptions.Point96)
                                    {
                                        _DeviceCurvePointF = CurvePointOptions.Point96;     //如果设备默认的曲线数据点数大于96点，则使用96点数据。
                                    }

                                    decimal?[] decValues = new decimal?[_DeviceCurvePointF.GetPointCount(startTimeNum, endTimeNum)];
                                    int intPointIndex = 0;
                                    for (int intIndex = intStartPointF; intIndex <= intEndPointF; intIndex++)
                                    {
                                        if (((intIndex - 1) % (96 / _DeviceCurvePointF.GetPointCount())) == 0)
                                        {
                                            decValues[intPointIndex++] = Function.ToDecimalNull(dr["Value" + intIndex]);
                                        }
                                    }

                                    if (curvePoint != 0 && curvePoint != (int)_DeviceCurvePointF)
                                    {
                                        //需要对曲线数据进行转换
                                        decValues = CurvePartValue.ConvertTo(_DeviceCurvePointF, this.StartTimeNum, decValues, (CurvePointOptions)curvePoint, mergeOption, splitOption);
                                        _DeviceCurvePointF = (CurvePointOptions)curvePoint;
                                    }

                                    this.DoDataReaderF(deviceF, dr, _DeviceCurvePointF, decValues);
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
                                strSqlDate += " AND TimeNum>=" + startTimeNum + " AND TimeNum<=" + endTimeNum;
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
        /// 曲线数据段的起始时间。
        /// </summary>
        public int StartTimeNum { get; private set; }

        /// <summary>
        /// 曲线数据段的结束时间。
        /// </summary>
        public int EndTimeNum { get; private set; }

        /// <summary>
        /// 获取当前数据读取器内的缓存数据是否适用于指定设备的某日数据。
        /// </summary>
        /// <param name="device"></param>
        /// <param name="startDate">起始日期。</param>
        /// <param name="endDate">结束日期。</param>
        /// <param name="startTimeNum">曲线数据段的起始时间。</param>
        /// <param name="endTimeNum">曲线数据段的结束时间。</param>
        /// <returns></returns>
        public bool Contains(Device device, DateTime startDate, DateTime endDate, int startTimeNum, int endTimeNum)
        {
            if (device.DateRange != null && this.DateRange.UniqueId == device.DateRange.UniqueId)
            {
                if (startTimeNum != this.StartTimeNum || endTimeNum != this.EndTimeNum)
                {
                    return false;
                }
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
        /// <param name="curvePoint">曲线点数。</param>
        /// <param name="values">经计算转换过的数据值集合。</param>
        protected abstract void DoDataReaderF(Device device, System.Data.IDataReader dr, CurvePointOptions curvePoint, decimal?[] values);

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
        /// 保存设备曲线数据段。
        /// </summary>
        /// <param name="tableTypeF">96及以下点数数据表类型声明。</param>
        /// <param name="tableNameF">96及以下点数数据表名。</param>
        /// <param name="tableTypeS">96以上点数数据表类型声明。</param>
        /// <param name="tableNameS">96以上点数数据表名。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        /// <param name="dbConnection">活动的数据库连接。</param>
        /// <param name="deviceId">设备编号。</param>
        /// <param name="date">数据起始日期及时间。使用其中的年、月、日、时、分、秒。</param>
        /// <param name="curvePoint">该数据的点数类型。</param>
        /// <param name="values">曲线数据值，该数组长度必须是 CurvePointOptions 中指定的长度之一。</param>
        /// <param name="save96Point">当 curvePoint 大于 96 点时是否另外计算一份96点的数据进行保存。当 curvePoint 小于等于 96 点时该参数无任何作用。</param>
        /// <param name="keyColumns">该数据所属数据表的额外主键字段，及这些主键字段的数值。</param>
        public static void Save(Type tableTypeF, string tableNameF, Type tableTypeS, string tableNameS, CurveMergeOptions mergeOption, CurveSplitOptions splitOption, Database.DbConnection dbConnection, int deviceId, DateTime date, CurvePointOptions curvePoint, decimal?[] values, CurvePartDataSave96PointOptions save96Point, params KeyValuePair<string, string>[] keyColumns)
        {
            CurvePartValue _CurvePartValue = new CurvePartValue(curvePoint, date.Hour * 10000 + date.Minute * 100 + date.Second, values);
            string strSql;
            string strTableName;
            bool bolSave96 = true;  //是否保存96点数据

            if (_CurvePartValue.CurvePoint > CurvePointOptions.Point96)
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
                strSql = "Delete From " + strTableName + " Where DateNum=" + Function.ToIntDate(date) + " AND TimeNum>=" + _CurvePartValue.StartTimeNum + " AND TimeNum<=" + _CurvePartValue.EndTimeNum + " AND DeviceId=" + deviceId + strWhere;
                dbConnection.ExecuteNonQuery(strSql);

                string strValue = "";
                string strColumn = "";
                foreach (KeyValuePair<string, string> kvp in keyColumns)
                {
                    strValue += "," + kvp.Value;
                    strColumn += "," + kvp.Key;
                }
                for (int intIndex = 0; intIndex < _CurvePartValue.Values.Length; intIndex++)
                {
                    try
                    {
                        strSql = Function.ToIntDate(date) + "," + _CurvePartValue.GetTimeNum(intIndex) + "," + deviceId + strValue + "," + Function.SqlDecimal(values[intIndex]);
                        strSql = "INSERT INTO " + strTableName + " (DateNum,TimeNum,DeviceId" + strColumn + ",DataValue) VALUES (" + strSql + ")";
                        dbConnection.ExecuteNonQuery(strSql);
                    }
                    catch { }
                }

                if (save96Point == CurvePartDataSave96PointOptions.No)
                {
                    bolSave96 = false;
                }
                else if (save96Point == CurvePartDataSave96PointOptions.Auto)
                {
                    if (curvePoint == CurvePointOptions.Point144)
                    {
                        if (values.Length <= 3)
                        {
                            if (((_CurvePartValue.EndTimeNum / 100) % 100) != 20 && ((_CurvePartValue.EndTimeNum / 100) % 100) != 50)
                            {
                                bolSave96 = false;
                            }
                        }
                    }
                    else
                    {
                        if (values.Length <= (curvePoint.GetPointCount() / CurvePointOptions.Point96.GetPointCount()))
                        {
                            if (curvePoint.GetTimeNum(_CurvePartValue.EndTimeNum, 1) != CurvePointOptions.Point96.GetTimeNum(_CurvePartValue.EndTimeNum, 1))
                            {
                                bolSave96 = false;
                            }
                        }
                    }
                }

                if (bolSave96)
                {
                    int intStartTimeNum96 = CurvePointOptions.Point96.FormatTimeNum(_CurvePartValue.StartTimeNum);
                    if (curvePoint == CurvePointOptions.Point144)
                    {
                        if (((_CurvePartValue.StartTimeNum / 100) % 100) == 20)
                        {
                            intStartTimeNum96 = (intStartTimeNum96 / 10000) * 10000;
                        }
                        else if (((_CurvePartValue.StartTimeNum / 100) % 100) == 50)
                        {
                            intStartTimeNum96 = (intStartTimeNum96 / 10000) * 10000 + 3000;
                        }
                    }

                    if (intStartTimeNum96 < _CurvePartValue.StartTimeNum)
                    {
                        //如果数据的起始时间不是每小时的00分、15分、30分、45分，则从数据库取出该一刻钟缺少的数据。
                        CurvePartValue _CurvePartValueTemp = new CurvePartValue(curvePoint, intStartTimeNum96, _CurvePartValue.EndTimeNum);
                        for (int intIndex = 0; intIndex < _CurvePartValue.Values.Length; intIndex++)
                        {
                            _CurvePartValueTemp.SetValue(_CurvePartValue.GetTimeNum(intIndex), _CurvePartValue.Values[intIndex]);
                        }

                        strSql = "SELECT * FROM " + strTableName + " WHERE DateNum=" + Function.ToIntDate(date) + " AND TimeNum>=" + intStartTimeNum96 + " AND TimeNum<" + _CurvePartValue.StartTimeNum + " AND DeviceId=" + deviceId + strWhere;
                        System.Data.IDataReader dr = dbConnection.ExecuteReader(strSql);
                        while (dr.Read())
                        {
                            _CurvePartValueTemp.SetValue(Function.ToInt(dr["TimeNum"]), Function.ToDecimal(dr["DataValue"]));
                        }
                        dr.Close();

                        //转换为96点数据
                        _CurvePartValueTemp.Convert(CurvePointOptions.Point96, mergeOption, splitOption);
                        _CurvePartValue = _CurvePartValueTemp;
                    }
                }
            }

            if (bolSave96)
            {
                //96点数据储存
                strTableName = Database.DbConnection.GetMonthlyName(tableNameF, date.Year, date.Month);
                if (dbConnection.TableIsExist(strTableName) == false)
                {
                    dbConnection.CreateTable(tableTypeF, strTableName);
                }

                if (_CurvePartValue.CurvePoint > CurvePointOptions.Point96)
                {
                    _CurvePartValue.Convert(CurvePointOptions.Point96, mergeOption, splitOption);
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
                    for (int intIndex = 0; intIndex < _CurvePartValue.Values.Length; intIndex++)
                    {
                        strColumnValues += "," + Function.SqlDecimal(_CurvePartValue.Values[intIndex]);
                        strColumnNames += ",Value" + ((_CurvePartValue.GetPointIndex(intIndex) * (96 / _CurvePartValue.CurvePoint.GetPointCount())) + 1);
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
                    for (int intIndex = 0; intIndex < _CurvePartValue.Values.Length; intIndex++)
                    {
                        strColumns += ",Value" + ((_CurvePartValue.GetPointIndex(intIndex) * (96 / _CurvePartValue.CurvePoint.GetPointCount())) + 1) + "=" + Function.SqlDecimal(_CurvePartValue.Values[intIndex]);
                    }
                    strSql = "UPDATE " + strTableName + " Set " + strColumns.Substring(1) + " Where DateNum=" + Function.ToIntDate(date) + " AND DeviceId=" + deviceId + strWhere;
                    dbConnection.ExecuteNonQuery(strSql);
                }
            }
        }
    }
}
