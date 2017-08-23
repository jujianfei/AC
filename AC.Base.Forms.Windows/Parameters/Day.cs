using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows.Parameters
{
    /// <summary>
    /// 年月日的单日期选择界面。
    /// </summary>
    [Control(typeof(AC.Base.Forms.Parameters.IDay))]
    public class Day : System.Windows.Forms.DateTimePicker, AC.Base.Forms.IParameterControl
    {
        /// <summary>
        /// 日期选择参数。提供年月日的单日期选择界面。
        /// </summary>
        public Day()
        {
            this.Size = new System.Drawing.Size(120, 23);
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
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
        /// 通过调用插件实现的参数接口将用户设置的参数值传入。
        /// </summary>
        /// <param name="parameter">同时实现 IPlugin 及 IParameter 接口的插件。</param>
        public void SetParameterPlugin(Forms.IParameter parameter)
        {
            AC.Base.Forms.Parameters.IDay plugin = parameter as AC.Base.Forms.Parameters.IDay;
            plugin.SetParameterDay(this.Value.Date);
        }

        /// <summary>
        /// 设置日期参数。格式为 YYYYMMDD
        /// </summary>
        /// <param name="value">参数字符串。</param>
        public void SetParameterValue(string value)
        {
            int intDateNum = Function.ToInt(value);
            if (intDateNum > 0 && intDateNum <= 99991231)
            {
                this.Value = new DateTime(intDateNum / 10000, (intDateNum / 100) % 100, intDateNum % 100);
            }
        }
    }
}
