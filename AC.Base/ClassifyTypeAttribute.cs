using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 分类类型特性。每个继承 Classify 的非抽象类必须添加该特性，以便描述该分类类型的一些信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassifyTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 描述继承 Classify 的分类的特征。
        /// </summary>
        /// <param name="name">分类类型名。</param>
        /// <param name="needCache">该类型分类是否需要进行缓存。通常对部门、行业等数量不多但是层级较多的分类进行缓存，对用户等数量较多层级简单的分类不进行缓存。</param>
        public ClassifyTypeAttribute(string name, bool needCache)
        {
            this.Name = name;
            this.NeedCache = needCache;
        }

        /// <summary>
        /// 分类类型名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 该类型分类是否需要进行缓存。通常对部门、行业等数量不多但是层级较多的分类进行缓存，对用户等数量较多层级简单的分类不进行缓存。
        /// </summary>
        public bool NeedCache { get; private set; }
    }
}
