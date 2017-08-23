using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 提供按照设定条件搜索指定分类的上级分类ID的功能。
    /// </summary>
    public class ClassifyParentIdSearch : Searchs.SearchBase<IList<int>, int, IClassifyFilter, IClassifyOrder>
    {
        private ClassifyFilterCollection m_Filters;

        /// <summary>
        /// 提供按照设定条件搜索指定分类的上级分类ID的功能。
        /// </summary>
        /// <param name="application"></param>
        public ClassifyParentIdSearch(ApplicationClass application)
            : base(application)
        {
            this.m_Filters = new ClassifyFilterCollection();
            this.m_Filters.SetApplication(application);
        }

        /// <summary>
        /// 获取筛选数据的筛选器。
        /// </summary>
        protected override Searchs.FilterCollection<IClassifyFilter> filters
        {
            get { return this.m_Filters; }
        }

        /// <summary>
        /// 获取筛选分类的筛选器。
        /// </summary>
        public ClassifyFilterCollection Filters
        {
            get { return this.m_Filters; }
        }

        /// <summary>
        /// 获取搜索器所执行SQL语句中的数据表。
        /// </summary>
        /// <returns></returns>
        public override Searchs.SearchTable[] GetTables()
        {
            return new Searchs.SearchTable[] { new Searchs.SearchTable(Tables.Classify.TableName) };
        }

        /// <summary>
        /// 获取搜索器所执行SQL语句中所选取的字段。
        /// </summary>
        /// <returns></returns>
        public override Searchs.SearchSelectColumn[] GetSelectColumn()
        {
            return new Searchs.SearchSelectColumn[] { new Searchs.SearchSelectColumn(Tables.Classify.TableName, new string[] { Tables.Classify.ParentId }) };
        }

        /// <summary>
        /// 获取搜索器所执行SQL语句中默认的排序字段及排序顺序。
        /// </summary>
        /// <returns></returns>
        public override Searchs.SearchOrderInfoCollection<IClassifyOrder> GetDefaultOrders()
        {
            Searchs.SearchOrderInfoCollection<IClassifyOrder> orderInfo = new Searchs.SearchOrderInfoCollection<IClassifyOrder>();
            orderInfo.Add(false, new OrdinalNumberOrder());
            orderInfo.Add(false, new IdOrder());
            return orderInfo;
        }

        /// <summary>
        /// 按设定的条件执行搜索，搜索的数据不分页，返回所有符合条件的数据。
        /// </summary>
        /// <returns></returns>
        public IList<int> Search()
        {
            base.PageSize = 0;
            return this.Search(1);
        }

        /// <summary>
        /// 按设定的条件执行搜索，并返回指定页数的数据。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。</param>
        /// <returns></returns>
        public override IList<int> Search(int pageNum)
        {
            base.PageNum = pageNum;
            List<int> lstId = new List<int>();

            try
            {
                System.Data.IDataReader dr = base.GetDataReader(false);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int intParentId = Function.ToInt(dr[Tables.Classify.ParentId]);
                        if (intParentId > 0 && lstId.Contains(intParentId) == false)
                        {
                            lstId.Add(intParentId);
                        }
                    }
                    dr.Close();
                }
            }
            finally
            {
                base.DbConnection.Close();
            }

            return lstId;
        }
    }
}
