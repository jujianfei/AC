using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 高级设备搜索条件控件。
    /// </summary>
    public class DeviceFilterAdvanced : Panel
    {
        /// <summary>
        /// 高级设备搜索条件控件。
        /// </summary>
        public DeviceFilterAdvanced()
        {
            this.AutoSize = true;
            
            Label lab = new Label();
            lab.Text = "高级设备搜索条件控件";
            this.Controls.Add(lab);
        }


        private DeviceFilterCollection m_ReadonlyFilters;
        /// <summary>
        /// 设置设备只读筛选器。只读筛选器通常用于强制性的添加默认搜索条件。
        /// </summary>
        /// <param name="filters"></param>
        public void SetReadonlyFilters(DeviceFilterCollection filters)
        {
            this.m_ReadonlyFilters = filters;
        }

        /// <summary>
        /// 获取或设置设备筛选器。
        /// </summary>
        public DeviceFilterCollection Filters
        {
            get
            {
                return new DeviceFilterCollection();
            }
            set
            {
            }
        }
    }
}
