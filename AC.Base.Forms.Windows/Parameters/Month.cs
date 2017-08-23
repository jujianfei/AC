using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Parameters
{
    /// <summary>
    /// 年月的单月份选择界面。
    /// </summary>
    [Control(typeof(AC.Base.Forms.Parameters.IMonth))]
    public class Month : System.Windows.Forms.Control, AC.Base.Forms.IParameterControl
    {
        ComboBox cmbMonth;
        ComboBox cmbYear;

        /// <summary>
        /// 日期选择参数。提供年月日的单日期选择界面。
        /// </summary>
        public Month()
        {
            cmbMonth = new ComboBox();
            cmbMonth.Dock = DockStyle.Left;
            cmbMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMonth.Size = new System.Drawing.Size(40, 23);
            for (int intIndex = 1; intIndex <= 12; intIndex++)
            {
                cmbMonth.Items.Add(intIndex);
                if (intIndex == DateTime.Today.Month)
                {
                    cmbMonth.SelectedItem = intIndex;
                }
            }
            this.Controls.Add(cmbMonth);

            cmbYear = new ComboBox();
            cmbYear.Dock = DockStyle.Left;
            cmbYear.Size = new System.Drawing.Size(60, 23);
            for (int intIndex = DateTime.Today.Year - 10; intIndex <= DateTime.Today.Year; intIndex++)
            {
                cmbYear.Items.Add(intIndex);

                if (intIndex == DateTime.Today.Year)
                {
                    cmbYear.SelectedItem = intIndex;
                }
            }
            this.Controls.Add(cmbYear);

            this.Size = new System.Drawing.Size(100, 23);
        }

        /// <summary>
        /// 设置应用程序框架。该方法仅在参数控件被初始化后调用一次。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(Forms.FormApplicationClass application)
        {
        }

        /// <summary>
        /// 当前参数界面采用水平布局还是垂直布局。
        /// </summary>
        /// <param name="isHorizontal"></param>
        public void SetOrientation(bool isHorizontal)
        {
        }

        /// <summary>
        /// 通过调用插件实现的参数接口将用户设置的参数值传入。该方法在插件初始化时调用，以及操作员每次修改参数点击确定后调用。
        /// </summary>
        /// <param name="parameter">同时实现 IPlugin 及 IParameter 接口的插件。</param>
        public void SetParameterPlugin(Forms.IParameter parameter)
        {
            AC.Base.Forms.Parameters.IMonth plugin = parameter as AC.Base.Forms.Parameters.IMonth;
            plugin.SetParameterMonth(new DateTime(Function.ToInt(this.cmbYear.Text), this.cmbMonth.SelectedIndex + 1, 1));
        }

        /// <summary>
        /// 设置日期参数。格式为 YYYYMM00
        /// </summary>
        /// <param name="value">参数字符串。</param>
        public void SetParameterValue(string value)
        {
            int intDateNum = Function.ToInt(value);
            if (intDateNum > 0 && intDateNum <= 99991231)
            {
                this.cmbYear.Text = (intDateNum / 10000).ToString();
                this.cmbMonth.SelectedIndex = ((intDateNum / 100) % 100) - 1;
            }
        }

        /// <summary>
        /// 获取选择的日期。仅年、月有效。
        /// </summary>
        /// <returns></returns>
        public DateTime GetDate()
        {
            return new DateTime(Function.ToInt(this.cmbYear.Text), this.cmbMonth.SelectedIndex + 1, 1);
        }

        /// <summary>
        /// 获取选择的日期。YYYYMM00
        /// </summary>
        /// <returns></returns>
        public int GetDateNum()
        {
            return Function.ToInt(this.cmbYear.Text) * 10000 + (this.cmbMonth.SelectedIndex + 1) * 100;
        }

        /// <summary>
        /// 设置日期。
        /// </summary>
        /// <param name="date"></param>
        public void SetDate(DateTime date)
        {
            this.cmbYear.Text = date.Year.ToString();
            this.cmbMonth.SelectedIndex = date.Month - 1;
        }

        /// <summary>
        /// 设置日期。
        /// </summary>
        /// <param name="dateNum">日期格式 YYYYMM00</param>
        public void SetDate(int dateNum)
        {
            this.SetDate(Function.ToDateTime(dateNum));
        }
    }
}
