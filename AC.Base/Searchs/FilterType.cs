using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索筛选器类型描述。
    /// </summary>
    /// <typeparam name="F"></typeparam>
    public abstract class FilterType<F> where F : IFilter
    {
        /// <summary>
        /// 应用程序框架。
        /// </summary>
        protected ApplicationClass Application;

        /// <summary>
        /// 筛选器。
        /// </summary>
        protected F Filter;

        /// <summary>
        /// 搜索筛选器类型描述。
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        protected FilterType(ApplicationClass application, Type type)
        {
            this.Application = application;

            System.Reflection.ConstructorInfo ciAssembly = type.GetConstructor(new System.Type[] { });
            object objInstance = ciAssembly.Invoke(new object[] { });
            this.Filter = (F)objInstance;
            this.Filter.SetApplication(this.Application);
        }

        /// <summary>
        /// 筛选器应用场合选项。
        /// </summary>
        public FilterAppliesOptions Applies
        {
            get
            {
                return this.Filter.FilterAppliesAttribute;
            }
        }

        /// <summary>
        /// 筛选器名称。
        /// </summary>
        public string[] Names
        {
            get
            {
                return this.Filter.FilterNamesAttribute;
            }
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string Remark
        {
            get
            {
                return this.Filter.FilterRemarkAttribute;
            }
        }

        /// <summary>
        /// 该筛选器是否需要使用账号信息。
        /// </summary>
        public bool UseAccount
        {
            get
            {
                return this.Filter is IUseAccount;
            }
        }
    }
}
