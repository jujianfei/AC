using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 可以在数据库中进行数据表创建的数据表集合。
    /// </summary>
    public class TableCollection : System.Collections.Generic.List<Table>
    {
        /// <summary>
        /// 获取或设置指定 code 处的元素。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Table this[string code]
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

                throw new Exception("未发现代码为“" + code + "”的表。");
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

                throw new Exception("未发现代码为“" + code + "”的表。");
            }
        }

        /// <summary>
        /// 指定的数据表代码是否存在于集合中
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsKey(string value)
        {
            foreach (Table _Table in this)
            {
                if (_Table.Code.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 按照表名对表集合进行排序
        /// </summary>
        public void OrderByCode()
        {
            System.Collections.Generic.SortedList<string, Table> slTables = new SortedList<string, Table>();

            foreach (Table _Table in this)
            {
                try
                {
                    slTables.Add(_Table.Code, _Table);
                }
                catch { }
            }

            this.Clear();

            foreach (Table _Table in slTables.Values)
            {
                this.Add(_Table);
            }
        }

        /// <summary>
        /// 输出当前对象的内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Count.ToString() + "张表";
        }
    }
}
