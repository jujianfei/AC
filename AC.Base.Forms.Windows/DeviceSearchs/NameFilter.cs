using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows.DeviceSearchs
{
    /// <summary>
    /// 设备通讯地址筛选器。
    /// </summary>
    [Control(typeof(AC.Base.DeviceSearchs.NameFilter))]
    public class NameFilter : Control, IDeviceFilterControl
    {
        private AC.Base.DeviceSearchs.NameFilter m_Filter;
        private TextBox m_Value;

        /// <summary>
        /// 设备通讯地址筛选器。
        /// </summary>
        public NameFilter()
        {
            this.Size = new System.Drawing.Size(0, 23);

            this.m_Value = new TextBox();
            this.m_Value.Dock = DockStyle.Fill;
            this.Controls.Add(this.m_Value);

            Label lab = new Label();
            lab.Text = "设备名称:";
            lab.AutoSize = false;
            lab.Dock = DockStyle.Left;
            lab.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lab.Size = new System.Drawing.Size(80, 0);
            this.Controls.Add(lab);
        }

        #region IFilterControl<IDeviceFilter> 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(ApplicationClass application)
        {
        }

        /// <summary>
        /// 设置搜索筛选器。
        /// </summary>
        /// <param name="filter">传入需要设置的搜索筛选器对象</param>
        public void SetFilter(IDeviceFilter filter)
        {
            this.m_Filter = filter as AC.Base.DeviceSearchs.NameFilter;

            string strValue = "";
            if (this.m_Filter.Name != null)
            {
                foreach (string s in this.m_Filter.Name)
                {
                    strValue += " " + s;
                }

                if (strValue.Length > 0)
                {
                    strValue = strValue.Substring(1);
                }
            }
            this.m_Value.Text = strValue;
        }

        /// <summary>
        /// 获取搜索筛选器。操作员在界面上设置搜索条件后，由框架调用该方法获取设置后的搜索筛选器对象。
        /// </summary>
        /// <returns></returns>
        public IDeviceFilter GetFilter()
        {
            this.m_Filter.Name = this.m_Value.Text.Split(new char[] { ' ', ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            return this.m_Filter;
        }

        /// <summary>
        /// 重置筛选器。清空设置值，使筛选器界面恢复默认状态。
        /// </summary>
        public void ResetFilter()
        {
            this.m_Value.Text = "";
        }

        #endregion
    }
}
