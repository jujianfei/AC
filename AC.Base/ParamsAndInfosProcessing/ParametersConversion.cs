using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;

namespace AC.Base
{
    /// <summary>
    /// 配置信息参数转换类(将送入的类型的所有公共属性转换成属性参数列表格式存入数据库,并且能从数据库反初始化对象,送入的Type必须是类,不能是值类型)
    /// </summary>
    /// <typeparam name="T">类型(必须是类,不能是值类型,且类中要保存的参数均为公共属性)</typeparam>
    public class ParametersConversion<T> : ISystemConfig
        where T : class, ICloneable, new()
    {
        /// <summary>
        /// 配置信息类T的所有公共参数的对象列表
        /// </summary>
        private List<ParametersEntity> _l_ppe;
        /// <summary>
        /// 配置信息类T的对象
        /// </summary>
        private T _Paras = new T();

        /// <summary>
        /// 获取或设置配置信息类T的对象[每次都会获取到的是与数据库内数据相同的默认值参数类,设置时会自动更新程序集内默认值(若设置值是null,则默认值不会做任何改动),但不会修改数据库,调用Application.SetSystemConfig()方法将程序集内默认值更新到数据库中]
        /// </summary>
        public T Paras
        {
            get
            {
                if (this._l_ppe != null && this._l_ppe.Count > 0)
                {
                    this._Paras = new T();
                    //获取类型信息 
                    Type t = typeof(T);
                    //获取对象引用 
                    object oParas = this._Paras;
                    //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值 
                    BindingFlags bf = BindingFlags.Public | BindingFlags.Instance;
                    //获取属性信息数组
                    PropertyInfo[] piArr = t.GetProperties();
                    foreach (PropertyInfo pi in piArr)
                    {
                        try
                        {
                            ParametersEntity ppe = this._l_ppe.Find(delegate(ParametersEntity sppe_temp) { return pi.Name.ToLower().Equals(sppe_temp.PP_ParaName.ToLower()) && pi.PropertyType.ToString().ToLower().Contains(sppe_temp.PP_ParaValueType.ToLower()); });
                            //给属性赋值
                            if (ppe != null && ppe.PP_ParaValue != ppe.PP_ParaValueType)//PP_ParaValue等于PP_ParaValueType时代表此对象是个数组
                                pi.SetValue(oParas, Convert.ChangeType(ppe.PP_ParaValue, pi.PropertyType), bf, null, null, null);
                        }
                        catch
                        { }
                    }
                }
                return this._Paras.Clone() as T;
            }
            set
            {
                if (value != null)
                {
                    this._Paras = value.Clone() as T;

                    #region << 修改_l_ppe中的内容 >>
                    if (this._l_ppe == null)
                        this._l_ppe = new List<ParametersEntity>();
                    else if (this._l_ppe.Count > 0)
                        this._l_ppe.Clear();
                    //获取类型信息 
                    Type t = typeof(T);
                    //获取对象引用 
                    object oParas = this._Paras;
                    //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值 
                    BindingFlags bf = BindingFlags.Public | BindingFlags.Instance;
                    //获取属性信息数组
                    PropertyInfo[] piArr = t.GetProperties();
                    foreach (PropertyInfo pi in piArr)
                    {
                        //this._l_ppe.Find(delegate(ParametersEntity sppe_temp) { return pi.Name.ToLower().Equals(sppe_temp.PP_ParaName.ToLower()) && pi.PropertyType.ToString().ToLower().Contains(sppe_temp.PP_ParaValueType.ToLower()); });
                        ParametersEntity ppe = new ParametersEntity();
                        ppe.PP_ParaName = pi.Name;
                        ppe.PP_ParaValue = pi.GetValue(oParas, bf, null, null, null).ToString();
                        ppe.PP_ParaValueType = pi.PropertyType.ToString();
                        this._l_ppe.Add(ppe);
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 传入系统中保存的 XML 配置数据初始化该系统配置对象。
        /// </summary>
        /// <param name="deviceConfig"></param>
        public void SetConfig(System.Xml.XmlNode deviceConfig)
        {
            if (this._l_ppe == null)
                this._l_ppe = new List<ParametersEntity>();
            else if (this._l_ppe.Count > 0)
                this._l_ppe.Clear();
            ParametersEntity ppe;
            foreach (XmlNode xn in deviceConfig.ChildNodes)
            {
                if (xn.Name.ToUpper().Equals("P"))
                {
                    ppe = new ParametersEntity();
                    ppe.PP_ParaName = xn.Attributes["N"].Value;
                    ppe.PP_ParaValue = xn.Attributes["V"].Value;
                    ppe.PP_ParaValueType = xn.Attributes["T"].Value;
                    this._l_ppe.Add(ppe);
                }
            }
        }

        /// <summary>
        /// 获取当前系统配置对象 XML 形式的配置数据。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public System.Xml.XmlNode GetConfig(System.Xml.XmlDocument xmlDoc)
        {
            XmlNode xnRoot = xmlDoc.CreateElement(typeof(T).ToString());
            if (this._l_ppe != null && this._l_ppe.Count > 0)
            {
                XmlElement xe;
                foreach (ParametersEntity ppe in this._l_ppe)
                {
                    xe = xmlDoc.CreateElement("P");
                    xe.SetAttribute("N", ppe.PP_ParaName);
                    xe.SetAttribute("V", ppe.PP_ParaValue);
                    xe.SetAttribute("T", ppe.PP_ParaValueType);
                    xe.InnerText = string.Empty;
                    xnRoot.AppendChild(xe);
                }
            }
            return xnRoot;
        }

        /// <summary>
        /// 获取当前类型公共属性的克隆代码,并在保存完成后自动打开生成的txt文本
        /// </summary>
        internal void GetCurrentTypeCloneCode()
        {
            GetTypeCloneCode(typeof(T));
        }

        /// <summary>
        /// 获取当前类型公共属性的克隆代码,并在保存完成后自动打开生成的txt文本
        /// </summary>
        private void GetTypeCloneCode(Type type)
        {
            //获取属性信息数组
            PropertyInfo[] piArr = type.GetProperties();
            string txtpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.txt", type.Name));
            StreamWriter sw = new StreamWriter(txtpath, true, Encoding.GetEncoding("gb2312"));
            sw.WriteLine(string.Format("此文本所在路径:\r\n{0}\r\n", txtpath));
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("        /// <summary>\r\n        /// 创建作为当前实例副本的新对象。\r\n        /// </summary>\r\n        /// <returns>作为此实例副本的新对象。</returns>\r\n        public object Clone()\r\n        {0}\r\n            {1} item = new {1}();\r\n", '{', type.Name));
            try
            {
                foreach (PropertyInfo pi in piArr)
                    sb.Append(string.Format("            item.{0} = this.{0};\r\n", pi.Name));
            }
            catch (Exception ex)
            {
                sb.Append(string.Format("\r\n\r\n\r\n\r\n{0}\r\n\r\n\r\n\r\n", ex.Message));
            }
            finally
            {
                sb.Append("            return item;\r\n        }\r\n");
                sw.WriteLine(sb.ToString());
                sw.Flush();
                sw.Close();
                sw.Dispose();
                System.Diagnostics.Process.Start(txtpath);
            }
        }
    }
}
