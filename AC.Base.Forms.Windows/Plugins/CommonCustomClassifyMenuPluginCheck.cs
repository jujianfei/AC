using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.ClassifySearchs;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 检查系统内是否有已配置的公共分类
    /// </summary>
    public class CommonCustomClassifyMenuPluginCheck : IGlobalCheck
    {
        #region IGlobalCheck 成员

        /// <summary>
        /// 检查全局插件能否使用。
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public bool Check(FormApplicationClass application)
        {
            ClassifySearch _Search = new ClassifySearch(application);
            _Search.Filters.Add(new ParentIdFilter(0));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(CommonCustomClassify)));
            return _Search.Search().Count > 0;
        }

        #endregion
    }
}
