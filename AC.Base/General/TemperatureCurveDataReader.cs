using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 日温度曲线数据读取器。
    /// </summary>
    public class TemperatureCurveDataReader : DeviceCurveDataReader
    {
        private SortedList<int, SortedList<int, TemperatureCurveValue>> m_Data = new SortedList<int, SortedList<int, TemperatureCurveValue>>();

        internal TemperatureCurveDataReader(Device device, DateTime startDate, DateTime endDate)
            : base(device, startDate, endDate, 0, CurveMergeOptions.Addition, CurveSplitOptions.Division, Tables.TemperatureF.TableName, Tables.TemperatureS.TableName)
        {
        }

        internal TemperatureCurveDataReader(Device device, DateTime startDate, DateTime endDate, CurvePointOptions convertedInto)
            : base(device, startDate, endDate, (int)convertedInto, CurveMergeOptions.Addition, CurveSplitOptions.Division, Tables.TemperatureF.TableName, Tables.TemperatureS.TableName)
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
                dataDevice.TemperatureCurveData.m_Reader = this;
            }
        }

        /// <summary>
        /// 处理数据库中读取到的96点曲线数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        /// <param name="values">经计算转换过的数据值集合。</param>
        protected override void DoDataReaderF(Device device, System.Data.IDataReader dr, decimal?[] values)
        {
            int intDateNum = Function.ToInt(dr[Tables.TemperatureF.DateNum]);

            if (this.m_Data.ContainsKey(device.DeviceId) == false)
            {
                this.m_Data.Add(device.DeviceId, new SortedList<int, TemperatureCurveValue>());
            }

            if (this.m_Data[device.DeviceId].ContainsKey(intDateNum) == false)
            {
                this.m_Data[device.DeviceId].Add(intDateNum, new TemperatureCurveValue(device, Function.ToDateTime(intDateNum), values));
            }
            else
            {
                this.m_Data[device.DeviceId][intDateNum] = new TemperatureCurveValue(device, Function.ToDateTime(intDateNum), values);
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
                this.m_Data.Add(device.DeviceId, new SortedList<int, TemperatureCurveValue>());
            }
            if (this.m_Data[device.DeviceId].ContainsKey(intDateNum) == false)
            {
                Drives.CurvePointOptions curvePoint = this.GetDeviceCurvePoint(device.DeviceId);
                this.m_Data[device.DeviceId].Add(intDateNum, new TemperatureCurveValue(device, Function.ToDateTime(intDateNum), curvePoint));
            }

            this.m_Data[device.DeviceId][intDateNum].SetValue(intTimeNum, Function.ToDecimalNull(dr[Tables.TemperatureS.DataValue]));
        }

        /// <summary>
        /// 开始将分秒曲线数据转换为指定的点数。
        /// </summary>
        protected override void DoConvertS()
        {
            if (base.CurvePoint > 0)
            {
                CurvePointOptions _CurvePoint = (CurvePointOptions)base.CurvePoint;

                foreach (KeyValuePair<int, SortedList<int, TemperatureCurveValue>> kvpDevice in this.m_Data)
                {
                    foreach (KeyValuePair<int, TemperatureCurveValue> kvpDateNum in kvpDevice.Value)
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
        /// 获取指定设备的温度曲线数据。
        /// </summary>
        /// <param name="device">设备。</param>
        /// <param name="date">数据日期。</param>
        /// <returns></returns>
        public TemperatureCurveValue GetData(Device device, DateTime date)
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
                return new TemperatureCurveValue(device, date, this.GetDeviceCurvePoint(device));
            }
            else
            {
                return new TemperatureCurveValue(device, date, (CurvePointOptions)base.CurvePoint);
            }
        }
    }
}
