using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 可以在数据库中进行数据表创建的数据表中的索引。
    /// </summary>
    public class Index
    {
        /// <summary>
        /// 索引名。
        /// </summary>
        public string Code { get; set; }

        private ColumnCollection m_Columns;
        /// <summary>
        /// 该索引关联的字段。
        /// </summary>
        public ColumnCollection Columns
        {
            get
            {
                if (this.m_Columns == null)
                {
                    this.m_Columns = new ColumnCollection();
                }
                return this.m_Columns;
            }
        }        
    }
}
