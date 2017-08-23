using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AC.Base.Database
{
    /// <summary>
    /// 可以在数据库中进行数据表创建的数据表。
    /// </summary>
    public class Table : System.IComparable
    {
        /// <summary>
        /// 可以在数据库中进行数据表创建的数据表。
        /// </summary>
        /// <param name="type">添加 TableAttribute 特性的数据表描述类</param>
        public Table(Type type)
        {
            object[] objAttrs = type.GetCustomAttributes(typeof(AC.Base.Database.TableAttribute), false);
            if (objAttrs.Length > 0)
            {
                this.Type = type;

                TableAttribute attr = objAttrs[0] as TableAttribute;
                this.Code = type.Name;
                this.Name = attr.Name;
                this.CreateRule = attr.CreateRule;
                this.Remark = attr.Remark;

                this.FindFields(this.Type);
            }
            else
            {
                throw new Exception(type.FullName + " 未添加 TableAttribute 特性。");
            }
        }

        private void FindFields(Type type)
        {
            foreach (FieldInfo fi in type.GetFields())
            {
                object[] objFieldAttrs = fi.GetCustomAttributes(typeof(AC.Base.Database.ColumnAttribute), false);
                if (objFieldAttrs.Length > 0)
                {
                    ColumnAttribute attrColumn = objFieldAttrs[0] as ColumnAttribute;

                    Column column = new Column();
                    column.Code = fi.Name;
                    column.Name = attrColumn.Name;
                    column.Remark = attrColumn.Remark;
                    column.DataType = attrColumn.DataType;
                    column.DataLength = attrColumn.DataLength;
                    column.DataPrecision = attrColumn.DataPrecision;
                    column.IsPrimaryKey = attrColumn.IsPrimaryKey;
                    column.IsNotNull = attrColumn.IsNotNull;

                    this.Columns.Add(column);

                    if (attrColumn.IndexCodes.Length > 0)
                    {
                        foreach (string strIndexName in attrColumn.IndexCodes)
                        {
                            if (this.Indexs.ContainsKey(strIndexName))
                            {
                                this.Indexs[strIndexName].Columns.Add(column);
                            }
                            else
                            {
                                Index index = new Index();
                                index.Code = strIndexName;
                                index.Columns.Add(column);
                                this.Indexs.Add(index);
                            }
                        }
                    }
                }
            }

            if (type.BaseType != null && type.BaseType.Equals(typeof(object)) == false)
            {
                this.FindFields(type.BaseType);
            }
        }

        /// <summary>
        /// 该数据表的类型。
        /// </summary>
        public Type Type { get; private set; }

        private string m_Code = "";
        /// <summary>
        /// 表示该模型对象的代码
        /// </summary>
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        private string m_Name = "";
        /// <summary>
        /// 表示该模型对象的名称
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// 数据表创建规则。
        /// </summary>
        public TableCreateRuleOptions CreateRule { get; set; }

        private string m_Remark = "";
        /// <summary>
        /// 表示该模型对象的备注
        /// </summary>
        public string Remark
        {
            get { return m_Remark; }
            set { m_Remark = value; }
        }

        private ColumnCollection m_Columns;
        /// <summary>
        /// 数据表的字段集合。
        /// </summary>
        public ColumnCollection Columns
        {
            get
            {
                if (m_Columns == null)
                {
                    m_Columns = new ColumnCollection();
                }
                return m_Columns;
            }
        }

        private IndexCollection m_Indexs;
        /// <summary>
        /// 数据表的索引集合。
        /// </summary>
        public IndexCollection Indexs
        {
            get
            {
                if (this.m_Indexs == null)
                {
                    this.m_Indexs = new IndexCollection();
                }
                return this.m_Indexs;
            }
        }



        /// <summary>
        /// 输出当前对象的内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Name.Length > 0)
            {
                return this.Code + " (" + this.Name + ")";
            }
            else
            {
                return this.Code;
            }
        }

        #region IComparable 成员

        /// <summary>
        /// 将此实例与指定的对象进行比较，并指示此实例在排序顺序中是位于指定的对象之前、之后还是与其出现在同一位置。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is Table)
            {
                Table table = obj as Table;
                return this.Code.CompareTo(table.Code);
            }
            return 1;
        }

        #endregion
    }
}
