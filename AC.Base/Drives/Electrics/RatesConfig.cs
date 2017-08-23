using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率配置。
    /// </summary>
    public class RatesConfig
    {
        private System.Collections.Generic.Dictionary<int, RatesOptions> dicRates = new Dictionary<int, RatesOptions>();

        /// <summary>
        /// 费率配置。
        /// </summary>
        public RatesConfig()
        {
        }

        /// <summary>
        /// 费率配置。
        /// </summary>
        /// <param name="pikeRatesNumber">尖费率对应的费率号。如无该费率则传入“0”。</param>
        /// <param name="peakRatesNumber">峰费率对应的费率号。如无该费率则传入“0”。</param>
        /// <param name="flatRatesNumber">平费率对应的费率号。如无该费率则传入“0”。</param>
        /// <param name="valeRatesNumber">谷费率对应的费率号。如无该费率则传入“0”。</param>
        public RatesConfig(int pikeRatesNumber, int peakRatesNumber, int flatRatesNumber, int valeRatesNumber)
        {
            if (pikeRatesNumber > 0)
            {
                this.dicRates.Add(pikeRatesNumber, RatesOptions.Pike);
            }
            if (peakRatesNumber > 0)
            {
                this.dicRates.Add(peakRatesNumber, RatesOptions.Peak);
            }
            if (flatRatesNumber > 0)
            {
                this.dicRates.Add(flatRatesNumber, RatesOptions.Flat);
            }
            if (valeRatesNumber > 0)
            {
                this.dicRates.Add(valeRatesNumber, RatesOptions.Vale);
            }
        }

        /// <summary>
        /// 获取费率数对应的费率
        /// </summary>
        /// <param name="ratesNumber"></param>
        /// <returns></returns>
        public RatesOptions GetRates(int ratesNumber)
        {
            if (ratesNumber == 0)
            {
                return RatesOptions.Total;
            }
            else
            {
                if (this.dicRates.ContainsKey(ratesNumber))
                {
                    return this.dicRates[ratesNumber];
                }
                else
                {
                    throw new Exception("无费率" + ratesNumber + "对应的费率。");
                }
            }
        }

        /// <summary>
        /// 获取费率对应的费率数。
        /// </summary>
        /// <param name="rates"></param>
        /// <returns></returns>
        public int GetRatesNumber(RatesOptions rates)
        {
            if (rates == RatesOptions.Total)
            {
                return 0;
            }

            foreach (System.Collections.Generic.KeyValuePair<int, RatesOptions> kvp in this.dicRates)
            {
                if (kvp.Value == rates)
                {
                    return kvp.Key;
                }
            }
            throw new Exception("无" + rates.GetDescription() + "费率对应的费率数。");
        }

        /// <summary>
        /// 获取支持的全部费率（不含总费率）。
        /// </summary>
        /// <returns></returns>
        public RatesOptions[] GetRates()
        {
            return this.dicRates.Values.ToArray();
        }

        /// <summary>
        /// 获取设备费率数量（不含总费率）。
        /// </summary>
        /// <returns></returns>
        public int GetRatesCount()
        {
            return this.dicRates.Count;
        }

        /// <summary>
        /// 从保存此设备数据的 XML 文档节点初始化当前费率设置。
        /// </summary>
        /// <param name="config">该费率设置节点的数据</param>
        public void SetRatesConfig(System.Xml.XmlNode config)
        {
            this.dicRates.Clear();

            foreach (System.Xml.XmlNode xnConfigItem in config.ChildNodes)
            {
                switch (xnConfigItem.Name)
                {
                    case "Pike":
                        if (Function.ToInt(xnConfigItem.InnerText) > 0)
                        {
                            this.dicRates.Add(Function.ToInt(xnConfigItem.InnerText), RatesOptions.Pike);
                        }
                        break;

                    case "Peak":
                        if (Function.ToInt(xnConfigItem.InnerText) > 0)
                        {
                            this.dicRates.Add(Function.ToInt(xnConfigItem.InnerText), RatesOptions.Peak);
                        }
                        break;

                    case "Flat":
                        if (Function.ToInt(xnConfigItem.InnerText) > 0)
                        {
                            this.dicRates.Add(Function.ToInt(xnConfigItem.InnerText), RatesOptions.Flat);
                        }
                        break;

                    case "Vale":
                        if (Function.ToInt(xnConfigItem.InnerText) > 0)
                        {
                            this.dicRates.Add(Function.ToInt(xnConfigItem.InnerText), RatesOptions.Vale);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 获取当前费率设置的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns>如无配置内容则返回 null。</returns>
        public System.Xml.XmlNode GetRatesConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            foreach (System.Collections.Generic.KeyValuePair<int, RatesOptions> kvp in this.dicRates)
            {
                switch (kvp.Value)
                {
                    case RatesOptions.Pike:
                        System.Xml.XmlNode xnPike = xmlDoc.CreateElement("Pike");
                        xnPike.InnerText = kvp.Key.ToString();
                        xnConfig.AppendChild(xnPike);
                        break;

                    case RatesOptions.Peak:
                        System.Xml.XmlNode xnPeak = xmlDoc.CreateElement("Peak");
                        xnPeak.InnerText = kvp.Key.ToString();
                        xnConfig.AppendChild(xnPeak);
                        break;

                    case RatesOptions.Flat:
                        System.Xml.XmlNode xnFlat = xmlDoc.CreateElement("Flat");
                        xnFlat.InnerText = kvp.Key.ToString();
                        xnConfig.AppendChild(xnFlat);
                        break;

                    case RatesOptions.Vale:
                        System.Xml.XmlNode xnVale = xmlDoc.CreateElement("Vale");
                        xnVale.InnerText = kvp.Key.ToString();
                        xnConfig.AppendChild(xnVale);
                        break;
                }
            }

            return xnConfig;
        }
    }
}
