using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 分类插件检查接口。
    /// 实现IClassifyPlugin接口并添加ClassifyPluginTypeAttribute特性的插件可以指定实现该接口的ForCheckType属性，当在分类上显示菜单前会初始化ForCheckType指向的类并调用Check方法。
    /// </summary>
    public interface IClassifyCheck
    {
        /// <summary>
        /// 检查分类插件能否用于指定的分类。
        /// </summary>
        /// <param name="classify"></param>
        /// <returns></returns>
        bool Check(Classify classify);
    }
}
