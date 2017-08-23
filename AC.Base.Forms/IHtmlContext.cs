using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 使运行在应用程序宿主中的 HTML 插件、运行在 WEB 宿主中的 HTML 插件及 WEB 插件能够在与宿主无关的方式下动态获得所需的内容而提供的接口。
    /// 实现该接口的类在 GetContext 方法中解析 parameter 中请求内容的标记并进行数据处理，然后返回字符串的内容。调用方根据该类声明的返回内容格式对返回的内容进行解析并显示在界面上。
    /// 如果需要操作员账号信息可以添加 IUseAccount 接口。
    /// </summary>
    public interface IHtmlContext
    {
        /// <summary>
        /// 获取内容。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="parameter">参数。</param>
        /// <returns></returns>
        string GetContext(FormApplicationClass application, string parameter);
    }
}
