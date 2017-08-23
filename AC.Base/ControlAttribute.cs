using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 控件属性。实现 IControl 接口的类必须添加该属性，以便描述该控件为哪种业务对象服务。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ControlAttribute : System.Attribute
    {
        /// <summary>
        /// 控件属性。实现 IControl 接口的类必须添加该属性，以便描述该控件为哪种业务对象服务。
        /// </summary>
        /// <param name="forType">所服务的业务对象类型。</param>
        public ControlAttribute(Type forType)
        {
            this.ForType = forType;
        }

        /// <summary>
        /// 获取该控件所服务的业务对象类型。
        /// </summary>
        public Type ForType { get; private set; }
    }
}
