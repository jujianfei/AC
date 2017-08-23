using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 查询参数所对应的界面控件。实现该接口的类必须添加 ControlAttribute 属性。
    /// </summary>
    public interface IParameterControl : IControl
    {
        /// <summary>
        /// 设置应用程序框架。该方法仅在参数控件被初始化后调用一次。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(FormApplicationClass application);

        /// <summary>
        /// 当前参数界面采用水平布局还是垂直布局。
        /// </summary>
        /// <param name="isHorizontal">true：水平布局；false：垂直布局。</param>
        void SetOrientation(bool isHorizontal);

        /// <summary>
        /// 通过调用插件实现的参数接口将用户设置的参数值传入。该方法在插件初始化时调用，以及操作员每次修改参数点击确定后调用。
        /// </summary>
        /// <param name="parameter">同时实现 IPlugin 及 IParameter 接口的插件。</param>
        void SetParameterPlugin(IParameter parameter);

        /// <summary>
        /// 用于功能项跳转时指定后一功能项某一参数的具体参数值。
        /// 每一个实现 IParameter 接口的参数接口通常会提供一个静态获取参数配置的方法，调用该方法可以将传入的参数转为一个字符串，并且实现该参数的界面控件能够识别该字符串并将设置值显示在界面上。
        /// </summary>
        /// <param name="value">参数字符串，该字符串内不得包含换行符、“\”、“/”、“:”、“*”、“?”、“"”、“&lt;”、“&gt;”、“|”等文件名不允许使用的字符。</param>
        void SetParameterValue(string value);

    }
}
