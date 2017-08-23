using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 机构分类。机构只允许有一个第一级节点，下级节点的层数及数量不限制。继承该基类的实体类必须提供一个无参数的构造函数，且必须添加 ClassifyTypeAttribute 特性。
    /// </summary>
    public abstract class Organization : Classify
    {
    }
}
