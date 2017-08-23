using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace AC.Base
{
    /// <summary>
    /// 对象信息转换类(将送入的类型的对象列表集合中所有对象的公共属性转换成属性参数列表格式存入数据库,并且能从数据库反初始化对象,送入的Type必须是类,不能是值类型)
    /// </summary>
    /// <typeparam name="T">对象信息类(必须是类,不能是值类型,且类中要保存的参数均为公共属性)</typeparam>
    public class InformationsConversion<T> : ISystemConfig
        where T : class, ICloneable, new()
    {
        /// <summary>
        /// 数据库中对应类型的全部对象信息的列表
        /// </summary>
        private List<T> _L_t;
        /// <summary>
        /// 参数转换用的对象
        /// </summary>
        private ParametersConversion<T> _pc_t = new ParametersConversion<T>();

        /// <summary>
        /// 获取或设置数据库中对应类型的全部对象信息的列表[每次都会获取到的是与数据库内数据相同的默认值信息类列表,设置时会自动更新程序集内默认值(若设置值是null,则默认值不会做任何改动),但不会修改数据库,调用Application.SetSystemConfig()方法将程序集内默认值更新到数据库中]
        /// </summary>
        public List<T> L_t
        {
            get
            {
                List<T> l_t_temp = new List<T>();
                if (this._L_t != null)
                {
                    l_t_temp = new List<T>();
                    if (this._L_t.Count > 0)
                        foreach (T t in this._L_t)
                            l_t_temp.Add(t.Clone() as T);
                }
                return l_t_temp;
            }
            set
            {
                if (value != null)
                {
                    if (this._L_t == null)
                        this._L_t = new List<T>();
                    else if (this._L_t.Count > 0)
                        this._L_t.Clear();
                    foreach (T t in value)
                        this._L_t.Add(t.Clone() as T);
                }
            }
        }

        /// <summary>
        /// 传入系统中保存的 XML 配置数据初始化该系统配置对象。
        /// </summary>
        /// <param name="deviceConfig"></param>
        public void SetConfig(System.Xml.XmlNode deviceConfig)
        {
            //deviceConfig.Name == 下面GetConfig(System.Xml.XmlDocument xmlDoc)中的根Node的Name"string.Format("{0}_List", typeof(T).ToString())"
            if (this._L_t == null)
                this._L_t = new List<T>();
            else if (this._L_t.Count > 0)
                this._L_t.Clear();
            if (this._pc_t == null)
                this._pc_t = new ParametersConversion<T>();
            foreach (XmlNode xn in deviceConfig.ChildNodes)
                if (xn.ChildNodes.Count > 0)
                {
                    //xn.Name == this._pc_t.GetConfig(System.Xml.XmlDocument xmlDoc)中的根Node的Name"typeof(T).ToString()"
                    this._pc_t.SetConfig(xn);
                    this._L_t.Add(this._pc_t.Paras);
                }
        }

        /// <summary>
        /// 获取当前系统配置对象 XML 形式的配置数据。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            XmlNode xnRoot = xmlDoc.CreateElement(string.Format("{0}_List", typeof(T).ToString()));
            XmlNode xnChild;
            if (this._pc_t == null)
                this._pc_t = new ParametersConversion<T>();
            if (this._L_t != null && this._L_t.Count > 0)
                foreach (T t in this._L_t)
                {
                    this._pc_t.Paras = t;
                    xnChild = this._pc_t.GetConfig(xmlDoc);
                    xnRoot.AppendChild(xnChild);
                }
            return xnRoot;
        }
    }
}
