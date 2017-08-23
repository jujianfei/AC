using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 针对查询页面系统内预置了一些常用的条件参数，如日期选择、时段选择、相序选择、费率选择等。实现 IPlugin 接口的插件另外再添加相应的参数接口，应用程序会在插件内容呈现区上方显示相应的参数选择界面，用户改变参数后通过调用参数接口指定的方法将选择的参数传递给插件。
    /// 可以添加 IUseAccount 接口获取当前使用的账号。
    /// </summary>
    public interface IParameter
    {

    }
}
