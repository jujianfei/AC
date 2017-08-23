using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Tasks;
using System.Data;

namespace AC.Base.Exam.Exam
{
    public class TaskRecord 
    {
        //private TaskTestingConfig TaskTestingConfig;

        //public TaskTestingConfig TaskTestingConfig
        //{
        //    get { return TaskTestingConfig; }
        //    set { TaskTestingConfig = value; }
        //}

       

        //private int commMod;

        //public int CommMod
        //{
        //    get { return commMod; }
        //    set { commMod = value; }
        //}


        //private int commProtocol;

        //public int CommProtocol
        //{
        //    get { return commProtocol; }
        //    set { commProtocol = value; }
        //}
        //private int commDevices;

        //public int CommDevices
        //{
        //    get { return commDevices; }
        //    set { commDevices = value; }
        //}


        //public  void SetTaskConfig(System.Xml.XmlNode taskConfig)
        //{
        //    foreach (System.Xml.XmlNode xnItem in taskConfig.ChildNodes)
        //    {
        //        switch (xnItem.Name)
        //        {
        //            case "basicdata":
        //                this.TaskTestingConfig.basicData = Boolean.Parse(xnItem.InnerText);
        //                break;
        //            case "upsdata":
        //                this.TaskTestingConfig.upsData = Boolean.Parse(xnItem.InnerText);
        //                break;
        //            case "signaldata":
        //                this.TaskTestingConfig.signalData = Boolean.Parse(xnItem.InnerText);
        //                break;
        //            case "commMod":
        //                this.commMod = Convert.ToInt32(xnItem.InnerText);
        //                break;
        //            case "commProtocol":
        //                this.commProtocol = Convert.ToInt32(xnItem.InnerText);
        //                break;
        //            case "commDevices":
        //                this.commDevices = Convert.ToInt32(xnItem.InnerText);
        //                break;
        //        }
        //    }
        //}

       


        //public  System.Xml.XmlNode GetTaskConfig(System.Xml.XmlDocument xmlDoc)
        //{
        //    System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

        //    System.Xml.XmlNode xnbasicdata = xmlDoc.CreateElement("basicdata");
        //    xnbasicdata.InnerText = this.TaskTestingConfig.basicData.ToString();
        //    xnConfig.AppendChild(xnbasicdata);

        //    System.Xml.XmlNode upsdata = xmlDoc.CreateElement("upsdata");
        //    upsdata.InnerText = this.TaskTestingConfig.upsData.ToString();
        //    xnConfig.AppendChild(upsdata);

        //    System.Xml.XmlNode signaldata = xmlDoc.CreateElement("signaldata");
        //    signaldata.InnerText = this.TaskTestingConfig.signalData.ToString();

        //    System.Xml.XmlNode xncommMod = xmlDoc.CreateElement("commMod");
        //    xncommMod.InnerText = this.commMod + "";
        //    xnConfig.AppendChild(xncommMod);
        //    System.Xml.XmlNode xncommProtocol = xmlDoc.CreateElement("commProtocol");
        //    xncommProtocol.InnerText = this.commProtocol + "";
        //    xnConfig.AppendChild(xncommProtocol);
        //    System.Xml.XmlNode xncommDevices = xmlDoc.CreateElement("commDevices");
        //    xncommDevices.InnerText = this.commDevices + "";
        //    xnConfig.AppendChild(xncommDevices);

        //    xnConfig.AppendChild(signaldata);

        //    return xnConfig;
        //}


        ///// <summary>
        ///// 保存当前任务信息。
        ///// </summary>
        //public void Save()
        //{
        //    if (this.TaskTestingConfig.Name == null || this.TaskTestingConfig.Name.Length == 0)
        //    {
        //        throw new Exception("任务名称必须输入。");
        //    }
        //    else if (this.TaskTestingConfig.Group == null)
        //    {
        //        throw new Exception("必须设置任务所属的任务组。");
        //    }
        //    else if (this.TaskTestingConfig.Period == null)
        //    {
        //        throw new Exception("必须配置任务的运行周期。");
        //    }

        //    AC.Base.Database.DbConnection dbConn = this.TaskTestingConfig.Application.GetDbConnection();
        //    if (dbConn != null)
        //    {
        //        try
        //        {
        //            string strConfig = null;
        //            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
        //            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
        //            xmlDoc.AppendChild(xnConfig);

        //            System.Xml.XmlNode xnPeriodType = xmlDoc.CreateElement("PeriodType");
        //            xnPeriodType.InnerText = this.TaskTestingConfig.Period.GetType().FullName;
        //            xnConfig.AppendChild(xnPeriodType);

        //            System.Xml.XmlNode xnPeriodConfig = this.TaskTestingConfig.Period.GetTaskPeriodConfig(xmlDoc);
        //            if (xnPeriodConfig != null)
        //            {
        //                System.Xml.XmlNode xnPeriod = xmlDoc.CreateElement("Period");
        //                xnPeriod.AppendChild(xnPeriodConfig);
        //                xnConfig.AppendChild(xnPeriod);
        //            }

        //            System.Xml.XmlNode xnMaxRunTime = xmlDoc.CreateElement("MaxRunTime");
        //            xnMaxRunTime.InnerText = this.TaskTestingConfig.MaxRunTime.ToString();
        //            xnConfig.AppendChild(xnMaxRunTime);

        //            System.Xml.XmlNode xnRetryProgress = xmlDoc.CreateElement("RetryProgress");
        //            xnRetryProgress.InnerText = this.TaskTestingConfig.RetryProgress.ToString();
        //            xnConfig.AppendChild(xnRetryProgress);

        //            System.Xml.XmlNode xnTaskConfig = this.GetTaskConfig(xmlDoc);
        //            if (xnTaskConfig != null)
        //            {
        //                System.Xml.XmlNode xn = xmlDoc.CreateElement("TaskConfig");
        //                xn.AppendChild(xnTaskConfig);
        //                xnConfig.AppendChild(xn);
        //            }

        //            strConfig = xmlDoc.OuterXml;
        //            string strSql = "SELECT MAX(" + classlib.Exam.Exam.TaskObject.TaskConfigId + ") FROM " + classlib.Exam.Exam.TaskObject.TableName;
        //            int TaskConfigId = Function.ToInt(dbConn.ExecuteScalar(strSql)) + 1;

        //            strSql = this.TaskTestingConfig.TaskConfigId + "," + Function.SqlStr(this.TaskTestingConfig.TaskType.Code, 250) + "," + Function.SqlStr(this.TaskTestingConfig.Name, 250) + "," + this.TaskTestingConfig.Group.TaskGroupId + "," + Function.BoolToByte(this.TaskTestingConfig.Enable) + "," + Function.SqlStr(strConfig);
        //            strSql = "INSERT INTO " + classlib.Exam.Exam.TaskObject.TableName + " (" + classlib.Exam.Exam.TaskObject.TaskConfigId + "," + classlib.Exam.Exam.TaskObject.TaskType + "," + classlib.Exam.Exam.TaskObject.Name + "," + classlib.Exam.Exam.TaskObject.TaskGroupId + "," + classlib.Exam.Exam.TaskObject.EnableAuto + "," + classlib.Exam.Exam.TaskObject.XMLConfig + ") VALUES (" + strSql + ")";
        //            dbConn.ExecuteNonQuery(strSql);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            dbConn.Close();
        //        }
        //    }
        //}

    }
}
