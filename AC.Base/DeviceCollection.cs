using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.DeviceSearchs;

namespace AC.Base
{
    /// <summary>
    /// 设备集合。
    /// </summary>
    public class DeviceCollection : System.Collections.Generic.IList<Device>, IDeviceCollection
    {
        private System.Collections.Generic.List<Device> Items = new List<Device>();

        /// <summary>
        /// 设备集合。
        /// </summary>
        /// <param name="keepSource">是否保持各设备与该集合的引用关系。</param>
        public DeviceCollection(bool keepSource)
        {
            this.m_KeepSource = keepSource;
        }

        private bool m_KeepSource;
        /// <summary>
        /// 获取或设置是否保持各设备与该集合的引用关系。
        /// </summary>
        public bool KeepSource
        {
            get
            {
                return this.m_KeepSource;
            }
            set
            {
                if (value)
                {
                    //保持引用
                    foreach (Device device in this)
                    {
                        device.Source = this;
                    }
                }
                else
                {
                    //清空引用
                    foreach (Device device in this)
                    {
                        device.Source = null;
                    }
                }
                this.m_KeepSource = value;
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个设备";
        }

        #region IList<Device> 成员

        /// <summary>
        /// 搜索指定的设备，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Device item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// 将设备插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Device item)
        {
            this.Items.Insert(index, item);

            if (this.KeepSource)
            {
                item.Source = this;
            }
            else
            {
                item.Source = null;
            }
        }

        /// <summary>
        /// 移除集合的指定索引处的设备。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);

            this.Items[index].Source = null;
        }

        /// <summary>
        /// 获取或设置指定索引处的设备。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Device this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                if (this.Items[index] != null)
                {
                    this.Items[index].Source = null;
                }

                this.Items[index] = value;

                if (this.KeepSource)
                {
                    this.Items[index].Source = this;
                }
                else
                {
                    this.Items[index].Source = null;
                }
            }
        }

        #endregion

        #region ICollection<Device> 成员

        /// <summary>
        /// 将设备添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        public void Add(Device item)
        {
            this.Items.Add(item);

            if (this.KeepSource)
            {
                item.Source = this;
            }
            else
            {
                item.Source = null;
            }
        }

        /// <summary>
        /// 从集合中移除所有设备。
        /// </summary>
        public void Clear()
        {
            foreach (Device item in this)
            {
                item.Source = null;
            }

            this.Items.Clear();
        }

        /// <summary>
        /// 确定某设备是否在集合中。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Device item)
        {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// 将整个设备集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Device[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中实际包含的设备数。
        /// </summary>
        public int Count
        {
            get { return this.Items.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示设备集合是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从集合中移除特定设备的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Device item)
        {
            item.Source = null;
            return this.Items.Remove(item);
        }

        #endregion

        #region IEnumerable<Device> 成员

        /// <summary>
        /// 返回循环访问设备集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Device> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问设备集合的枚举数。
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IDeviceCollection 成员

        /// <summary>
        /// 获取该集合中指定 ID 的设备。
        /// </summary>
        /// <param name="deviceId">设备 ID。</param>
        /// <returns>返回集合中指定 ID 的设备，如果未查找到该设备则返回 null。</returns>
        public Device GetDevice(int deviceId)
        {
            foreach (Device device in this)
            {
                if (device.DeviceId == deviceId)
                {
                    return device;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取该集合中符合筛选条件的设备集合。
        /// </summary>
        /// <param name="filters">设备筛选条件。</param>
        /// <returns></returns>
        public DeviceCollection GetDevices(DeviceFilterCollection filters)
        {
            DeviceCollection devices = new DeviceCollection(false);

            foreach (Device device in this)
            {
                if (filters.DeviceFilterCheck(device))
                {
                    devices.Add(device);
                }
            }

            return devices;
        }

        /// <summary>
        /// 获取当前集合内所有的设备编号。
        /// </summary>
        /// <returns></returns>
        public int[] GetIdForArray()
        {
            int[] ids = new int[this.Count];

            for (int intIndex = 0; intIndex < this.Count; intIndex++)
            {
                ids[intIndex] = this[intIndex].DeviceId;
            }

            return ids;
        }

        /// <summary>
        /// 获取当前集合内所有的设备编号。
        /// </summary>
        /// <returns></returns>
        public ICollection<int> GetIdForCollection()
        {
            List<int> lsId = new List<int>(this.Count);

            foreach (Device device in this)
            {
                lsId.Add(device.DeviceId);
            }

            return lsId;
        }

        /// <summary>
        /// 获取当前设备集合中以逗号分隔的设备编号字符串。
        /// </summary>
        /// <returns></returns>
        public string GetIdForString()
        {
            return this.GetIdForString(",");
        }

        /// <summary>
        /// 获取当前设备集合中以指定字符分隔的设备编号字符串。
        /// </summary>
        /// <param name="separator">用于分隔设备编号的字符。</param>
        /// <returns></returns>
        public string GetIdForString(string separator)
        {
            string strIds = "";

            foreach (Device device in this)
            {
                strIds += separator + device.DeviceId;
            }

            if (strIds.Length > 0)
            {
                strIds = strIds.Substring(separator.Length);
            }
            return strIds;
        }

        #endregion
    }
}
