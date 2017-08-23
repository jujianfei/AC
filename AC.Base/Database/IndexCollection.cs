using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 索引集合。
    /// </summary>
    public class IndexCollection : ReadOnlyCollection<Index>
    {
        internal IndexCollection()
        {
        }

        /// <summary>
        /// 将索引添加到集合中。
        /// </summary>
        /// <param name="item"></param>
        public new void Add(Index item)
        {
            base.Add(item);
        }

        /// <summary>
        /// 获取或设置指定 code 处的元素。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Index this[string code]
        {
            get
            {
                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    if (this[intIndex].Code.Equals(code))
                    {
                        return this[intIndex];
                    }
                }

                throw new Exception("未发现代码为“" + code + "”的字段。");
            }
            set
            {
                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    if (this[intIndex].Code.Equals(code))
                    {
                        this[intIndex] = value;
                        return;
                    }
                }

                throw new Exception("未发现代码为“" + code + "”的字段。");
            }
        }

        /// <summary>
        /// 指定的字段代码是否存在于集合中
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ContainsKey(string code)
        {
            for (int intIndex = 0; intIndex < this.Count; intIndex++)
            {
                if (this[intIndex].Code.Equals(code))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 输出当前对象的内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Count.ToString() + "个索引";
        }
    }
}
