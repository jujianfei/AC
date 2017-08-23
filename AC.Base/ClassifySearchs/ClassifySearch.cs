using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 执行搜索分类方法时搜索到一个分类产生的事件所用到的委托。
    /// </summary>
    /// <param name="classify">搜索到的分类。</param>
    public delegate void ClassifySearchFoundEventHandler(Classify classify);

    //对于对象选择列表，应该调用 Search(x, true, true) 在查询各分类的下级分类数量后设置 KeepSource = false。
    /// <summary>
    /// 提供按照设定条件搜索指定分类的功能。
    /// </summary>
    public class ClassifySearch : Searchs.SearchBase<ClassifyCollection, Classify, IClassifyFilter, IClassifyOrder>
    {
        private ClassifyFilterCollection m_Filters;

        /// <summary>
        /// 提供按照设定条件搜索指定分类的功能。
        /// </summary>
        /// <param name="application"></param>
        public ClassifySearch(ApplicationClass application)
            : base(application)
        {
            this.m_Filters = new ClassifyFilterCollection();
            this.m_Filters.SetApplication(application);
            this.OrderInfos = new Searchs.SearchOrderInfoCollection<IClassifyOrder>();
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
            return new Searchs.SearchSelectColumn[] { new Searchs.SearchSelectColumn(Tables.Classify.TableName) };
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
        public ClassifyCollection Search()
        {
            base.PageSize = 0;
            return this.Search(1, false, true, false);
        }

        /// <summary>
        /// 按设定的条件执行搜索，并返回指定页数的数据。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。</param>
        /// <returns></returns>
        public override ClassifyCollection Search(int pageNum)
        {
            return this.Search(pageNum, false, true, false);
        }

        /// <summary>
        /// 按设定的条件执行搜索。
        /// </summary>
        /// <param name="pageNum">搜索指定页的分类。如果要搜索第2页及其之后页的数据，则应先设置 PageSize 属性。</param>
        /// <param name="isReverse">是否对排序信息 OrderInfos 中设定的排序条件执行倒序，如果该参数为 true 将导致整个搜索结果的先后顺序发生逆转，通常情况下应使用 false。</param>
        /// <param name="keepSource">是否保留所有分类至该搜索结果集合源的引用。当需要对分类进行进一步的数据查询操作，保留源引用可以提升查询性能。</param>
        /// <param name="reload">对于应用程序框架缓存的分类，是否重新载入分类属性。如果应用程序框架对分类进行了缓存用以保持引用分类的一致性，该参数为 true 时可以强制刷新分类档案，例如桌面应用程序通常需要对分类对象进行缓存；但是对于 WEB 应用程序通常不做分类对象缓存，所以将该参数设为 false。</param>
        /// <returns>符合条件的分类集合。</returns>
        public ClassifyCollection Search(int pageNum, bool isReverse, bool keepSource, bool reload)
        {
            base.PageNum = pageNum;

            ClassifyCollection classifys = new ClassifyCollection(keepSource);

            try
            {
                System.Data.IDataReader dr = base.GetDataReader(isReverse);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int intClassifyId = Function.ToInt(dr[Tables.Classify.ClassifyId]);

                        Classify classify = base.Application.GetClassifyInstance(intClassifyId);
                        if (classify != null)
                        {
                            if (reload)
                            {
                                classify.SetDataReader(dr);
                            }

                            if (this.Found != null)
                            {
                                if (keepSource)
                                {
                                    if (isReverse)
                                    {
                                        classifys.Insert(0, classify);
                                    }
                                    else
                                    {
                                        classifys.Add(classify);
                                    }
                                }
                                this.Found(classify);
                            }
                            else
                            {
                                if (isReverse)
                                {
                                    classifys.Insert(0, classify);
                                }
                                else
                                {
                                    classifys.Add(classify);
                                }
                            }
                        }
                        else
                        {
                            ClassifyType classifyType = base.Application.ClassifyTypes.GetClassifyType(Function.ToString(dr[Tables.Classify.ClassifyType]));
                            if (classifyType != null)
                            {
                                classify = classifyType.CreateClassify();
                                classify.SetApplication(base.Application);
                                classify.SetDataReader(dr);

                                if (classifyType.NeedCache)
                                {
                                    base.Application.SetClassifyInstance(classify);
                                }

                                if (this.Found != null)
                                {
                                    if (keepSource)
                                    {
                                        if (isReverse)
                                        {
                                            classifys.Insert(0, classify);
                                        }
                                        else
                                        {
                                            classifys.Add(classify);
                                        }
                                    }
                                    this.Found(classify);
                                }
                                else
                                {
                                    if (isReverse)
                                    {
                                        classifys.Insert(0, classify);
                                    }
                                    else
                                    {
                                        classifys.Add(classify);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("未发现类型为 " + Function.ToString(dr[Tables.Classify.ClassifyType]) + " 的分类。");
                            }
                        }
                    }
                    dr.Close();
                }
            }
            finally
            {
                base.DbConnection.Close();
            }

            return classifys;
        }

        /// <summary>
        /// 调用 Search 方法搜索到分类后产生的事件,如果绑定该事件则每搜索到一个分类会产生一次事件。
        /// </summary>
        public event ClassifySearchFoundEventHandler Found;
    }
}
