using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AC.Base.Exam.Exam
{
    public class Record
    {
        private int baseKey;

        public int BaseKey
        {
            get { return baseKey; }
            set { baseKey = value; }
        }

        private int taskConfigId;

        public int TaskConfigId
        {
            get { return taskConfigId; }
            set { taskConfigId = value; }
        }

        private String xmlConfig;

        public String XmlConfig
        {
            get
            {
                return xmlConfig=this.SetRecordConfig(); 
                
            }
            set
            { 
                xmlConfig = value;
                if (xmlConfig != "")
                {
                    XmlDocument newXmlDocument = new XmlDocument();
                    newXmlDocument.LoadXml(value);
                    this.GetRecordConfig(newXmlDocument);
                }
            }
        }

        private List<RecordValue> recordValueList = new List<RecordValue>();

        public  List<RecordValue> RecordValueList
        {
            get { return recordValueList; }
            set { recordValueList = value; }
        }

     
        public void GetRecordConfig(System.Xml.XmlNode xmlConfig)
        {
            
            foreach (System.Xml.XmlNode xnConfig in xmlConfig.ChildNodes)
            {
                RecordValue record = new RecordValue();
                foreach (System.Xml.XmlNode node in xnConfig)
                {
                   
                    switch (node.Name)
                    {
                        case "recordId":
                            record.RecordId = node.InnerText;
                            break;
                        case "value":
                            record.Value = node.InnerText;
                            break;
                    }
                }
                this.RecordValueList.Add(record);
            }
        }



        public String SetRecordConfig()
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.XmlNode xnRoot = xmlDoc.CreateElement("root");

            foreach (RecordValue recordValue in recordValueList)
            {
                System.Xml.XmlDocument xmlDocChild = new System.Xml.XmlDocument();
                System.Xml.XmlNode xnRecord = xmlDoc.CreateElement("record");

                System.Xml.XmlNode xnRecordId = xmlDoc.CreateElement("recordId");
                xnRecordId.InnerText = recordValue.RecordId;
                xnRecord.AppendChild(xnRecordId);

                System.Xml.XmlNode xnValue = xmlDoc.CreateElement("value");
                xnValue.InnerText = recordValue.Value;
                xnRecord.AppendChild(xnValue);
                xnRoot.AppendChild(xnRecord);

            }
            return  xnRoot.InnerXml;
        }
    }
}
