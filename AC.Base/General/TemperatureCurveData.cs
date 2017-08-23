using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;
using AMS.Monitoring.General.Tables;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 日温度曲线数据。
    /// 在设备上添加 ITemperatureCurveDataDeviceImplement 接口，通过设备的 TemperatureCurveData 属性使用该对象。
    /// </summary>
    public class TemperatureCurveData
    {
        internal TemperatureCurveDataReader m_Reader;
        internal TemperatureCurvePartDataReader m_PartReader;

        /// <summary>
        /// 日温度曲线数据。表示一天从 0:00:00 至 23:59:59 这一时间段内的温度曲线。
        /// </summary>
        /// <param name="device"></param>
        public TemperatureCurveData(Device device)
        {
            this.m_Device = device;
        }

        private Device m_Device;
        /// <summary>
        /// 该数据所属的设备。
        /// </summary>
        public Device Device
        {
            get
            {
                return this.m_Device;
            }
        }

        /// <summary>
        /// 查询指定日期的温度曲线数据。
        /// </summary>
        /// <param name="date">数据日期。</param>
        /// <returns>无论该日有无数据，均返回 TemperatureCurveValue 的实例。</returns>
        public TemperatureCurveValue GetData(DateTime date)
        {
            if (this.m_Reader == null || this.m_Reader.Contains(this.Device, date, date) == false)
            {
                new TemperatureCurveDataReader(this.Device, date, date);
            }

            return this.m_Reader.GetData(this.Device, date);
        }

        /// <summary>
        /// 查询一段日期的温度曲线数据。
        /// </summary>
        /// <param name="startDate">数据起始日期。</param>
        /// <param name="endDate">数据结束日期。</param>
        /// <returns>无论有无数据，返回的日温度曲线数组长度均为起始日期与结束日期所相差的天数，并且数组内的数据按照起始日期至结束日期的顺序排列。</returns>
        public TemperatureCurveValue[] GetData(DateTime startDate, DateTime endDate)
        {
            TimeSpan tsDay = endDate - startDate;
            if (tsDay.Days < 0)
            {
                throw new Exception("结束日期不得小于起始日期。");
            }
            else
            {
                if (this.m_Reader == null || this.m_Reader.Contains(this.Device, startDate, endDate) == false)
                {
                    new TemperatureCurveDataReader(this.Device, startDate, endDate);
                }

                TemperatureCurveValue[] values = new TemperatureCurveValue[tsDay.Days + 1];

                for (int intIndex = 0; intIndex <= tsDay.Days; intIndex++)
                {
                    values[intIndex] = this.m_Reader.GetData(this.Device, startDate.AddDays(intIndex));
                }

                return values;
            }
        }

        /// <summary>
        /// 查询指定日期的温度曲线数据，并确保该曲线具有指定的点数。
        /// </summary>
        /// <param name="date">数据日期。</param>
        /// <param name="convertedInto">返回的曲线数据应具有的点数。转换方法可参见 AMS.Drives.CurveValue.ConvertTo</param>
        /// <returns>无论该日有无数据，均返回 TemperatureCurveValue 的实例。</returns>
        public TemperatureCurveValue GetData(DateTime date, CurvePointOptions convertedInto)
        {
            if (this.m_Reader == null || this.m_Reader.Contains(this.Device, date, date) == false)
            {
                new TemperatureCurveDataReader(this.Device, date, date, convertedInto);
            }

            return this.m_Reader.GetData(this.Device, date);
        }

        /// <summary>
        /// 查询一段日期的温度曲线数据，并确保该曲线具有指定的点数。
        /// </summary>
        /// <param name="startDate">数据起始日期。</param>
        /// <param name="endDate">数据结束日期。</param>
        /// <param name="convertedInto">返回的曲线数据应具有的点数。转换方法可参见 AMS.Drives.CurveValue.ConvertTo</param>
        /// <returns>无论有无数据，返回的日温度曲线数组长度均为起始日期与结束日期所相差的天数，并且数组内的数据按照起始日期至结束日期的顺序排列。</returns>
        public TemperatureCurveValue[] GetData(DateTime startDate, DateTime endDate, CurvePointOptions convertedInto)
        {
            TimeSpan tsDay = endDate - startDate;
            if (tsDay.Days < 0)
            {
                throw new Exception("结束日期不得小于起始日期。");
            }
            else
            {
                if (this.m_Reader == null || this.m_Reader.Contains(this.Device, startDate, endDate) == false)
                {
                    new TemperatureCurveDataReader(this.Device, startDate, endDate, convertedInto);
                }

                TemperatureCurveValue[] values = new TemperatureCurveValue[tsDay.Days + 1];

                for (int intIndex = 0; intIndex <= tsDay.Days; intIndex++)
                {
                    values[intIndex] = this.m_Reader.GetData(this.Device, startDate.AddDays(intIndex));
                }

                return values;
            }
        }

        /// <summary>
        /// 查询指定日期的温度曲线数据中的一段数据。
        /// </summary>
        /// <param name="date">数据日期。</param>
        /// <param name="startTimeNum">曲线数据段的起始时间。</param>
        /// <param name="endTimeNum">曲线数据段的结束时间。</param>
        /// <returns>无论该日有无数据，均返回 TemperatureCurvePartValue 的实例。</returns>
        public TemperatureCurvePartValue GetPartData(DateTime date, int startTimeNum, int endTimeNum)
        {
            if (this.m_PartReader == null || this.m_PartReader.Contains(this.Device, date, date, startTimeNum, endTimeNum) == false)
            {
                new TemperatureCurvePartDataReader(this.Device, date, date, startTimeNum, endTimeNum);
            }

            return this.m_PartReader.GetData(this.Device, date);
        }

        /// <summary>
        /// 查询一段日期的温度曲线数据中的一段数据。
        /// </summary>
        /// <param name="startDate">数据起始日期。</param>
        /// <param name="endDate">数据结束日期。</param>
        /// <param name="startTimeNum">曲线数据段的起始时间。</param>
        /// <param name="endTimeNum">曲线数据段的结束时间。</param>
        /// <returns>无论有无数据，返回的日温度曲线数组长度均为起始日期与结束日期所相差的天数，并且数组内的数据按照起始日期至结束日期的顺序排列。</returns>
        public TemperatureCurvePartValue[] GetPartData(DateTime startDate, DateTime endDate, int startTimeNum, int endTimeNum)
        {
            TimeSpan tsDay = endDate - startDate;
            if (tsDay.Days < 0)
            {
                throw new Exception("结束日期不得小于起始日期。");
            }
            else
            {
                if (this.m_PartReader == null || this.m_PartReader.Contains(this.Device, startDate, endDate, startTimeNum, endTimeNum) == false)
                {
                    new TemperatureCurvePartDataReader(this.Device, startDate, endDate, startTimeNum, endTimeNum);
                }

                TemperatureCurvePartValue[] values = new TemperatureCurvePartValue[tsDay.Days + 1];

                for (int intIndex = 0; intIndex <= tsDay.Days; intIndex++)
                {
                    values[intIndex] = this.m_PartReader.GetData(this.Device, startDate.AddDays(intIndex));
                }

                return values;
            }
        }

        /// <summary>
        /// 查询指定日期的温度曲线数据中的一段数据，并确保该曲线具有指定的点数。
        /// </summary>
        /// <param name="date">数据日期。</param>
        /// <param name="startTimeNum">曲线数据段的起始时间。</param>
        /// <param name="endTimeNum">曲线数据段的结束时间。</param>
        /// <param name="convertedInto">返回的曲线数据应具有的点数。转换方法可参见 AMS.Drives.CurveValue.ConvertTo</param>
        /// <returns>无论该日有无数据，均返回 TemperatureCurvePartValue 的实例。</returns>
        public TemperatureCurvePartValue GetPartData(DateTime date, int startTimeNum, int endTimeNum, CurvePointOptions convertedInto)
        {
            if (this.m_PartReader == null || this.m_PartReader.Contains(this.Device, date, date, startTimeNum, endTimeNum) == false)
            {
                new TemperatureCurvePartDataReader(this.Device, date, date, startTimeNum, endTimeNum, convertedInto);
            }

            return this.m_PartReader.GetData(this.Device, date);
        }

        /// <summary>
        /// 查询一段日期的温度曲线数据中的一段数据，并确保该曲线具有指定的点数。
        /// </summary>
        /// <param name="startDate">数据起始日期。</param>
        /// <param name="endDate">数据结束日期。</param>
        /// <param name="startTimeNum">曲线数据段的起始时间。</param>
        /// <param name="endTimeNum">曲线数据段的结束时间。</param>
        /// <param name="convertedInto">返回的曲线数据应具有的点数。转换方法可参见 AMS.Drives.CurvePartValue.ConvertTo</param>
        /// <returns>无论有无数据，返回的日温度曲线数组长度均为起始日期与结束日期所相差的天数，并且数组内的数据按照起始日期至结束日期的顺序排列。</returns>
        public TemperatureCurvePartValue[] GetPartData(DateTime startDate, DateTime endDate, int startTimeNum, int endTimeNum, CurvePointOptions convertedInto)
        {
            TimeSpan tsDay = endDate - startDate;
            if (tsDay.Days < 0)
            {
                throw new Exception("结束日期不得小于起始日期。");
            }
            else
            {
                if (this.m_PartReader == null || this.m_PartReader.Contains(this.Device, startDate, endDate, startTimeNum, endTimeNum) == false)
                {
                    new TemperatureCurvePartDataReader(this.Device, startDate, endDate, startTimeNum, endTimeNum, convertedInto);
                }

                TemperatureCurvePartValue[] values = new TemperatureCurvePartValue[tsDay.Days + 1];

                for (int intIndex = 0; intIndex <= tsDay.Days; intIndex++)
                {
                    values[intIndex] = this.m_PartReader.GetData(this.Device, startDate.AddDays(intIndex));
                }

                return values;
            }
        }

        /// <summary>
        /// 保存设备日温度曲线数据，如果数据点数大于96点，则另外再计算保存一份96点数据。（每月产生一张名为 TemperatureF 或 TemperatureS 的数据表）
        /// </summary>
        /// <param name="date">数据日期。只使用其中的年、月、日。</param>
        /// <param name="values">日温度曲线数据值，该数组长度必须是 CurvePointOptions 中指定的长度之一。</param>
        public void Save(DateTime date, decimal?[] values)
        {
            Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
            try
            {
                Save(dbConn, this.Device.DeviceId, date, values);
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

        /// <summary>
        /// 保存设备日温度曲线数据，如果数据点数大于96点，则另外再计算保存一份96点数据。（每月产生一张名为 TemperatureF 或 TemperatureS 的数据表）
        /// </summary>
        /// <param name="dbConnection">活动的数据库连接。</param>
        /// <param name="deviceId">设备编号。</param>
        /// <param name="date">数据日期。只使用其中的年、月、日。</param>
        /// <param name="values">日温度曲线数据值，该数组长度必须是 CurvePointOptions 中指定的长度之一。</param>
        public static void Save(Database.DbConnection dbConnection, int deviceId, DateTime date, decimal?[] values)
        {
            DeviceCurveDataReader.Save(typeof(TemperatureF), TemperatureF.TableName, typeof(TemperatureS), TemperatureS.TableName, CurveMergeOptions.Average, CurveSplitOptions.Copy, dbConnection, deviceId, date, values, new KeyValuePair<string, string>[] { });
        }

        /// <summary>
        /// 保存设备日温度曲线数据段（非完整的曲线数据，曲线中的一个点或连续的几个点），如果数据点数大于96点，则在每小时的00分、15分、30分、45分时自动进行96点数据的计算。（每月产生一张名为 TemperatureF 或 TemperatureS 的数据表）
        /// </summary>
        /// <param name="date">数据段中数据的起始日期及时间。使用其中的年、月、日、时、分、秒。。</param>
        /// <param name="curvePoint">该数据的点数类型。</param>
        /// <param name="values">日温度曲线数据段值，该数组仅存放曲线中的一部分数据。</param>
        public void Save(DateTime date, CurvePointOptions curvePoint, decimal?[] values)
        {
            Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
            try
            {
                Save(dbConn, this.Device.DeviceId, date, curvePoint, values);
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

        /// <summary>
        /// 保存设备日温度曲线数据段（非完整的曲线数据，曲线中的一个点或连续的几个点），如果数据点数大于96点，则每次保存时一次传入15分钟周期的全部数据，将获得最佳性能。（每月产生一张名为 TemperatureF 或 TemperatureS 的数据表）
        /// </summary>
        /// <param name="date">数据段中数据的起始日期及时间。使用其中的年、月、日、时、分、秒。。</param>
        /// <param name="curvePoint">该数据的点数类型。</param>
        /// <param name="values">日温度曲线数据段值，该数组仅存放曲线中的一部分数据。</param>
        /// <param name="save96Point">当 curvePoint 大于 96 点时是否另外计算一份96点的数据进行保存。当 curvePoint 小于等于 96 点时该参数无任何作用。</param>
        public void Save(DateTime date, CurvePointOptions curvePoint, decimal?[] values, CurvePartDataSave96PointOptions save96Point)
        {
            Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
            try
            {
                Save(dbConn, this.Device.DeviceId, date, curvePoint, values, save96Point);
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

        /// <summary>
        /// 保存设备日温度曲线数据段（非完整的曲线数据，曲线中的一个点或连续的几个点），如果数据点数大于96点，则在每小时的00分、15分、30分、45分时自动进行96点数据的计算。（每月产生一张名为 TemperatureF 或 TemperatureS 的数据表）
        /// </summary>
        /// <param name="dbConnection">活动的数据库连接。</param>
        /// <param name="deviceId">设备编号。</param>
        /// <param name="date">数据段中数据的起始日期及时间。使用其中的年、月、日、时、分、秒。。</param>
        /// <param name="curvePoint">该数据的点数类型。</param>
        /// <param name="values">日温度曲线数据段值，该数组仅存放曲线中的一部分数据。</param>
        public static void Save(Database.DbConnection dbConnection, int deviceId, DateTime date, CurvePointOptions curvePoint, decimal?[] values)
        {
            Save(dbConnection, deviceId, date, curvePoint, values, CurvePartDataSave96PointOptions.Auto);
        }

        /// <summary>
        /// 保存设备日温度曲线数据段（非完整的曲线数据，曲线中的一个点或连续的几个点），如果数据点数大于96点，则每次保存时一次传入15分钟周期的全部数据，将获得最佳性能。（每月产生一张名为 TemperatureF 或 TemperatureS 的数据表）
        /// </summary>
        /// <param name="dbConnection">活动的数据库连接。</param>
        /// <param name="deviceId">设备编号。</param>
        /// <param name="date">数据段中数据的起始日期及时间。使用其中的年、月、日、时、分、秒。</param>
        /// <param name="curvePoint">该数据的点数类型。</param>
        /// <param name="values">日温度曲线数据段值，该数组仅存放曲线中的一部分数据。</param>
        /// <param name="save96Point">当 curvePoint 大于 96 点时是否另外计算一份96点的数据进行保存。当 curvePoint 小于等于 96 点时该参数无任何作用。</param>
        public static void Save(Database.DbConnection dbConnection, int deviceId, DateTime date, CurvePointOptions curvePoint, decimal?[] values, CurvePartDataSave96PointOptions save96Point)
        {
            DeviceCurvePartDataReader.Save(typeof(TemperatureF), TemperatureF.TableName, typeof(TemperatureS), TemperatureS.TableName, CurveMergeOptions.Average, CurveSplitOptions.Copy, dbConnection, deviceId, date, curvePoint, values, save96Point, new KeyValuePair<string, string>[] { });
        }
    }
}
