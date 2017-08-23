using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 数据库连接配置界面。实现该接口的类必须添加 ControlAttribute 属性。
    /// </summary>
    public interface IDbConfigControl : IControl
    {
        /// <summary>
        /// 设置数据库对象，并将该连接内容显示在界面控件上。
        /// </summary>
        /// <param name="db"></param>
        void SetDb(IDb db);

        /// <summary>
        /// 获取该数据库连接配置信息。
        /// </summary>
        /// <returns></returns>
        IDb GetDb();
    }
}
