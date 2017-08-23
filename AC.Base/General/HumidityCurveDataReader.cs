using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 日湿度曲线数据读取器。
    /// </summary>
    public class HumidityCurveDataReader : DeviceCurveDataReader
    {
        private SortedList<int, SortedList<int, HumidityCurveValue>> m_Data = new SortedList<int, SortedList<int, HumidityCurveValue>>();

        internal HumidityCurveDataReader(Device device, DateTime startDate, DateTime endDate)
            : base(device, startDate, endDate, 0, CurveMergeOptions.Addition, CurveSplitOptions.Division, Tables.HumidityF.TableName, Tables.HumidityS.TableName)
        {
        }

        internal HumidityCurveDataReader(Device device, DateTime startDate, DateTime endDate, CurvePointOptions convertedInto)
            : base(device, startDate, endDate, (int)convertedInto, CurveMergeOptions.Addition, CurveSplitOptions.Division, Tables.HumidityF.TableName, Tables.HumidityS.TableName)
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
            if (device is IHumidityCurveDataDeviceImplement)
            {
                return ((IHumidityCurveDataDeviceImplement)device).HumidityCurveDataPoint;
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
            IHumidityCurveDataDeviceImplement dataDevice = device as IHumidityCurveDataDeviceImplement;
            if (dataDevice != null)
            {
                dataDevice.HumidityCurveData.m_Reader = this;
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
            int intDateNum = Function.ToInt(dr[Tables.HumidityF.DateNum]);

            if (this.m_Data.ContainsKey(device.DeviceId) == false)
            {
                this.m_Data.Add(device.DeviceId, new SortedList<int, HumidityCurveValue>());
            }

            if (this.m_Data[device.DeviceId].ContainsKey(intDateNum) == false)
            {
                this.m_Data[device.DeviceId].Add(intDateNum, new HumidityCurveValue(device, Function.ToDateTime(intDateNum), values));
            }
            else
            {
                this.m_Data[device.DeviceId][intDateNum] = new HumidityCurveValue(device, Function.ToDateTime(intDateNum), values);
            }
        }

        /// <summary>
        /// 处理数据库中读取到的按时间储存的数据。
        /// </summary>
        /// <param name="device">该数据相关的设备。</param>
        /// <param name="dr"></param>
        protected override void DoDataReaderS(Device device, System.Data.IDataReader dr)
        {
            int intDateNum = Function.ToInt(dr[Tables.HumidityS.DateNum]);
            int intTimeNum = Function.ToInt(dr[Tables.HumidityS.TimeNum]);

            if (this.m_Data.ContainsKey(device.DeviceId) == false)
            {
                this.m_Data.Add(device.DeviceId, new SortedList<int, HumidityCurveValue>());
            }
            if (this.m_Data[device.DeviceId].ContainsKey(intDateNum) == false)
            {
                Drives.CurvePointOptions curvePoint = this.GetDeviceCurvePoint(device.DeviceId);
                this.m_Data[device.DeviceId].Add(intDateNum, new HumidityCurveValue(device, Function.ToDateTime(intDateNum), curvePoint));
            }

            this.m_Data[device.DeviceId][intDateNum].SetValue(intTimeNum, Function.ToDecimalNull(dr[Tables.HumidityS.DataValue]));
        }

        /// <summary>
        /// 开始将分秒曲线数据转换为指定的点数。
        /// </summary>
        protected override void DoConvertS()
        {
            if (base.CurvePoint > 0)
            {
                CurvePointOptions _CurvePoint = (CurvePointOptions)base.CurvePoint;

                foreach (KeyValuePair<int, SortedList<int, HumidityCurveValue>> kvpDevice in this.m_Data)
                {
                    foreach (KeyValuePair<int, HumidityCurveValue> kvpDateNum in kvpDevice.Value)
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
        /// 获取指定设备的湿度曲线数据。
        /// </summary>
        /// <param name="device">设备。</param>
        /// <param name="date">数据日期。</param>
        /// <returns></returns>
        public HumidityCurveValue GetData(Device device, DateTime date)
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
                return new HumidityCurveValue(device, date, this.GetDeviceCurvePoint(device));
            }
            else
            {
                return new HumidityCurveValue(device, date, (CurvePointOptions)base.CurvePoint);
            }
        }
    }
}
