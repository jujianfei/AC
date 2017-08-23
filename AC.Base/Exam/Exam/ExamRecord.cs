using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AC.Base.Exam.Exam
{
    public class ExamRecord
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
                return xmlConfig = this.SetExamRecordConfig();

            }
            set
            {
                xmlConfig = value;
                if (value != "")
                {
                    XmlDocument newXmlDocument = new XmlDocument();
                    newXmlDocument.LoadXml(value);
                    this.GetExamRecordConfig(newXmlDocument);
                }
            }
        }

        private List<ExamRecordValue> examRecordValueList = new List<ExamRecordValue>();

        public List<ExamRecordValue> ExamRecordValueList
        {
            get { return examRecordValueList; }
            set { examRecordValueList = value; }
        }



        public void GetExamRecordConfig(System.Xml.XmlNode xmlRootConfig)
        {
            foreach (System.Xml.XmlNode xmlConfig in xmlRootConfig.ChildNodes)
            {
                foreach (System.Xml.XmlNode xnConfig in xmlConfig.ChildNodes)
                {
                    ExamRecordValue examRecordValue = new ExamRecordValue();
                    ExamRecordValueDevice examRecordValueDevice = new ExamRecordValueDevice();
                    foreach (System.Xml.XmlNode node in xnConfig)
                    {

                        switch (node.Name)
                        {
                            case "device":
                                examRecordValue.Device = Convert.ToInt32(node.InnerText);
                                break;
                            case "item":
                                string[] values = node.InnerText.Split(new Char[] { ',' });
                                examRecordValueDevice.Item = Convert.ToInt32(values[0]);
                                examRecordValueDevice.Result = Convert.ToInt32(values[1]);
                                examRecordValueDevice.RecordId = Convert.ToInt32(values[2]);
                                examRecordValue.ExamRecordValueDeviceList.Add(examRecordValueDevice);
                                break;
                        }
                    }
                    this.ExamRecordValueList.Add(examRecordValue);
                }
            }
        }



        public String SetExamRecordConfig()
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.XmlNode xnRoot = xmlDoc.CreateElement("root");

            foreach (ExamRecordValue examRecordValue in examRecordValueList)
            {
                System.Xml.XmlDocument xmlDocChild = new System.Xml.XmlDocument();
                System.Xml.XmlNode xnRecord = xmlDoc.CreateElement("examItem");

                System.Xml.XmlNode xnExamRecordValue = xmlDoc.CreateElement("device");
                xnExamRecordValue.InnerText = examRecordValue.Device.ToString();
                xnRecord.AppendChild(xnExamRecordValue);

                foreach (ExamRecordValueDevice examRecordValueDevice in examRecordValue.ExamRecordValueDeviceList)
                {
                    System.Xml.XmlNode xnDevice = xmlDoc.CreateElement("item");
                    xnDevice.InnerText = examRecordValueDevice.Item + "," + examRecordValueDevice.Result + "," + examRecordValueDevice.RecordId;
                    xnRecord.AppendChild(xnDevice);
                    xnRoot.AppendChild(xnRecord);
                }

            }
            return xnRoot.OuterXml;
        }

    }
}
