using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows.DeviceSearchs
{
    /// <summary>
    /// 公共分类筛选器。
    /// </summary>
    [Control(typeof(AC.Base.DeviceSearchs.CommonCustomClassifyFilter))]
    public class CommonCustomClassifyFilter : Control, IDeviceFilterControl
    {
        private ApplicationClass m_Application;
        private AC.Base.DeviceSearchs.CommonCustomClassifyFilter m_Filter;
        private ComboBox m_Value;
        private Label labName;

        /// <summary>
        /// 设备类型筛选器。
        /// </summary>
        public CommonCustomClassifyFilter()
        {
            this.Size = new System.Drawing.Size(0, 23);

            this.m_Value = new ComboBox();
            this.m_Value.Dock = DockStyle.Fill;
            this.m_Value.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(this.m_Value);

            labName = new Label();
            labName.AutoSize = false;
            labName.Dock = DockStyle.Left;
            labName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labName.Size = new System.Drawing.Size(80, 0);
            this.Controls.Add(labName);
        }

        #region IFilterControl<IDeviceFilter> 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;
            this.m_Value.Items.Clear();
            this.m_Value.Items.Add("");
        }

        private void FillClassifys(IClassifyCollection classifys, string indent)
        {
            foreach (CommonCustomClassify _Classify in classifys)
            {
                this.m_Value.Items.Add(new CommonCustomClassifyInfo(_Classify));

                this.FillClassifys(_Classify.Children, indent + "　");
            }
        }

        /// <summary>
        /// 设置搜索筛选器。
        /// </summary>
        /// <param name="filter">传入需要设置的搜索筛选器对象</param>
        public void SetFilter(IDeviceFilter filter)
        {
            this.m_Filter = filter as AC.Base.DeviceSearchs.CommonCustomClassifyFilter;
            this.labName.Text = this.m_Filter.FilterName + ":";

            AC.Base.ClassifySearchs.ClassifySearch _Search = new AC.Base.ClassifySearchs.ClassifySearch(this.m_Application);
            _Search.Filters.Add(new AC.Base.ClassifySearchs.ParentIdFilter(0));
            _Search.Filters.Add(new AC.Base.ClassifySearchs.ClassifyTypeFilter(typeof(CommonCustomClassify)));
            foreach (CommonCustomClassify _Classify in _Search.Search())
            {
                if (_Classify.Name.Equals(this.m_Filter.FilterName))
                {
                    this.FillClassifys(_Classify.Children, "");
                }
            }

            if (this.m_Filter.Classify == null)
            {
                this.m_Value.SelectedIndex = 0;
            }
            else
            {
                for (int intIndex = 1; intIndex < this.m_Value.Items.Count; intIndex++)
                {
                    if (this.m_Value.Items[intIndex] is CommonCustomClassifyInfo)
                    {
                        CommonCustomClassifyInfo _Classify = this.m_Value.Items[intIndex] as CommonCustomClassifyInfo;
                        if (_Classify.Classify.Equals(this.m_Filter.Classify))
                        {
                            this.m_Value.SelectedIndex = intIndex;
                            return;
                        }
                    }
                }

                this.m_Value.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 获取搜索筛选器。操作员在界面上设置搜索条件后，由框架调用该方法获取设置后的搜索筛选器对象。
        /// </summary>
        /// <returns></returns>
        public IDeviceFilter GetFilter()
        {
            if (this.m_Value.SelectedItem is CommonCustomClassifyInfo)
            {
                this.m_Filter.Classify = ((CommonCustomClassifyInfo)this.m_Value.SelectedItem).Classify;
            }
            else
            {
                this.m_Filter.Classify = null;
            }
            return this.m_Filter;
        }

        /// <summary>
        /// 重置筛选器。清空设置值，使筛选器界面恢复默认状态。
        /// </summary>
        public void ResetFilter()
        {
            this.m_Value.SelectedIndex = 0;
        }

        #endregion

        private class CommonCustomClassifyInfo
        {
            public CommonCustomClassifyInfo(CommonCustomClassify classify)
            {
                this.Classify = classify;
            }

            public CommonCustomClassify Classify { get; private set; }

            public override string ToString()
            {
                string str = "";
                for (int intIndex = 1; intIndex < this.Classify.GetLevel(); intIndex++)
                {
                    str += "　";
                }
                return str + this.Classify.Name;
            }
        }
    }
}
