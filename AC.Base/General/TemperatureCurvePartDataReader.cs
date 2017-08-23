using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 日温度曲线段数据读取器。
    /// </summary>
    public class TemperatureCurvePartDataReader : DeviceCurvePartDataReader
    {
        private SortedList<int, SortedList<int, TemperatureCurvePartValue>> m_Data = new SortedList<int, SortedList<int, TemperatureCurvePartValue>>();

        internal TemperatureCurvePartDataReader(Device device, DateTime startDate, DateTime endDate, int startTimeNum, int endTimeNum)
            : base(device, startDate, endDate, startTimeNum, endTimeNum, 0, CurveMergeOptions.Addition, CurveSplitOptions.Division, Tables.TemperatureF.TableName, null, Tables.TemperatureS.TableName)
        {
        }

        internal TemperatureCurvePartDataReader(Device device, DateTime startDate, DateTime endDate, int startTimeNum, int endTimeNum, CurvePointOptions convertedInto)
            : base(device, startDate, endDate, startTimeNum, endTimeNum, (int)convertedInto, CurveMergeOptions.Addition, CurveSplitOptions.Division, Tables.TemperatureF.TableName, null, Tables.TemperatureS.TableName)
        {
        }


        private Drives.CurvePointOptions GetDeviceCurvePoint(int deviceId)
        {
            Device device = base.GetDevice(deviceId);
            if (device != null)
            {
                return this.GetDeviceCurvePoint(device);
            }

            return Drives.CurvePointOptions.Point24;
        }

        /// <summary>
        /// 获取指定设备默认的曲线数据点数。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        protected override Drives.CurvePointOptions GetDeviceCurvePoint(Device device)
        {
            if (device is ITemperatureCurveDataDeviceImplement)
            {
                return ((ITemperatureCurveDataDeviceImplement)device).TemperatureCurveDataPoint;
            }
            else
            {
                return Drives.CurvePointOptions.Point24;
            }
        }

        /// <summary>
        /// 为指定的设备设置数据读取器。
        /// </summary>
        /// <param name="device"></param>
        protected override void SetReader(Device device)
        {
            ITemperatureCurveDataDeviceImplement dataDevice = device as ITemperatureCurveDataDeviceImplement;
            if (dataDevice != null)
            {
                dataDevice.TemperatureCurveData.m_PartReader = this;
            }
        }

        /// <summary>
        /// 处理数据库中读取到的96点曲线数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        /// <param name="curvePoint">曲线点数。</param>
        /// <param name="values">经计算转换过的数据值集合。</param>
        protected override void DoDataReaderF(Device device, System.Data.IDataReader dr, CurvePointOptions curvePoint, decimal?[] values)
        {
            int intDateNum = Function.ToInt(dr[Tables.TemperatureF.DateNum]);

            if (this.m_Data.ContainsKey(device.DeviceId) == false)
            {
                this.m_Data.Add(device.DeviceId, new SortedList<int, TemperatureCurvePartValue>());
            }

            if (this.m_Data[device.DeviceId].ContainsKey(intDateNum) == false)
            {
                this.m_Data[device.DeviceId].Add(intDateNum, new TemperatureCurvePartValue(device, Function.ToDateTime(intDateNum), curvePoint, base.StartTimeNum, values));
            }
            else
            {
                this.m_Data[device.DeviceId][intDateNum] = new TemperatureCurvePartValue(device, Function.ToDateTime(intDateNum), curvePoint, base.StartTimeNum, values);
            }
        }

        /// <summary>
        /// 处理数据库中读取到的按时间储存的数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        protected override void DoDataReaderS(Device device, System.Data.IDataReader dr)
        {
            int intDateNum = Function.ToInt(dr[Tables.TemperatureS.DateNum]);
            int intTimeNum = Function.ToInt(dr[Tables.TemperatureS.TimeNum]);

            if (this.m_Data.ContainsKey(device.DeviceId) == false)
            {
                this.m_Data.Add(device.DeviceId, new SortedList<int, TemperatureCurvePartValue>());
            }
            if (this.m_Data[device.DeviceId].ContainsKey(intDateNum) == false)
            {
                Drives.CurvePointOptions curvePoint = this.GetDeviceCurvePoint(device.DeviceId);
                this.m_Data[device.DeviceId].Add(intDateNum, new TemperatureCurvePartValue(device, Function.ToDateTime(intDateNum), curvePoint, base.StartTimeNum, base.EndTimeNum));
            }

            this.m_Data[device.DeviceId][intDateNum].SetValue(intTimeNum, Function.ToDecimalNull(dr[Tables.TemperatureS.DataValue]));
        }

        /// <summary>
        /// 开始将分秒曲线数据段转换为指定的点数。
        /// </summary>
        protected override void DoConvertS()
        {
            if (base.CurvePoint > 0)
            {
                CurvePointOptions _CurvePoint = (CurvePointOptions)base.CurvePoint;

                foreach (KeyValuePair<int, SortedList<int, TemperatureCurvePartValue>> kvpDevice in this.m_Data)
                {
                    foreach (KeyValuePair<int, TemperatureCurvePartValue> kvpDateNum in kvpDevice.Value)
                    {
                        if (kvpDateNum.Value.CurvePoint != _CurvePoint)
                        {
                            kvpDateNum.Value.Convert(_CurvePoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定设备的温度曲线数据段。
        /// </summary>
        /// <param name="device">设备。</param>
        /// <param name="date">数据日期。</param>
        /// <returns></returns>
        public TemperatureCurvePartValue GetData(Device device, DateTime date)
        {
            if (this.m_Data.ContainsKey(device.DeviceId))
            {
                int intDateNum = Function.ToIntDate(date);
                if (this.m_Data[device.DeviceId].ContainsKey(intDateNum))
                {
                    return this.m_Data[device.DeviceId][intDateNum];
                }
            }

            if (base.CurvePoint == 0)
            {
                return new TemperatureCurvePartValue(device, date, this.GetDeviceCurvePoint(device), base.StartTimeNum, base.EndTimeNum);
            }
            else
            {
                return new TemperatureCurvePartValue(device, date, (CurvePointOptions)base.CurvePoint, base.StartTimeNum, base.EndTimeNum);
            }
        }
    }
}
