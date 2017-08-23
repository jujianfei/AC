using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 可以对分类进行遍历枚举、使用索引器获取分类、根据分类 ID 获取分类的接口。
    /// </summary>
    public interface IClassifyCollection : IReadOnlyCollection<Classify>
    {
        /// <summary>
        /// 获取该集合中指定 ID 的分类。
        /// </summary>
        /// <param name="classifyId">分类 ID。</param>
        /// <returns>返回集合中指定 ID 的分类，如果未查找到该分类则返回 null。</returns>
        Classify GetClassify(int classifyId);

        /// <summary>
        /// 获取该集合中符合筛选条件的分类集合。
        /// </summary>
        /// <param name="filters">分类筛选条件。</param>
        /// <returns></returns>
        ClassifyCollection GetClassifys(ClassifySearchs.ClassifyFilterCollection filters);

        /// <summary>
        /// 获取当前集合内所有的分类编号。
        /// </summary>
        /// <returns></returns>
        int[] GetIdForArray();

        /// <summary>
        /// 获取当前集合内所有的分类编号。
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.ICollection<int> GetIdForCollection();

        /// <summary>
        /// 获取当前集合内所有的分类编号。
        /// </summary>
        /// <returns>以逗号分隔的字符串形式的分类编号。</returns>
        string GetIdForString();

        /// <summary>
        /// 获取当前集合内所有的分类编号。
        /// </summary>
        /// <param name="separator">分隔各分类编号的字符。</param>
        /// <returns>以指定字符分隔的字符串形式的分类编号。</returns>
        string GetIdForString(string separator);
    }
}
