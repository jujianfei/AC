using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 系统配置接口。实现该接口的类可以通过 Application.SetSystemConfig() 方法将系统配置数据保存在系统中，也可以通过 Application.GetSystemConfig() 方法获取该配置对象的实例。
    /// </summary>
    public interface ISystemConfig
    {
        /// <summary>
        /// 传入系统中保存的 XML 配置数据初始化该系统配置对象。
        /// </summary>
        /// <param name="deviceConfig"></param>
        void SetConfig(System.Xml.XmlNode deviceConfig);

        /// <summary>
        /// 获取当前系统配置对象 XML 形式的配置数据。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc);
    }
}
