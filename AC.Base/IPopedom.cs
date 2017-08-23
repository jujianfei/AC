using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 权限接口，实现该接口的类认为是系统内的一个权限。
    /// 实现该接口的类应添加 [HuiZhong.Popedom("权限名称", "权限描述", 上级权限类型, 扩展控件的中介者类型, 是否允许使用上级节点设置)] 属性。
    /// </summary>
    public interface IPopedom
    {
        /// <summary>
        /// 设置扩展配置 XML 文档，控件应根据设置的 XML 文档在界面上显示相应内容。
        /// 在调用过 SetApplication 方法后随即调用此方法。
        /// </summary>
        /// <param name="config"></param>
        void SetPopedomConfig(System.Xml.XmlNode config);

        /// <summary>
        /// 获取扩展配置 XML 文档，根据界面上的内容生成用于保存的 XML 文档。
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        System.Xml.XmlNode GetPopedomConfig(System.Xml.XmlDocument doc);
    }
}
