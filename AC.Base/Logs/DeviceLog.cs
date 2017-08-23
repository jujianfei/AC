using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 表示为与设备相关的日志。继承该基类的实体类必须提供一个具有 ApplicationClass 参数的构造函数，且必须添加 LogTypeAttribute 特性。
    /// </summary>
    public abstract class DeviceLog : Log
    {
        /// <summary>
        /// 表示为与设备相关的日志。继承该基类的实体类必须提供一个具有 ApplicationClass 参数的构造函数，且必须添加 LogTypeAttribute 特性。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public DeviceLog(ApplicationClass application)
            : base(application)
        {
        }

        private int m_DeviceId;
        private Device m_Device;
        /// <summary>
        /// 与该日志有关的设备。
        /// </summary>
        public Device Device
        {
            get
            {
                if (this.m_Device == null && this.m_DeviceId > 0)
                {
                    List<int> lstId = new List<int>();
                    if (base.Source == null)
                    {
                        lstId.Add(this.m_DeviceId);
                    }
                    else
                    {
                        foreach (Log log in this.Source)
                        {
                            if (log is DeviceLog)
                            {
                                DeviceLog deviceLog = log as DeviceLog;
                                if (deviceLog.m_Device == null && deviceLog.m_DeviceId > 0)
                                {
                                    lstId.Add(deviceLog.m_DeviceId);
                                }
                            }
                        }
                    }
                    if (lstId.Count > 0)
                    {
                        DeviceSearchs.DeviceSearch search = new DeviceSearchs.DeviceSearch(base.Application);
                        search.Filters.Add(new DeviceSearchs.IdFilter(lstId));
                        foreach (Device device in search.Search())
                        {
                            if (device.DeviceId == this.m_DeviceId)
                            {
                                this.Device = device;
                            }
                            else if (base.Source != null)
                            {
                                foreach (Log log in this.Source)
                                {
                                    if (log is DeviceLog)
                                    {
                                        DeviceLog deviceLog = log as DeviceLog;
                                        if (deviceLog.m_DeviceId == device.DeviceId)
                                        {
                                            deviceLog.Device = device;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return this.m_Device;
            }
            set
            {
                this.m_Device = value;
            }
        }

        internal override void SetDataReader(System.Data.IDataReader dr)
        {
            base.SetDataReader(dr);
            this.m_DeviceId = Function.ToInt(dr[Tables.Log.ObjId]);
        }

        /// <summary>
        /// 保存当前设备日志，调用该方法前必须设置 Device 属性。
        /// </summary>
        public override void Save()
        {
            if (this.Device == null)
            {
                throw new Exception("调用该方法前必须设置 Device 属性。");
            }

            base.Save(this.Device.DeviceId);
        }

        /// <summary>
        /// 保存当前设备日志。
        /// </summary>
        /// <param name="device">该日志所属的设备。</param>
        public void Save(Device device)
        {
            this.Device = device;
            base.Save(this.Device.DeviceId);
        }

        /// <summary>
        /// 保存当前设备日志。
        /// </summary>
        /// <param name="deviceId">设备编号。</param>
        public new void Save(int deviceId)
        {
            this.m_DeviceId = deviceId;
            base.Save(deviceId);
        }
    }
}
