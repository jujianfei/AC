using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 用于界面插件跳转时传递并设置目标插件某一参数的默认值。
    /// </summary>
    public class ParameterValue
    {
        /// <summary>
        /// 用于界面插件跳转时传递并设置目标插件某一参数的默认值。
        /// </summary>
        public ParameterValue()
        {
        }

        /// <summary>
        /// 用于界面插件跳转时传递并设置目标插件某一参数的默认值。
        /// </summary>
        /// <param name="parameterType">实现 IParameter 接口的参数类的类型声明。</param>
        /// <param name="parameterValue">需要设置的新值。每一个实现 IParameter 接口的参数通常附带一个将指定参数转为字符串值的静态方法，可以通过该方法获得欲设置参数的字符串值。</param>
        public ParameterValue(Type parameterType, string parameterValue)
        {
            this.Type = parameterType;
            this.Value = parameterValue;
        }

        /// <summary>
        /// 实现 IParameter 接口的参数类的类型声明。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 需要设置的新值。每一个实现 IParameter 接口的参数通常附带一个将指定参数转为字符串值的静态方法，可以通过该方法获得欲设置参数的字符串值。
        /// </summary>
        public string Value { get; set; }
    }
}
