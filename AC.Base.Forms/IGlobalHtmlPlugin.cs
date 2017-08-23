using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 全局 HTML 插件。应用程序框架会在程序工具栏上提供实现该接口插件的菜单或按钮，点击菜单或按钮后由框架初始化实现接口的类并调用接口上的各方法传入必需的参数，之后将插件界面加载入程序中心区域呈现出来。
    /// 实现该接口的插件无须继承特定类，但必须添加 GlobalPluginTypeAttribute 特性，如果需要账号信息可以添加 IUseAccount 接口。
    /// </summary>
    public interface IGlobalHtmlPlugin : IGlobalPlugin, IHtmlPlugin
    {
    }
}
