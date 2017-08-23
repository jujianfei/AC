using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 执行搜索设备方法时搜索到一个设备产生的事件所用到的委托。
    /// </summary>
    /// <param name="device">搜索到的设备。</param>
    public delegate void DeviceSearchFoundEventHandler(Device device);

    /// <summary>
    /// 提供按照设定条件搜索指定设备的功能。
    /// </summary>
    public class DeviceSearch : Searchs.SearchBase<DeviceCollection, Device, IDeviceFilter, IDeviceOrder>
    {
        private DeviceFilterCollection m_Filters;

        /// <summary>
        /// 提供按照设定条件搜索指定设备的功能。
        /// </summary>
        /// <param name="application"></param>
        public DeviceSearch(ApplicationClass application)
            : base(application)
        {
            this.m_Filters = new DeviceFilterCollection();
            this.m_Filters.SetApplication(application);
            this.OrderInfos = new Searchs.SearchOrderInfoCollection<IDeviceOrder>();
        }

        /// <summary>
        /// 获取筛选数据的筛选器。
        /// </summary>
        protected override Searchs.FilterCollection<IDeviceFilter> filters
        {
            get { return this.m_Filters; }
        }

        /// <summary>
        /// 获取筛选设备的筛选器。
        /// </summary>
        public DeviceFilterCollection Filters
        {
            get { return this.m_Filters; }
        }

        /// <summary>
        /// 获取搜索器所执行SQL语句中的数据表。
        /// </summary>
        /// <returns></returns>
        public override Searchs.SearchTable[] GetTables()
        {
            return new Searchs.SearchTable[] { new Searchs.SearchTable(Tables.Device.TableName) };
        }

        /// <summary>
        /// 获取搜索器所执行SQL语句中所选取的字段。
        /// </summary>
        /// <returns></returns>
        public override Searchs.SearchSelectColumn[] GetSelectColumn()
        {
            return new Searchs.SearchSelectColumn[] { new Searchs.SearchSelectColumn(Tables.Device.TableName) };
        }

        /// <summary>
        /// 获取搜索器所执行SQL语句中默认的排序字段及排序顺序。
        /// </summary>
        /// <returns></returns>
        public override Searchs.SearchOrderInfoCollection<IDeviceOrder> GetDefaultOrders()
        {
            Searchs.SearchOrderInfoCollection<IDeviceOrder> orderInfo = new Searchs.SearchOrderInfoCollection<IDeviceOrder>();
            orderInfo.Add(false, new OrdinalNumberOrder());
            orderInfo.Add(false, new IdOrder());
            return orderInfo;
        }

        private DateRange m_DateRange;
        /// <summary>
        /// 获取查询设备数据时所使用的日期范围。
        /// </summary>
        public DateRange DateRange
        {
            get
            {
                if (this.m_DateRange == null)
                {
                    this.m_DateRange = new DateRange();
                }
                return this.m_DateRange;
            }
        }

        /// <summary>
        /// 按设定的条件执行搜索，搜索的数据不分页，返回所有符合条件的数据。
        /// </summary>
        /// <returns></returns>
        public DeviceCollection Search()
        {
            base.PageSize = 0;
            return this.Search(1, false, true);
        }

        /// <summary>
        /// 按设定的条件执行搜索，并返回指定页数的数据。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。</param>
        /// <returns></returns>
        public override DeviceCollection Search(int pageNum)
        {
            return this.Search(pageNum, false, true);
        }

        /// <summary>
        /// 按设定的条件执行搜索。
        /// </summary>
        /// <param name="pageNum">搜索指定页的设备。如果要搜索第2页及其之后页的数据，则应先设置 PageSize 属性。</param>
        /// <param name="isReverse">是否对排序信息 OrderInfos 中设定的排序条件执行倒序，如果该参数为 true 将导致整个搜索结果的先后顺序发生逆转，通常情况下应使用 false。</param>
        /// <param name="keepSource">是否保留所有设备至该搜索结果集合源的引用。当需要对设备进行进一步的数据查询操作，保留源引用可以提升查询性能。</param>
        /// <returns>符合条件的设备集合。</returns>
        public DeviceCollection Search(int pageNum, bool isReverse, bool keepSource)
        {
            base.PageNum = pageNum;

            DeviceCollection devices = new DeviceCollection(keepSource);

            try
            {
                System.Data.IDataReader dr = base.GetDataReader(isReverse);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int intDeviceId = Function.ToInt(dr[Tables.Device.DeviceId]);

                        Device device = base.Application.GetDeviceInstance(intDeviceId);
                        if (device != null)
                        {
                            device.SetDataReader(dr);
                            if (this.Found != null)
                            {
                                if (keepSource)
                                {
                                    if (isReverse)
                                    {
                                        devices.Insert(0, device);
                                    }
                                    else
                                    {
                                        devices.Add(device);
                                    }
                                }
                                this.Found(device);
                            }
                            else
                            {
                                if (isReverse)
                                {
                                    devices.Insert(0, device);
                                }
                                else
                                {
                                    devices.Add(device);
                                }
                            }

                            device.DateRange = this.m_DateRange;
                        }
                        else
                        {
                            DeviceType deviceType = base.Application.DeviceTypeSort.GetDeviceType(Function.ToString(dr[Tables.Device.DeviceType]));
                            if (deviceType != null)
                            {
                                device = deviceType.CreateDevice();
                                device.SetApplication(base.Application);
                                device.SetDataReader(dr);

                                base.Application.SetDeviceInstance(device);

                                if (this.Found != null)
                                {
                                    if (keepSource)
                                    {
                                        if (isReverse)
                                        {
                                            devices.Insert(0, device);
                                        }
                                        else
                                        {
                                            devices.Add(device);
                                        }
                                    }
                                    this.Found(device);
                                }
                                else
                                {
                                    if (isReverse)
                                    {
                                        devices.Insert(0, device);
                                    }
                                    else
                                    {
                                        devices.Add(device);
                                    }
                                }

                                device.DateRange = this.m_DateRange;
                            }
                            else
                            {
                                throw new Exception("未发现类型为 " + Function.ToString(dr[Tables.Device.DeviceType]) + " 的设备。");
                            }
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                base.DbConnection.Close();
            }

            return devices;
        }

        /// <summary>
        /// 调用 Search 方法搜索到设备后产生的事件,如果绑定该事件则每搜索到一个设备会产生一次事件。
        /// </summary>
        public event DeviceSearchFoundEventHandler Found;
    }
}
